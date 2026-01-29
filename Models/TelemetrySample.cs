namespace TelemetryApi.Models;
public class TelemetrySample
{
    public string? ClientId { get; set; }
    public DateTimeOffset Timestamp { get; set; }    
    public double CpuUsage { get; set; }            
    public double MemoryUsage { get; set; }         
    public int DiskErrors { get; set; }             
    public int NetworkLatency { get; set; } 
}
