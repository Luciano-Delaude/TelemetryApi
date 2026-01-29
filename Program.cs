using System.Threading.Channels;
using TelemetryApi.Models;
using TelemetryApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITelemetryLatestStore, InMemoryTelemetryLatestStore>();

builder.Services.AddSingleton(Channel.CreateBounded<TelemetrySample>(
    new BoundedChannelOptions(capacity: 10_000)
    {
        SingleReader = true,
        SingleWriter = false,
        FullMode = BoundedChannelFullMode.DropWrite
    }));

builder.Services.AddHostedService<TelemetryIngestionWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/telemetry", async (
    TelemetrySample sample,
    Channel<TelemetrySample> channel,
    ILoggerFactory loggerFactory,
    CancellationToken ct) =>
{
    var logger = loggerFactory.CreateLogger("TelemetryIngest");

    // Validación básica
    if (string.IsNullOrWhiteSpace(sample.ClientId))
        return Results.BadRequest(new { error = "ClientId is required" });

    if (sample.Timestamp == default)
        return Results.BadRequest(new { error = "Timestamp is required" });

    if (sample.CpuUsage is < 0 or > 100)
        return Results.BadRequest(new { error = "CpuUsage must be between 0 and 100" });

    if (sample.MemoryUsage is < 0 or > 100)
        return Results.BadRequest(new { error = "MemoryUsage must be between 0 and 100" });

    if (sample.DiskErrors < 0)
        return Results.BadRequest(new { error = "DiskErrors must be >= 0" });

    if (sample.NetworkLatency < 0)
        return Results.BadRequest(new { error = "NetworkLatency must be >= 0" });

    var now = DateTimeOffset.UtcNow;
    if (sample.Timestamp > now.AddMinutes(5))
        return Results.BadRequest(new { error = "Timestamp is in the future" });

    if (!channel.Writer.TryWrite(sample))
        return Results.StatusCode(503);

    return Results.Accepted();
});

app.MapGet("/clients/{clientId}/diagnostics", (
    string clientId,
    ITelemetryLatestStore store,
    ILogger<Program> logger) =>
{
    if (!store.TryGetLatest(clientId, out var latest) || latest is null)
        return Results.NotFound(new { error = "Client not found" });

    var status = Diagnostics.GetStatus(latest);

    if (status != "Normal")
        logger.LogWarning("Predictive status {Status} for client {ClientId}", status, clientId);

    return Results.Ok(new
    {
        clientId,
        status,
        telemetry = latest
    });
});

app.Run();

public partial class Program { }
