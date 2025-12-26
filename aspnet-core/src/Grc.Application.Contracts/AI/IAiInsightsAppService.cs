using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Grc.AI;

public interface IAiInsightsAppService : IApplicationService
{
    Task<AiAnalysisResult> AnalyzeComplianceGapAsync(AnalyzeComplianceInput input);
    Task<AiAnalysisResult> GenerateRiskAssessmentAsync(RiskAssessmentInput input);
    Task<AiAnalysisResult> SuggestControlImplementationAsync(ControlImplementationInput input);
    Task<AiAnalysisResult> AnalyzeFrameworkAsync(FrameworkAnalysisInput input);
    Task<AiChatResponse> ChatAsync(AiChatInput input);
}

public class AnalyzeComplianceInput
{
    public string FrameworkCode { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public string[] ImplementedControls { get; set; } = [];
}

public class RiskAssessmentInput
{
    public string RiskDescription { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string ThreatScenario { get; set; } = string.Empty;
}

public class ControlImplementationInput
{
    public string ControlId { get; set; } = string.Empty;
    public string ControlTitle { get; set; } = string.Empty;
    public string ControlRequirement { get; set; } = string.Empty;
    public string OrganizationType { get; set; } = string.Empty;
}

public class FrameworkAnalysisInput
{
    public string FrameworkCode { get; set; } = string.Empty;
    public string FrameworkName { get; set; } = string.Empty;
    public int TotalControls { get; set; }
    public int ImplementedControls { get; set; }
}

public class AiChatInput
{
    public string Message { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Model { get; set; } = "qwen2.5:32b";
}

public class AiAnalysisResult
{
    public bool Success { get; set; }
    public string Analysis { get; set; } = string.Empty;
    public string[] Recommendations { get; set; } = [];
    public string[] ActionItems { get; set; } = [];
    public string RiskLevel { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string Model { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
}

public class AiChatResponse
{
    public bool Success { get; set; }
    public string Response { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}
