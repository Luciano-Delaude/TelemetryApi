using System.Collections.Concurrent;
using TelemetryApi.Models;

namespace TelemetryApi.Services;

public class InMemoryTelemetryLatestStore : ITelemetryLatestStore
{
    private readonly ConcurrentDictionary<string, TelemetrySample> _latestByClient = new();

    public void UpsertIfNewer(TelemetrySample sample)
    {
        // Idempotencia simple: solo actualiza si es más nuevo (timestamp mayor).
        _latestByClient.AddOrUpdate(
            sample.ClientId!,               // validado antes
            sample,
            (_, existing) => sample.Timestamp > existing.Timestamp ? sample : existing
        );
    }

    public bool TryGetLatest(string clientId, out TelemetrySample? latest)
    {
        if (_latestByClient.TryGetValue(clientId, out var v))
        {
            latest = v;
            return true;
        }
        latest = null;
        return false;
    }
}
