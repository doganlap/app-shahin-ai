using Microsoft.AspNetCore.Mvc.RazorPages;
using NocHub.Models;
using NocHub.Services;

namespace NocHub.Pages;

public class IndexModel : PageModel
{
    private readonly IMonitoringToolService _toolService;
    private readonly IMetricsService _metricsService;
    private readonly IAlertService _alertService;
    private readonly ILogger<IndexModel> _logger;

    public NocDashboardViewModel Dashboard { get; set; } = new();

    public IndexModel(IMonitoringToolService toolService, IMetricsService metricsService,
        IAlertService alertService, ILogger<IndexModel> logger)
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
            Dashboard.ServerName = Environment.MachineName;
            Dashboard.Location = "Primary NOC";
            Dashboard.DashboardRefreshedAt = DateTime.UtcNow;

            // Load tools configuration
            Dashboard.Tools = await _toolService.GetAllToolsAsync();

            // Load tool statuses
            Dashboard.ToolStatuses = await _toolService.GetAllToolsStatusAsync();
            Dashboard.HealthyToolsCount = Dashboard.ToolStatuses.Count(t => t.IsHealthy);
            Dashboard.TotalToolsCount = Dashboard.ToolStatuses.Count;

            // Load system metrics
            Dashboard.SystemMetrics = await _metricsService.GetSystemMetricsAsync();

            // Load active alerts
            Dashboard.ActiveAlerts = await _alertService.GetActiveAlertsAsync();
            Dashboard.ActiveAlertsCount = Dashboard.ActiveAlerts.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading dashboard: {ex.Message}");
        }
    }
}
