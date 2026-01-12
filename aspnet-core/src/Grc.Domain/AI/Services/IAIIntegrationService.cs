using System.Threading.Tasks;

namespace Grc.AI.Services;

/// <summary>
/// AI Integration Service Interface
/// Integrates with external AI providers (GPT-4, Claude, etc.)
/// </summary>
public interface IAIIntegrationService
{
    /// <summary>
    /// AI Model name (e.g., "GPT-4", "Claude-3")
    /// </summary>
    string ModelName { get; }
    
    /// <summary>
    /// Model version
    /// </summary>
    string ModelVersion { get; }
    
    /// <summary>
    /// تحليل الفجوات - Analyze compliance gaps
    /// </summary>
    Task<string> AnalyzeComplianceGapsAsync(string prompt);
    
    /// <summary>
    /// توليد التوصيات - Generate recommendations
    /// </summary>
    Task<string> GenerateRecommendationsAsync(string prompt);
    
    /// <summary>
    /// توليد تقرير امتثال - Generate compliance report
    /// </summary>
    Task<string> GenerateComplianceReportAsync(string prompt);
    
    /// <summary>
    /// تحليل المخاطر - Analyze risks
    /// </summary>
    Task<string> AnalyzeRisksAsync(string prompt);
}
