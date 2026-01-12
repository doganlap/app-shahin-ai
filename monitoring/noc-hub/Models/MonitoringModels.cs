namespace NocHub.Models
{
    /// <summary>
    /// Configuration for a monitoring tool
    /// </summary>
    public class MonitoringToolConfig
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string HealthCheckUrl { get; set; } = string.Empty;
        public string Category { get; set; } = "Monitoring";
        public int Port { get; set; }
    }

    /// <summary>
    /// Health status of a monitoring tool
    /// </summary>
    public class ToolStatus
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public bool IsResponding { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime LastCheck { get; set; }
        public int ResponseTimeMs { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// System metrics aggregated from Prometheus
    /// </summary>
    public class SystemMetrics
    {
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public double DiskUsage { get; set; }
        public double NetworkBandwidthIn { get; set; }
        public double NetworkBandwidthOut { get; set; }
        public string SystemUptime { get; set; } = string.Empty;
        public int LoadAverage1Min { get; set; }
        public int LoadAverage5Min { get; set; }
        public int LoadAverage15Min { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Alert aggregated from monitoring tools
    /// </summary>
    public class AlertInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "info"; // info, warning, critical
        public string Source { get; set; } = string.Empty; // prometheus, zabbix, netdata
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// NOC Dashboard view model
    /// </summary>
    public class NocDashboardViewModel
    {
        public string ServerName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<MonitoringToolConfig> Tools { get; set; } = new();
        public List<ToolStatus> ToolStatuses { get; set; } = new();
        public SystemMetrics SystemMetrics { get; set; } = new();
        public List<AlertInfo> ActiveAlerts { get; set; } = new();
        public DateTime DashboardRefreshedAt { get; set; }
        public int HealthyToolsCount { get; set; }
        public int TotalToolsCount { get; set; }
        public int ActiveAlertsCount { get; set; }
    }
}
