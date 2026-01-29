using Microsoft.AspNetCore.Mvc;
using TelemetryApi.Models;
using TelemetryApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ITelemetryStore, InMemoryTelemetryStore>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/health", () => Results.Ok(new { status = "Healty"}));

app.MapPost("/ingest", ([FromBody] TelemetryEvent telemetryEvent, 
                                    ITelemetryStore telemetryStore,
                                    CancellationToken cancellationToken) =>{
            if (string.IsNullOrWhiteSpace(telemetryEvent.SessionId))
            {
                return Results.BadRequest(new { error = "SessionId is required." });
            }
            
            if (string.IsNullOrWhiteSpace(telemetryEvent.EventName))
            {
                return Results.BadRequest(new {error = "EventName is required."});
            }

            if (telemetryEvent.Timestamp == default){
                return Results.BadRequest( new { error = "Timestamp is required."});
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return Results.StatusCode(499); // Client Closed Request
            }
            
            telemetryStore.AddEvent(telemetryEvent);
            return Results.Accepted();
            });

app.MapGet("/sessions/{sessionId}/summary", (string sessionId, ITelemetryStore telemetryStore) =>{
    if(telemetryStore.TryGetSessionSummary(sessionId, out var summary)){
        return Results.Ok(summary);
    }
    else{
        return Results.NotFound(new { error = "Session not found."});
    }});

app.Run();
