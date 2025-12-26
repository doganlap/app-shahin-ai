using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Grc.Integration;

/// <summary>
/// Client service for communicating with n8n workflow automation
/// </summary>
public interface IN8nClientService
{
    Task<N8nWorkflowResponse> TriggerWorkflowAsync(string webhookPath, object payload, int timeoutSeconds = 300);
    Task<N8nWorkflowResponse> TriggerComplianceCheckAsync(string request, string context = null);
    Task<N8nWorkflowResponse> TriggerRiskAnalysisAsync(string riskDescription, string assetType);
    Task<bool> IsN8nAvailableAsync();
}

public class N8nClientService : IN8nClientService, ITransientDependency
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<N8nClientService> _logger;
    private readonly string _n8nUrl;
    private readonly string _webhookUrl;

    public N8nClientService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<N8nClientService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _n8nUrl = configuration["N8n:Url"] ?? "http://localhost:5678";
        _webhookUrl = configuration["N8n:WebhookUrl"] ?? "http://localhost:5678/webhook";
    }

    public async Task<bool> IsN8nAvailableAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("N8n");
            client.Timeout = TimeSpan.FromSeconds(5);
            var response = await client.GetAsync($"{_n8nUrl}/healthz");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "n8n health check failed");
            return false;
        }
    }

    public async Task<N8nWorkflowResponse> TriggerWorkflowAsync(string webhookPath, object payload, int timeoutSeconds = 300)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("N8n");
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            var url = $"{_webhookUrl}/{webhookPath.TrimStart('/')}";
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            _logger.LogInformation("Triggering n8n workflow at {Url}", url);

            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("n8n workflow completed successfully");

                try
                {
                    var result = JsonSerializer.Deserialize<N8nWorkflowResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result ?? new N8nWorkflowResponse { Success = true, RawResponse = responseContent };
                }
                catch
                {
                    return new N8nWorkflowResponse
                    {
                        Success = true,
                        RawResponse = responseContent
                    };
                }
            }

            _logger.LogWarning("n8n workflow failed with status {StatusCode}: {Response}",
                response.StatusCode, responseContent);

            return new N8nWorkflowResponse
            {
                Success = false,
                Error = $"HTTP {response.StatusCode}: {responseContent}"
            };
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("n8n workflow timed out after {Timeout} seconds", timeoutSeconds);
            return new N8nWorkflowResponse
            {
                Success = false,
                Error = $"Workflow timed out after {timeoutSeconds} seconds"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering n8n workflow {WebhookPath}", webhookPath);
            return new N8nWorkflowResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<N8nWorkflowResponse> TriggerComplianceCheckAsync(string request, string context = null)
    {
        var payload = new
        {
            request = request,
            context = context ?? "GRC Compliance Analysis",
            timestamp = DateTime.UtcNow,
            source = "GRC-Platform"
        };

        return await TriggerWorkflowAsync("grc-compliance-check", payload);
    }

    public async Task<N8nWorkflowResponse> TriggerRiskAnalysisAsync(string riskDescription, string assetType)
    {
        var payload = new
        {
            riskDescription = riskDescription,
            assetType = assetType,
            timestamp = DateTime.UtcNow,
            source = "GRC-Platform"
        };

        return await TriggerWorkflowAsync("grc-risk-analysis", payload);
    }
}

public class N8nWorkflowResponse
{
    public bool Success { get; set; }
    public string Analysis { get; set; }
    public string Model { get; set; }
    public string Error { get; set; }
    public string RawResponse { get; set; }
    public string[] Recommendations { get; set; }
    public string[] ActionItems { get; set; }
}
