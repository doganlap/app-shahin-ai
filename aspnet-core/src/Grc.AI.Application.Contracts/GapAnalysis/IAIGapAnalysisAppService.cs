using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Grc.Domain.Shared.AI;

namespace Grc.AI.GapAnalysis;

/// <summary>
/// AI Gap Analysis Application Service Interface
/// </summary>
public interface IAIGapAnalysisAppService : IApplicationService
{
    /// <summary>
    /// بدء تحليل الفجوات - Start gap analysis
    /// </summary>
    Task<AIGapAnalysisDto> StartGapAnalysisAsync(StartGapAnalysisInput input);
    
    /// <summary>
    /// الحصول على تحليل - Get analysis by ID
    /// </summary>
    Task<AIGapAnalysisDto> GetAsync(Guid id);
    
    /// <summary>
    /// الحصول على قائمة التحاليل - Get list of analyses
    /// </summary>
    Task<List<AIGapAnalysisDto>> GetListAsync(Guid? assessmentId = null, AIAnalysisStatus? status = null);
    
    /// <summary>
    /// الحصول على التوصيات - Get recommendations by priority
    /// </summary>
    Task<List<AIRecommendationDto>> GetRecommendationsAsync(Guid analysisId, RecommendationPriority? priority = null);
    
    /// <summary>
    /// الموافقة على التحليل - Approve analysis
    /// </summary>
    Task ApproveAsync(Guid id, ApproveGapAnalysisInput input);
    
    /// <summary>
    /// رفض التحليل - Reject analysis
    /// </summary>
    Task RejectAsync(Guid id, RejectGapAnalysisInput input);
    
    /// <summary>
    /// إلغاء التحليل - Cancel analysis
    /// </summary>
    Task CancelAsync(Guid id);
    
    /// <summary>
    /// حذف التحليل - Delete analysis
    /// </summary>
    Task DeleteAsync(Guid id);
}
