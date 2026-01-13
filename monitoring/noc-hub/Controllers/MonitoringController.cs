using Microsoft.AspNetCore.Mvc;
using NocHub.Models;
using NocHub.Services;

namespace NocHub.Controllers;

/// <summary>
/// API Controller for monitoring operations
/// Provides endpoints for retrieving monitoring data, health status, and metrics
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MonitoringController : ControllerBase
{
    private readonly IMonitoringToolService _toolService;
    private readonly IMetricsService _metricsService;
    private readonly IAlertService _alertService;
    private readonly IProxyService _proxyService;
    private readonly ILogger<MonitoringController> _logger;

    public MonitoringController(
        IMonitoringToolService toolService,
        IMetricsService metricsService,
        IAlertService alertService,
        IProxyService proxyService,
        ILogger<MonitoringController> logger)
    {
        _toolService = toolService;
        _metricsService = metricsService;
        _alertService = alertService;
        _proxyService = proxyService;
        _logger = logger;
    }

    /// <summary>
    /// Get all monitoring tools configuration
    /// </summary>
    [HttpGet("tools")]
    [ProducesResponseType(typeof(List<MonitoringToolConfig>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MonitoringToolConfig>>> GetTools()
    {
        try
        {
            var tools = await _toolService.GetAllToolsAsync();
            return Ok(tools);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching tools: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get health status of all monitoring tools
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(List<ToolStatus>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ToolStatus>>> GetStatus()
    {
        try
        {
            var status = await _toolService.GetAllToolsStatusAsync();
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching status: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get health status of a specific tool
    /// </summary>
    [HttpGet("status/{toolId}")]
    [ProducesResponseType(typeof(ToolStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToolStatus>> GetToolStatus(string toolId)
    {
        try
        {
            var status = await _toolService.GetToolStatusAsync(toolId);
            if (status == null)
                return NotFound();
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching tool status: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get current system metrics
    /// </summary>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(SystemMetrics), StatusCodes.Status200OK)]
    public async Task<ActionResult<SystemMetrics>> GetMetrics()
    {
        try
        {
            var metrics = await _metricsService.GetSystemMetricsAsync();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching metrics: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get system CPU usage percentage
    /// </summary>
    [HttpGet("metrics/cpu")]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    public async Task<ActionResult<double>> GetCpuUsage()
    {
        try
        {
            var cpu = await _metricsService.GetCpuUsageAsync();
            return Ok(cpu);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching CPU: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get system memory usage percentage
    /// </summary>
    [HttpGet("metrics/memory")]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    public async Task<ActionResult<double>> GetMemoryUsage()
    {
        try
        {
            var memory = await _metricsService.GetMemoryUsageAsync();
            return Ok(memory);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching memory: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get system disk usage percentage
    /// </summary>
    [HttpGet("metrics/disk")]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    public async Task<ActionResult<double>> GetDiskUsage()
    {
        try
        {
            var disk = await _metricsService.GetDiskUsageAsync();
            return Ok(disk);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching disk: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get system uptime
    /// </summary>
    [HttpGet("metrics/uptime")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> GetUptime()
    {
        try
        {
            var uptime = await _metricsService.GetUptimeAsync();
            return Ok(uptime);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching uptime: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all active alerts from monitoring tools
    /// </summary>
    [HttpGet("alerts")]
    [ProducesResponseType(typeof(List<AlertInfo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AlertInfo>>> GetAlerts()
    {
        try
        {
            var alerts = await _alertService.GetActiveAlertsAsync();
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching alerts: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get count of active alerts
    /// </summary>
    [HttpGet("alerts/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetAlertCount()
    {
        try
        {
            var count = await _alertService.GetAlertCountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching alert count: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Proxy request to a monitoring tool
    /// </summary>
    [HttpPost("proxy/{toolId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> ProxyRequest(string toolId, [FromQuery] string endpoint = "")
    {
        try
        {
            var result = await _proxyService.ProxyRequestAsync(toolId, endpoint);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Proxy error: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
