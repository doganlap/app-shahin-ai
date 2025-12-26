using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Grc.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Grc.Integration;

/// <summary>
/// Unified AI orchestration service that routes requests through n8n workflows
/// or directly to Ollama based on availability and configuration
/// </summary>
public interface IAiOrchestrationService
{
    Task<AiOrchestrationResult> ExecuteAsync(AiOrchestrationRequest request);
    Task<AiOrchestrationResult> AnalyzeComplianceAsync(string frameworkCode, string requirement, string currentState);
    Task<AiOrchestrationResult> AssessRiskAsync(string riskDescription, string assetType, string threatScenario);
    Task<AiOrchestrationResult> GenerateControlGuidanceAsync(string controlId, string controlTitle, string requirement);
    Task<AiOrchestrationResult> ChatAsync(string message, string context = null);
    Task<OrchestratorStatus> GetStatusAsync();
}

public class AiOrchestrationService : IAiOrchestrationService, ITransientDependency
{
    private readonly IN8nClientService _n8nClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AiOrchestrationService> _logger;
    private readonly string _ollamaUrl;
    private readonly string _defaultModel;
    private readonly string _grcModel;
    private readonly bool _preferN8n;

    public AiOrchestrationService(
        IN8nClientService n8nClient,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AiOrchestrationService> logger)
    {
        _n8nClient = n8nClient;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _ollamaUrl = configuration["Ollama:Url"] ?? "http://localhost:11434";
        _defaultModel = configuration["Ollama:DefaultModel"] ?? "qwen2.5:32b";
        _grcModel = configuration["Ollama:GrcModel"] ?? "grc-specialist:latest";
        _preferN8n = configuration.GetValue<bool>("N8n:PreferN8nForWorkflows", true);
    }

    public async Task<OrchestratorStatus> GetStatusAsync()
    {
        var n8nAvailable = await _n8nClient.IsN8nAvailableAsync();
        var ollamaAvailable = await CheckOllamaAsync();

        return new OrchestratorStatus
        {
            N8nAvailable = n8nAvailable,
            OllamaAvailable = ollamaAvailable,
            PreferN8n = _preferN8n,
            DefaultModel = _defaultModel,
            GrcModel = _grcModel,
            OllamaUrl = _ollamaUrl
        };
    }

    public async Task<AiOrchestrationResult> ExecuteAsync(AiOrchestrationRequest request)
    {
        var useN8n = _preferN8n && !string.IsNullOrEmpty(request.N8nWebhookPath);

        if (useN8n)
        {
            var n8nAvailable = await _n8nClient.IsN8nAvailableAsync();
            if (n8nAvailable)
            {
                _logger.LogInformation("Routing AI request through n8n workflow: {WebhookPath}", request.N8nWebhookPath);
                return await ExecuteViaN8nAsync(request);
            }
            _logger.LogWarning("n8n not available, falling back to direct Ollama");
        }

        return await ExecuteViaOllamaAsync(request);
    }

    public async Task<AiOrchestrationResult> AnalyzeComplianceAsync(string frameworkCode, string requirement, string currentState)
    {
        var prompt = $@"You are a GRC compliance expert specializing in Saudi Arabian regulatory frameworks.

Analyze the compliance status for:
- Framework: {frameworkCode}
- Requirement: {requirement}
- Current State: {currentState}

Provide:
1. Compliance gap analysis
2. Risk level assessment (Low/Medium/High/Critical)
3. Specific recommendations
4. Priority action items
5. Timeline considerations

Respond in JSON format with fields: analysis, riskLevel, recommendations (array), actionItems (array), confidenceScore (0-1)";

        var request = new AiOrchestrationRequest
        {
            Prompt = prompt,
            Model = _grcModel,
            N8nWebhookPath = "grc-compliance-check",
            Context = new { frameworkCode, requirement, currentState },
            TimeoutSeconds = 300
        };

        return await ExecuteAsync(request);
    }

    public async Task<AiOrchestrationResult> AssessRiskAsync(string riskDescription, string assetType, string threatScenario)
    {
        var prompt = $@"You are a cybersecurity risk assessment expert.

Perform a comprehensive risk assessment for:
- Risk: {riskDescription}
- Asset Type: {assetType}
- Threat Scenario: {threatScenario}

Provide:
1. Risk score (1-10) with justification
2. Likelihood assessment
3. Impact assessment (financial, operational, reputational)
4. Recommended controls and mitigations
5. Residual risk estimation

Respond in JSON format with fields: analysis, riskScore (1-10), riskLevel, likelihood, impact, recommendations (array), actionItems (array), confidenceScore (0-1)";

        var request = new AiOrchestrationRequest
        {
            Prompt = prompt,
            Model = _grcModel,
            N8nWebhookPath = "grc-risk-analysis",
            Context = new { riskDescription, assetType, threatScenario },
            TimeoutSeconds = 300
        };

        return await ExecuteAsync(request);
    }

    public async Task<AiOrchestrationResult> GenerateControlGuidanceAsync(string controlId, string controlTitle, string requirement)
    {
        var prompt = $@"You are a compliance implementation specialist.

Provide implementation guidance for:
- Control ID: {controlId}
- Control Title: {controlTitle}
- Requirement: {requirement}

Provide:
1. Step-by-step implementation guide
2. Required resources and tools
3. Common implementation pitfalls
4. Evidence collection recommendations
5. Testing and validation steps

Respond in JSON format with fields: analysis, implementationSteps (array), resources (array), pitfalls (array), evidenceTypes (array), actionItems (array)";

        var request = new AiOrchestrationRequest
        {
            Prompt = prompt,
            Model = _grcModel,
            N8nWebhookPath = "grc-control-guidance",
            Context = new { controlId, controlTitle, requirement },
            TimeoutSeconds = 300
        };

        return await ExecuteAsync(request);
    }

    public async Task<AiOrchestrationResult> ChatAsync(string message, string context = null)
    {
        var systemPrompt = @"You are an AI assistant specialized in GRC (Governance, Risk, and Compliance).
You help organizations with regulatory compliance (SAMA, NCA, ISO 27001), risk management, control implementation, and audit preparation.
Be concise, practical, and provide actionable advice.";

        var fullPrompt = context != null
            ? $"{systemPrompt}\n\nContext: {context}\n\nUser: {message}"
            : $"{systemPrompt}\n\nUser: {message}";

        var request = new AiOrchestrationRequest
        {
            Prompt = fullPrompt,
            Model = _defaultModel,
            TimeoutSeconds = 120
        };

        return await ExecuteViaOllamaAsync(request);
    }

    private async Task<AiOrchestrationResult> ExecuteViaN8nAsync(AiOrchestrationRequest request)
    {
        var payload = new
        {
            request = request.Prompt,
            context = request.Context,
            model = request.Model,
            timestamp = DateTime.UtcNow
        };

        var response = await _n8nClient.TriggerWorkflowAsync(
            request.N8nWebhookPath,
            payload,
            request.TimeoutSeconds
        );

        if (response.Success)
        {
            return new AiOrchestrationResult
            {
                Success = true,
                Response = response.Analysis ?? response.RawResponse,
                Model = response.Model ?? request.Model,
                Source = "n8n",
                Recommendations = response.Recommendations,
                ActionItems = response.ActionItems
            };
        }

        _logger.LogWarning("n8n workflow failed, falling back to direct Ollama: {Error}", response.Error);
        return await ExecuteViaOllamaAsync(request);
    }

    private async Task<AiOrchestrationResult> ExecuteViaOllamaAsync(AiOrchestrationRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Ollama");
            client.Timeout = TimeSpan.FromSeconds(request.TimeoutSeconds);

            var ollamaRequest = new
            {
                model = request.Model ?? _defaultModel,
                prompt = request.Prompt,
                stream = false
            };

            var response = await client.PostAsync(
                $"{_ollamaUrl}/api/generate",
                new StringContent(JsonSerializer.Serialize(ollamaRequest), Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(content);

                var result = new AiOrchestrationResult
                {
                    Success = true,
                    Response = ollamaResponse?.response ?? content,
                    Model = request.Model ?? _defaultModel,
                    Source = "ollama",
                    TokensUsed = ollamaResponse?.eval_count ?? 0
                };

                TryParseJsonResponse(result);
                return result;
            }

            return new AiOrchestrationResult
            {
                Success = false,
                Error = $"Ollama request failed: HTTP {response.StatusCode}",
                Source = "ollama"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Ollama");
            return new AiOrchestrationResult
            {
                Success = false,
                Error = ex.Message,
                Source = "ollama"
            };
        }
    }

    private void TryParseJsonResponse(AiOrchestrationResult result)
    {
        if (string.IsNullOrEmpty(result.Response)) return;

        var jsonStart = result.Response.IndexOf('{');
        var jsonEnd = result.Response.LastIndexOf('}');

        if (jsonStart >= 0 && jsonEnd > jsonStart)
        {
            try
            {
                var jsonPart = result.Response.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var parsed = JsonSerializer.Deserialize<ParsedAiResponse>(jsonPart, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed != null)
                {
                    result.Analysis = parsed.Analysis;
                    result.RiskLevel = parsed.RiskLevel;
                    result.RiskScore = parsed.RiskScore;
                    result.Recommendations = parsed.Recommendations;
                    result.ActionItems = parsed.ActionItems;
                    result.ConfidenceScore = parsed.ConfidenceScore;
                }
            }
            catch
            {
                // Keep raw response
            }
        }
    }

    private async Task<bool> CheckOllamaAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Ollama");
            client.Timeout = TimeSpan.FromSeconds(5);
            var response = await client.GetAsync($"{_ollamaUrl}/api/tags");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private class OllamaResponse
    {
        public string response { get; set; }
        public int eval_count { get; set; }
    }

    private class ParsedAiResponse
    {
        public string Analysis { get; set; }
        public string RiskLevel { get; set; }
        public int? RiskScore { get; set; }
        public string[] Recommendations { get; set; }
        public string[] ActionItems { get; set; }
        public double? ConfidenceScore { get; set; }
    }
}

public class AiOrchestrationRequest
{
    public string Prompt { get; set; }
    public string Model { get; set; }
    public string N8nWebhookPath { get; set; }
    public object Context { get; set; }
    public int TimeoutSeconds { get; set; } = 300;
}

public class AiOrchestrationResult
{
    public bool Success { get; set; }
    public string Response { get; set; }
    public string Analysis { get; set; }
    public string Model { get; set; }
    public string Source { get; set; } // "n8n" or "ollama"
    public string Error { get; set; }
    public string RiskLevel { get; set; }
    public int? RiskScore { get; set; }
    public string[] Recommendations { get; set; }
    public string[] ActionItems { get; set; }
    public double? ConfidenceScore { get; set; }
    public int TokensUsed { get; set; }
}

public class OrchestratorStatus
{
    public bool N8nAvailable { get; set; }
    public bool OllamaAvailable { get; set; }
    public bool PreferN8n { get; set; }
    public string DefaultModel { get; set; }
    public string GrcModel { get; set; }
    public string OllamaUrl { get; set; }
}
