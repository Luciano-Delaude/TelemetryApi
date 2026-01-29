using TelemetryApi.Models;

namespace TelemetryApi.Services;

public interface ITelemetryLatestStore
{
    void UpsertIfNewer(TelemetrySample sample);
    bool TryGetLatest(string clientId, out TelemetrySample? latest);
}
