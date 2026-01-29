using System.Text.Json;
namespace TelemetryApi.Models;

public class TelemetryEvent
{
    public string? EventName { get; set; }
    public string? SessionId {get; set; }
    public float CpuUsage { get; set; }
    public float MemoryUsage { get; set; }
    public int DiskErrors { get; set; }
    public int NetworkLatencyMs { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Dictionary<string, JsonElement> Properties { get; set; }

    public TelemetryEvent(string eventName)
    {
        EventName = eventName;
        Timestamp = DateTime.UtcNow;
        CpuUsage = 0;
        MemoryUsage = 0;
        DiskErrors = 0;
        NetworkLatencyMs = 0;
        Properties = new Dictionary<string, JsonElement>();
    }
    
} 

