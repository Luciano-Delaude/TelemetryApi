using System.Text.Json;
namespace TelemetryApi.Models;

public class TelemetryEvent
{
    public string? EventName { get; set; }
    public string? SessionId {get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Dictionary<string, JsonElement> Properties { get; set; }

    public TelemetryEvent(string eventName)
    {
        EventName = eventName;
        Timestamp = DateTime.UtcNow;
        Properties = new Dictionary<string, JsonElement>();
    }
    
} 

