using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;

namespace Grc.AI;

[Authorize(GrcPermissions.AI.Default)]
public class AiInsightsAppService : ApplicationService, IAiInsightsAppService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _ollamaUrl;
    private readonly string _defaultModel;

    public AiInsightsAppService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _ollamaUrl = configuration["Ollama:Url"] ?? "http://localhost:11434";
        _defaultModel = configuration["Ollama:DefaultModel"] ?? "qwen2.5:32b";
    }

    [Authorize(GrcPermissions.AI.UseRecommendations)]
    public async Task<AiAnalysisResult> AnalyzeComplianceGapAsync(AnalyzeComplianceInput input)
    {
        var prompt = $@"You are a GRC (Governance, Risk, and Compliance) expert specializing in Saudi Arabian regulatory frameworks.

Analyze the compliance gap for the following:
- Framework: {input.FrameworkCode}
- Current Status: {input.CurrentStatus}
- Implemented Controls: {string.Join(", ", input.ImplementedControls)}

Provide:
1. Gap Analysis Summary
2. Priority areas requiring immediate attention
3. Specific recommendations for achieving compliance
4. Risk assessment of current gaps
5. Suggested timeline for remediation

Format your response as JSON with fields: analysis, recommendations (array), actionItems (array), riskLevel (Low/Medium/High/Critical)";

        return await CallOllamaAsync(prompt, _defaultModel);
    }

    [Authorize(GrcPermissions.AI.UseRecommendations)]
    public async Task<AiAnalysisResult> GenerateRiskAssessmentAsync(RiskAssessmentInput input)
    {
        var prompt = $@"You are a cybersecurity risk assessment expert.

Perform a risk assessment for:
- Risk Description: {input.RiskDescription}
- Asset Type: {input.AssetType}
- Threat Scenario: {input.ThreatScenario}

Provide:
1. Risk Score (1-10) with justification
2. Likelihood assessment
3. Impact assessment
4. Recommended controls and mitigations
5. Residual risk after controls

Format your response as JSON with fields: analysis, recommendations (array), actionItems (array), riskLevel (Low/Medium/High/Critical), confidenceScore (0-1)";

        return await CallOllamaAsync(prompt, _defaultModel);
    }

    [Authorize(GrcPermissions.AI.UseRecommendations)]
    public async Task<AiAnalysisResult> SuggestControlImplementationAsync(ControlImplementationInput input)
    {
        var prompt = $@"You are a compliance implementation specialist.

Provide implementation guidance for:
- Control ID: {input.ControlId}
- Control Title: {input.ControlTitle}
- Requirement: {input.ControlRequirement}
- Organization Type: {input.OrganizationType}

Provide:
1. Step-by-step implementation guide
2. Required resources and tools
3. Common pitfalls to avoid
4. Evidence collection recommendations
5. Estimated implementation effort

Format your response as JSON with fields: analysis, recommendations (array), actionItems (array), riskLevel";

        return await CallOllamaAsync(prompt, _defaultModel);
    }

    [Authorize(GrcPermissions.AI.UseRecommendations)]
    public async Task<AiAnalysisResult> AnalyzeFrameworkAsync(FrameworkAnalysisInput input)
    {
        var complianceRate = input.TotalControls > 0
            ? (double)input.ImplementedControls / input.TotalControls * 100
            : 0;

        var prompt = $@"You are a regulatory compliance analyst.

Analyze the framework implementation status:
- Framework: {input.FrameworkCode} ({input.FrameworkName})
- Total Controls: {input.TotalControls}
- Implemented Controls: {input.ImplementedControls}
- Current Compliance Rate: {complianceRate:F1}%

Provide:
1. Assessment of current compliance posture
2. Key areas of strength
3. Critical gaps that need attention
4. Industry benchmarks comparison
5. Roadmap to full compliance

Format your response as JSON with fields: analysis, recommendations (array), actionItems (array), riskLevel, confidenceScore";

        return await CallOllamaAsync(prompt, _defaultModel);
    }

    public async Task<AiChatResponse> ChatAsync(AiChatInput input)
    {
        var systemPrompt = @"You are an AI assistant specialized in GRC (Governance, Risk, and Compliance).
You help organizations with:
- Regulatory compliance (SAMA, NCA, ISO 27001, etc.)
- Risk management
- Control implementation
- Audit preparation
- Policy development

Be concise, practical, and provide actionable advice.";

        var fullPrompt = $"{systemPrompt}\n\nContext: {input.Context}\n\nUser Question: {input.Message}";
        var model = string.IsNullOrEmpty(input.Model) ? _defaultModel : input.Model;

        try
        {
            var client = _httpClientFactory.CreateClient();
            var request = new
            {
                model = model,
                prompt = fullPrompt,
                stream = false
            };

            var response = await client.PostAsync(
                $"{_ollamaUrl}/api/generate",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OllamaResponse>(content);

                return new AiChatResponse
                {
                    Success = true,
                    Response = result?.response ?? "No response",
                    Model = model
                };
            }

            return new AiChatResponse
            {
                Success = false,
                Response = "Failed to get response from AI",
                Model = model
            };
        }
        catch (Exception ex)
        {
            return new AiChatResponse
            {
                Success = false,
                Response = $"Error: {ex.Message}",
                Model = model
            };
        }
    }

    private async Task<AiAnalysisResult> CallOllamaAsync(string prompt, string model)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            var request = new
            {
                model = model,
                prompt = prompt,
                stream = false
            };

            var response = await client.PostAsync(
                $"{_ollamaUrl}/api/generate",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ollamaResult = JsonSerializer.Deserialize<OllamaResponse>(content);

                // Try to parse JSON from response
                var analysisText = ollamaResult?.response ?? "";

                // Extract JSON if present
                var jsonStart = analysisText.IndexOf('{');
                var jsonEnd = analysisText.LastIndexOf('}');

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    try
                    {
                        var jsonPart = analysisText.Substring(jsonStart, jsonEnd - jsonStart + 1);
                        var parsed = JsonSerializer.Deserialize<ParsedAnalysis>(jsonPart, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new AiAnalysisResult
                        {
                            Success = true,
                            Analysis = parsed?.Analysis ?? analysisText,
                            Recommendations = parsed?.Recommendations ?? [],
                            ActionItems = parsed?.ActionItems ?? [],
                            RiskLevel = parsed?.RiskLevel ?? "Medium",
                            ConfidenceScore = parsed?.ConfidenceScore ?? 0.8,
                            Model = model,
                            TokensUsed = ollamaResult?.eval_count ?? 0
                        };
                    }
                    catch
                    {
                        // Fall back to plain text
                    }
                }

                return new AiAnalysisResult
                {
                    Success = true,
                    Analysis = analysisText,
                    Recommendations = [],
                    ActionItems = [],
                    RiskLevel = "Medium",
                    ConfidenceScore = 0.7,
                    Model = model,
                    TokensUsed = ollamaResult?.eval_count ?? 0
                };
            }

            return new AiAnalysisResult
            {
                Success = false,
                Analysis = "Failed to get AI response",
                Model = model
            };
        }
        catch (Exception ex)
        {
            return new AiAnalysisResult
            {
                Success = false,
                Analysis = $"Error: {ex.Message}",
                Model = model
            };
        }
    }

    private class OllamaResponse
    {
        public string response { get; set; } = string.Empty;
        public int eval_count { get; set; }
    }

    private class ParsedAnalysis
    {
        public string Analysis { get; set; } = string.Empty;
        public string[] Recommendations { get; set; } = [];
        public string[] ActionItems { get; set; } = [];
        public string RiskLevel { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
    }
}
