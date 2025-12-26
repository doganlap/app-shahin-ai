using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Grc.AI.Services;

/// <summary>
/// OpenAI GPT-4 Integration Service
/// Integrates with OpenAI API for AI-powered compliance analysis
/// </summary>
public class OpenAIIntegrationService : IAIIntegrationService, ITransientDependency
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAIIntegrationService> _logger;
    
    private string ApiKey => _configuration["AI:OpenAI:ApiKey"] ?? throw new BusinessException("OpenAI API key not configured");
    private string ApiEndpoint => _configuration["AI:OpenAI:Endpoint"] ?? "https://api.openai.com/v1/chat/completions";
    
    public string ModelName => "GPT-4";
    public string ModelVersion => "1.0";
    
    public OpenAIIntegrationService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OpenAIIntegrationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }
    
    /// <summary>
    /// تحليل الفجوات - Analyze compliance gaps
    /// </summary>
    public async Task<string> AnalyzeComplianceGapsAsync(string prompt)
    {
        _logger.LogInformation("Analyzing compliance gaps with GPT-4");
        
        var systemPrompt = @"You are a GRC (Governance, Risk, Compliance) expert specializing in Saudi Arabian regulatory frameworks including NCA ECC, SAMA CSF, CITC, NDMO, SDAIA, and MOH HIS. 

Your task is to analyze compliance assessments and identify gaps. Provide responses in both English and Arabic.

For each gap, specify:
- Control number and title
- Current compliance level (0-100%)
- Target compliance level
- Gap size (difference)
- Priority (Critical, High, Medium, Low)
- Estimated effort in hours
- Risk if not addressed
- Confidence level (0-100%)

Format your response as structured data that can be parsed.";
        
        return await CallOpenAIAsync(systemPrompt, prompt);
    }
    
    /// <summary>
    /// توليد التوصيات - Generate recommendations
    /// </summary>
    public async Task<string> GenerateRecommendationsAsync(string prompt)
    {
        _logger.LogInformation("Generating recommendations with GPT-4");
        
        var systemPrompt = @"You are a GRC compliance consultant providing actionable recommendations for Saudi Arabian organizations.

Generate detailed, bilingual (English/Arabic) recommendations including:
- Clear title
- Detailed recommendation
- Priority level
- Category (Technical, Process, People, Documentation)
- Implementation steps (bilingual)
- Expected impact
- Estimated time (days)
- Estimated cost (SAR)
- Confidence level (0-100%)
- Reasoning/justification

Be specific, practical, and aligned with Saudi regulatory requirements.";
        
        return await CallOpenAIAsync(systemPrompt, prompt);
    }
    
    /// <summary>
    /// توليد تقرير امتثال - Generate compliance report
    /// </summary>
    public async Task<string> GenerateComplianceReportAsync(string prompt)
    {
        _logger.LogInformation("Generating compliance report with GPT-4");
        
        var systemPrompt = @"You are an expert compliance auditor generating comprehensive compliance reports for Saudi Arabian organizations.

Generate bilingual (English/Arabic) reports including:
- Executive summary
- Overall compliance score
- Domain-level breakdown
- Key findings with priorities
- Risk assessment
- Actionable recommendations
- Trend analysis (if previous data available)

Use professional, formal language suitable for board presentations and regulatory submissions.";
        
        return await CallOpenAIAsync(systemPrompt, prompt);
    }
    
    /// <summary>
    /// تحليل المخاطر - Analyze risks
    /// </summary>
    public async Task<string> AnalyzeRisksAsync(string prompt)
    {
        _logger.LogInformation("Analyzing risks with GPT-4");
        
        var systemPrompt = @"You are a cybersecurity and compliance risk analyst specializing in Saudi Arabian regulatory frameworks.

Analyze risks and provide:
- Risk identification
- Likelihood (1-5)
- Impact (1-5)
- Risk score (likelihood × impact)
- Risk level (Critical, High, Medium, Low)
- Mitigation strategies
- Residual risk assessment

Responses should be bilingual (English/Arabic) and aligned with Saudi risk management standards.";
        
        return await CallOpenAIAsync(systemPrompt, prompt);
    }
    
    private async Task<string> CallOpenAIAsync(string systemPrompt, string userPrompt)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
            
            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.7,
                max_tokens = 4000
            };
            
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("Calling OpenAI API: {Endpoint}", ApiEndpoint);
            
            var response = await httpClient.PostAsync(ApiEndpoint, content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);
            
            var aiResponse = result?.choices?[0]?.message?.content ?? string.Empty;
            
            _logger.LogInformation("OpenAI API call successful. Response length: {Length}", aiResponse.Length);
            
            return aiResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling OpenAI API");
            throw new BusinessException($"Failed to call OpenAI API: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenAI API");
            throw new BusinessException($"AI processing error: {ex.Message}");
        }
    }
    
    private class OpenAIResponse
    {
        public Choice[]? choices { get; set; }
    }
    
    private class Choice
    {
        public Message? message { get; set; }
    }
    
    private class Message
    {
        public string? content { get; set; }
    }
}
