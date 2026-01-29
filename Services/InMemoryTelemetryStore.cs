using System.Collections.Concurrent;
using TelemetryApi.Models;
using System.Threading.Tasks;

namespace TelemetryApi.Services;
public class InMemoryTelemetryStore : ITelemetryStore
{
    private readonly ConcurrentDictionary<string, SessionAggregator> _sessions = new();

    public void AddEvent(TelemetryEvent telemetryEvent){
        var sessionAggregator = _sessions.GetOrAdd(telemetryEvent.SessionId, _ => new SessionAggregator(telemetryEvent.Timestamp));
        sessionAggregator.AddEvent(telemetryEvent);
    }

    public bool TryGetSessionSummary(string sessionId, out SessionSummary? summary){
        if(_sessions.TryGetValue(sessionId, out var aggregator)){
            summary = aggregator.ToSummary(sessionId);
            return true;
        }
        else{
            summary = null;
            return false;
        }
    }

    private class SessionAggregator {
        private readonly object _lock = new();
        private DateTimeOffset _startTime;
        private DateTimeOffset _endTime;
        private readonly Dictionary<string, int> _countsByType = new(StringComparer.Ordinal);

        public SessionAggregator(DateTimeOffset initialTimestamp){
            _startTime = initialTimestamp;
            _endTime = initialTimestamp;
        }

        public void AddEvent(TelemetryEvent telemetryEvent){
            lock(_lock) {
                if(telemetryEvent.Timestamp < _startTime){
                    _startTime = telemetryEvent.Timestamp;
                }
                if(telemetryEvent.Timestamp > _endTime){
                    _endTime = telemetryEvent.Timestamp;
                }
                _countsByType.TryGetValue(telemetryEvent.EventName, out var count);
                _countsByType[telemetryEvent.EventName] = count + 1;
            }
        }

        public SessionSummary ToSummary(string sessionId){
            lock(_lock){
                return new SessionSummary(
                    sessionId,
                    _startTime,
                    _endTime,
                    new Dictionary<string, int>(_countsByType, StringComparer.Ordinal)
                );
            }
        }
    }
}