using System.Threading.Tasks;
using Grc.AI;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Grc.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/ai-insights")]
[IgnoreAntiforgeryToken]
public class AiInsightsController : GrcController
{
    private readonly IAiInsightsAppService _aiInsightsAppService;

    public AiInsightsController(IAiInsightsAppService aiInsightsAppService)
    {
        _aiInsightsAppService = aiInsightsAppService;
    }

    [HttpPost("analyze-compliance-gap")]
    public async Task<AiAnalysisResult> AnalyzeComplianceGapAsync(AnalyzeComplianceInput input)
    {
        return await _aiInsightsAppService.AnalyzeComplianceGapAsync(input);
    }

    [HttpPost("generate-risk-assessment")]
    public async Task<AiAnalysisResult> GenerateRiskAssessmentAsync(RiskAssessmentInput input)
    {
        return await _aiInsightsAppService.GenerateRiskAssessmentAsync(input);
    }

    [HttpPost("suggest-control-implementation")]
    public async Task<AiAnalysisResult> SuggestControlImplementationAsync(ControlImplementationInput input)
    {
        return await _aiInsightsAppService.SuggestControlImplementationAsync(input);
    }

    [HttpPost("analyze-framework")]
    public async Task<AiAnalysisResult> AnalyzeFrameworkAsync(FrameworkAnalysisInput input)
    {
        return await _aiInsightsAppService.AnalyzeFrameworkAsync(input);
    }

    [HttpPost("chat")]
    public async Task<AiChatResponse> ChatAsync(AiChatInput input)
    {
        return await _aiInsightsAppService.ChatAsync(input);
    }
}
