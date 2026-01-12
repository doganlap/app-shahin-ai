using Microsoft.AspNetCore.Mvc.RazorPages;
using NocHub.Models;
using NocHub.Services;

namespace NocHub.Pages;

public class DashboardModel : PageModel
{
    private readonly IMonitoringToolService _toolService;
    private readonly IMetricsService _metricsService;
    private readonly IAlertService _alertService;
    private readonly ILogger<DashboardModel> _logger;

    public NocDashboardViewModel Dashboard { get; set; } = new();
    public string ServerName => Environment.MachineName;

    public DashboardModel(IMonitoringToolService toolService, IMetricsService metricsService,
        IAlertService alertService, ILogger<DashboardModel> logger)
    {
        _toolService = toolService;
        _metricsService = metricsService;
        _alertService = alertService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Dashboard.ServerName = ServerName;
            Dashboard.Location = "Primary NOC";
            Dashboard.DashboardRefreshedAt = DateTime.UtcNow;

            // Load tools
            Dashboard.Tools = await _toolService.GetAllToolsAsync();

            // Load statuses
            Dashboard.ToolStatuses = await _toolService.GetAllToolsStatusAsync();
            Dashboard.HealthyToolsCount = Dashboard.ToolStatuses.Count(t => t.IsHealthy);
            Dashboard.TotalToolsCount = Dashboard.ToolStatuses.Count;

            // Load metrics
            Dashboard.SystemMetrics = await _metricsService.GetSystemMetricsAsync();

            // Load alerts
            Dashboard.ActiveAlerts = await _alertService.GetActiveAlertsAsync();
            Dashboard.ActiveAlertsCount = Dashboard.ActiveAlerts.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading dashboard: {ex.Message}");
        }
    }
}
