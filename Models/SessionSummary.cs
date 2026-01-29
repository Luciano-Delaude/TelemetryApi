namespace TelemetryApi.Models;

public class SessionSummary{
    public string SessionId {get; set; }
    public DateTimeOffset StartTime {get; set; }
    public DateTimeOffset EndTime {get; set; }
    public Dictionary<string, int> CountsByType {get; set; } = new();

    public SessionSummary(string sessionId, DateTimeOffset startTime, DateTimeOffset endTime, Dictionary<string, int> countsByType){
        SessionId = sessionId;
        StartTime = startTime;
        EndTime = endTime;
        CountsByType = countsByType;
    }
}