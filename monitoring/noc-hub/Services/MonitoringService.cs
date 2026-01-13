using NocHub.Models;
using System.Text.Json;

namespace NocHub.Services
{
    // Models are now in Models/MonitoringModels.cs

    #region Interfaces

    /// <summary>
    /// Service interface for monitoring tool configuration and status
    /// </summary>
    public interface IMonitoringToolService
    {
        Task<List<MonitoringToolConfig>> GetAllToolsAsync();
        Task<MonitoringToolConfig?> GetToolByIdAsync(string id);
        Task<List<ToolStatus>> GetAllToolsStatusAsync();
        Task<ToolStatus?> GetToolStatusAsync(string id);
    }

    /// <summary>
    /// Service interface for system metrics
    /// </summary>
    public interface IMetricsService
    {
        Task<SystemMetrics> GetSystemMetricsAsync();
        Task<double> GetCpuUsageAsync();
        Task<double> GetMemoryUsageAsync();
        Task<double> GetDiskUsageAsync();
        Task<string> GetUptimeAsync();
    }

    /// <summary>
    /// Service interface for alert aggregation
    /// </summary>
    public interface IAlertService
    {
        Task<List<AlertInfo>> GetActiveAlertsAsync();
        Task<List<AlertInfo>> GetAlertsFromPrometheusAsync();
        Task<List<AlertInfo>> GetAlertsFromNetdataAsync();
        Task<int> GetAlertCountAsync();
    }

    /// <summary>
    /// Service interface for proxy requests
    /// </summary>
    public interface IProxyService
    {
        Task<string> ProxyRequestAsync(string toolId, string endpoint);
        Task<string> GetToolDataAsync(string toolId, string dataType);
    }

    #endregion

    #region Implementations

    /// <summary>
    /// MonitoringToolService - Manages tool configuration and health checks
    /// </summary>
    public class MonitoringToolService : IMonitoringToolService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MonitoringToolService> _logger;

        public MonitoringToolService(HttpClient httpClient, ILogger<MonitoringToolService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<MonitoringToolConfig>> GetAllToolsAsync()
        {
            var tools = new List<MonitoringToolConfig>
            {
                new MonitoringToolConfig
                {
                    Id = "netdata",
                    Name = "Netdata",
                    Url = "http://localhost:19999",
                    Description = "Real-time performance monitoring with sub-second updates",
                    Icon = "📊",
                    Category = "Real-time Monitoring",
                    Port = 19999,
                    HealthCheckUrl = "http://localhost:19999/api/v1/info"
                },
                new MonitoringToolConfig
                {
                    Id = "prometheus",
                    Name = "Prometheus",
                    Url = "http://localhost:9090",
                    Description = "Metrics collection, storage, and querying with PromQL",
                    Icon = "🔥",
                    Category = "Metrics Database",
                    Port = 9090,
                    HealthCheckUrl = "http://localhost:9090/-/healthy"
                },
                new MonitoringToolConfig
                {
                    Id = "grafana",
                    Name = "Grafana",
                    Url = "http://localhost:3000",
                    Description = "Beautiful dashboards and data visualization",
                    Icon = "📈",
                    Category = "Visualization",
                    Port = 3000,
                    HealthCheckUrl = "http://localhost:3000/api/health"
                },
                new MonitoringToolConfig
                {
                    Id = "zabbix",
                    Name = "Zabbix",
                    Url = "http://localhost:8080",
                    Description = "Enterprise monitoring with advanced alerting and automation",
                    Icon = "⚡",
                    Category = "Enterprise Monitoring",
                    Port = 8080,
                    HealthCheckUrl = "http://localhost:8080/"
                }
            };

            return await Task.FromResult(tools);
        }

        public async Task<MonitoringToolConfig?> GetToolByIdAsync(string id)
        {
            var tools = await GetAllToolsAsync();
            return tools.FirstOrDefault(t => t.Id == id);
        }

        public async Task<List<ToolStatus>> GetAllToolsStatusAsync()
        {
            var tools = await GetAllToolsAsync();
            var statuses = new List<ToolStatus>();

            foreach (var tool in tools)
            {
                var status = await GetToolStatusAsync(tool.Id);
                if (status != null)
                    statuses.Add(status);
            }

            return statuses;
        }

        public async Task<ToolStatus?> GetToolStatusAsync(string id)
        {
            var tool = await GetToolByIdAsync(id);
            if (tool == null) return null;

            var status = new ToolStatus
            {
                Id = tool.Id,
                Name = tool.Name,
                Url = tool.Url,
                LastCheck = DateTime.UtcNow
            };

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                var response = await _httpClient.GetAsync(tool.HealthCheckUrl, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                
                stopwatch.Stop();
                status.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                status.IsHealthy = response.IsSuccessStatusCode;
                status.IsResponding = true;
                status.StatusMessage = response.StatusCode.ToString();
            }
            catch (OperationCanceledException)
            {
                status.IsHealthy = false;
                status.IsResponding = false;
                status.StatusMessage = "Timeout";
            }
            catch (Exception ex)
            {
                status.IsHealthy = false;
                status.IsResponding = false;
                status.StatusMessage = "Unreachable";
                _logger.LogWarning($"Health check failed for {tool.Name}: {ex.Message}");
            }

            return status;
        }
    }

    /// <summary>
    /// MetricsService - Aggregates metrics from Prometheus and system
    /// </summary>
    public class MetricsService : IMetricsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MetricsService> _logger;

        public MetricsService(HttpClient httpClient, ILogger<MetricsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<SystemMetrics> GetSystemMetricsAsync()
        {
            var metrics = new SystemMetrics
            {
                Timestamp = DateTime.UtcNow,
                CpuUsage = await GetCpuUsageAsync(),
                MemoryUsage = await GetMemoryUsageAsync(),
                DiskUsage = await GetDiskUsageAsync(),
                SystemUptime = await GetUptimeAsync()
            };

            return metrics;
        }

        public async Task<double> GetCpuUsageAsync()
        {
            try
            {
                var query = "100 - (avg(rate(node_cpu_seconds_total{mode=\"idle\"}[1m])) * 100)";
                return await QueryPrometheusAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error fetching CPU: {ex.Message}");
                return 0;
            }
        }

        public async Task<double> GetMemoryUsageAsync()
        {
            try
            {
                var query = "(1 - (node_memory_MemAvailable_bytes / node_memory_MemTotal_bytes)) * 100";
                return await QueryPrometheusAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error fetching Memory: {ex.Message}");
                return 0;
            }
        }

        public async Task<double> GetDiskUsageAsync()
        {
            try
            {
                var query = "(1 - (node_filesystem_avail_bytes{mountpoint=\"/\"} / node_filesystem_size_bytes{mountpoint=\"/\"})) * 100";
                return await QueryPrometheusAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error fetching Disk: {ex.Message}");
                return 0;
            }
        }

        public async Task<string> GetUptimeAsync()
        {
            try
            {
                if (System.IO.File.Exists("/proc/uptime"))
                {
                    var content = await System.IO.File.ReadAllTextAsync("/proc/uptime");
                    var seconds = int.Parse(content.Split()[0].Split('.')[0]);
                    var uptime = TimeSpan.FromSeconds(seconds);
                    return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
                }
                return "Unknown";
            }
            catch
            {
                return "Unable to read";
            }
        }

        private async Task<double> QueryPrometheusAsync(string query)
        {
            try
            {
                var url = $"http://localhost:9090/api/v1/query?query={Uri.EscapeDataString(query)}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    var result = jsonDoc.RootElement.GetProperty("data").GetProperty("result");

                    if (result.GetArrayLength() > 0)
                    {
                        var value = result[0].GetProperty("value")[1].GetString();
                        if (double.TryParse(value, out var parsed))
                        {
                            return Math.Round(parsed, 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Prometheus query failed: {ex.Message}");
            }

            return 0;
        }
    }

    /// <summary>
    /// AlertService - Aggregates alerts from all monitoring tools
    /// </summary>
    public class AlertService : IAlertService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AlertService> _logger;

        public AlertService(HttpClient httpClient, ILogger<AlertService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<AlertInfo>> GetActiveAlertsAsync()
        {
            var alerts = new List<AlertInfo>();
            alerts.AddRange(await GetAlertsFromPrometheusAsync());
            alerts.AddRange(await GetAlertsFromNetdataAsync());
            return alerts.OrderByDescending(a => a.CreatedAt).ToList();
        }

        public async Task<List<AlertInfo>> GetAlertsFromPrometheusAsync()
        {
            var alerts = new List<AlertInfo>();
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:9090/api/v1/alerts");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    var alertsData = jsonDoc.RootElement.GetProperty("data").GetProperty("alerts");

                    foreach (var alert in alertsData.EnumerateArray())
                    {
                        var state = alert.GetProperty("state").GetString();
                        if (state == "firing")
                        {
                            alerts.Add(new AlertInfo
                            {
                                Id = Guid.NewGuid().ToString(),
                                Title = alert.GetProperty("labels").GetProperty("alertname").GetString() ?? "Unknown",
                                Description = alert.GetProperty("annotations").GetProperty("description").GetString() ?? "",
                                Severity = alert.GetProperty("labels").TryGetProperty("severity", out var sev) 
                                    ? sev.GetString() ?? "info" 
                                    : "warning",
                                Source = "prometheus",
                                CreatedAt = DateTime.UtcNow,
                                IsActive = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error fetching Prometheus alerts: {ex.Message}");
            }

            return alerts;
        }

        public async Task<List<AlertInfo>> GetAlertsFromNetdataAsync()
        {
            var alerts = new List<AlertInfo>();
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:19999/api/v1/alarms");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(content);
                    
                    if (jsonDoc.RootElement.TryGetProperty("alarms", out var alarmsProperty))
                    {
                        foreach (var alarm in alarmsProperty.EnumerateObject())
                        {
                            var status = alarm.Value.GetProperty("status").GetString();
                            if (status == "CRITICAL" || status == "WARNING")
                            {
                                alerts.Add(new AlertInfo
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Title = alarm.Name,
                                    Description = alarm.Value.GetProperty("info").GetString() ?? "",
                                    Severity = status?.ToLower() ?? "warning",
                                    Source = "netdata",
                                    CreatedAt = DateTime.UtcNow,
                                    IsActive = true
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error fetching Netdata alerts: {ex.Message}");
            }

            return alerts;
        }

        public async Task<int> GetAlertCountAsync()
        {
            var alerts = await GetActiveAlertsAsync();
            return alerts.Count;
        }
    }

    /// <summary>
    /// ProxyService - Proxies requests to monitoring tools
    /// </summary>
    public class ProxyService : IProxyService
    {
        private readonly HttpClient _httpClient;
        private readonly IMonitoringToolService _toolService;
        private readonly ILogger<ProxyService> _logger;

        public ProxyService(HttpClient httpClient, IMonitoringToolService toolService, ILogger<ProxyService> logger)
        {
            _httpClient = httpClient;
            _toolService = toolService;
            _logger = logger;
        }

        public async Task<string> ProxyRequestAsync(string toolId, string endpoint)
        {
            try
            {
                var tool = await _toolService.GetToolByIdAsync(toolId);
                if (tool == null)
                    return JsonSerializer.Serialize(new { error = "Tool not found" });

                var url = $"{tool.Url}{endpoint}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                return JsonSerializer.Serialize(new { error = "Request failed", statusCode = response.StatusCode });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Proxy error: {ex.Message}");
                return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }

        public async Task<string> GetToolDataAsync(string toolId, string dataType)
        {
            return dataType switch
            {
                "health" => await ProxyRequestAsync(toolId, "/health"),
                "status" => await ProxyRequestAsync(toolId, "/status"),
                "info" => await ProxyRequestAsync(toolId, "/api/v1/info"),
                _ => await ProxyRequestAsync(toolId, "")
            };
        }
    }

    #endregion
}
