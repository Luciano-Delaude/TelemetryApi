using TelemetryApi.Models;

namespace TelemetryApi.Services;

public static class Diagnostics
{
    public static string GetStatus(TelemetrySample s)
    {
        if (s.CpuUsage > 85 || s.MemoryUsage > 90) return "High Resource Usage";
        if (s.DiskErrors > 5) return "Disk Issues Detected";
        if (s.NetworkLatency > 200) return "Network Latency High";
        return "Normal";
    }
}
