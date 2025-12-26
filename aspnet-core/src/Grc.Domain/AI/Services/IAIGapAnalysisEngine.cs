using System;
using System.Threading.Tasks;

namespace Grc.AI.Services;

/// <summary>
/// AI Gap Analysis Engine Interface
/// Core service that performs AI-powered gap analysis
/// </summary>
public interface IAIGapAnalysisEngine
{
    /// <summary>
    /// معالجة تحليل الفجوات - Process gap analysis using AI
    /// </summary>
    /// <param name="analysisId">Gap analysis ID to process</param>
    Task ProcessGapAnalysisAsync(Guid analysisId);
    
    /// <summary>
    /// تحليل التقييم - Analyze an assessment for gaps
    /// </summary>
    Task<AIGapAnalysisResult> AnalyzeAssessmentAsync(Guid assessmentId);
    
    /// <summary>
    /// تحليل الإطار - Analyze a framework for compliance gaps
    /// </summary>
    Task<AIGapAnalysisResult> AnalyzeFrameworkAsync(Guid frameworkId);
    
    /// <summary>
    /// توليد توصيات - Generate AI recommendations
    /// </summary>
    Task<AIRecommendations> GenerateRecommendationsAsync(Guid analysisId);
}

/// <summary>
/// AI Gap Analysis Result
/// </summary>
public class AIGapAnalysisResult
{
    public decimal CompliancePercentage { get; set; }
    public decimal ConfidenceScore { get; set; }
    public int ProcessingTimeSeconds { get; set; }
    public int ControlsAnalyzed { get; set; }
    public GapDetail[] Gaps { get; set; } = Array.Empty<GapDetail>();
}

/// <summary>
/// AI Recommendations Result
/// </summary>
public class AIRecommendations
{
    public AIRecommendation[] Recommendations { get; set; } = Array.Empty<AIRecommendation>();
    public decimal ConfidenceScore { get; set; }
}
