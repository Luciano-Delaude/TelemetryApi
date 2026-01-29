using TelemetryApi.Models;

namespace TelemetryApi.Services;

public interface ITelemetryStore
{
    void AddEvent(TelemetryEvent telemetryEvent);

    bool TryGetSessionSummary(string sessionId, out SessionSummary? summary);
}
