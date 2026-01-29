using System.Threading.Channels;
using TelemetryApi.Models;

namespace TelemetryApi.Services;

public class TelemetryIngestionWorker : BackgroundService
{
    private readonly ChannelReader<TelemetrySample> _reader;
    private readonly ITelemetryLatestStore _store;
    private readonly ILogger<TelemetryIngestionWorker> _logger;

    public TelemetryIngestionWorker(
        Channel<TelemetrySample> channel,
        ITelemetryLatestStore store,
        ILogger<TelemetryIngestionWorker> logger)
    {
        _reader = channel.Reader;
        _store = store;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var sample in _reader.ReadAllAsync(stoppingToken))
        {
            _store.UpsertIfNewer(sample);
            // opcional: log de eventos “problemáticos” podrías hacerlo aquí o en GET
        }
    }
}
