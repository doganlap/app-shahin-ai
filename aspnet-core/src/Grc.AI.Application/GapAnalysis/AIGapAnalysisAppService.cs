using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Grc.AI.GapAnalysis;
using Grc.Domain.Shared.AI;
using Grc.AI.Services;

namespace Grc.AI.Application;

/// <summary>
/// AI Gap Analysis Application Service
/// Handles gap analysis operations with AI integration
/// </summary>
public class AIGapAnalysisAppService : ApplicationService, IAIGapAnalysisAppService
{
    private readonly IRepository<AIGapAnalysis, Guid> _gapAnalysisRepository;
    // TODO: Restore after migration - temporarily commented to allow DB migration
    // private readonly IAIGapAnalysisEngine _aiEngine;
    private readonly ILogger<AIGapAnalysisAppService> _logger;
    
    public AIGapAnalysisAppService(
        IRepository<AIGapAnalysis, Guid> gapAnalysisRepository,
        // TODO: Restore after migration
        // IAIGapAnalysisEngine aiEngine,
        ILogger<AIGapAnalysisAppService> logger)
    {
        _gapAnalysisRepository = gapAnalysisRepository;
        // _aiEngine = aiEngine;
        _logger = logger;
    }
    
    /// <summary>
    /// بدء تحليل الفجوات - Start gap analysis
    /// </summary>
    [UnitOfWork]
    public virtual async Task<AIGapAnalysisDto> StartGapAnalysisAsync(StartGapAnalysisInput input)
    {
        _logger.LogInformation(
            "Starting gap analysis for Assessment: {AssessmentId}, Framework: {FrameworkId}",
            input.AssessmentId,
            input.FrameworkId
        );
        
        // Validate input
        if (input.AssessmentId == null && input.FrameworkId == null)
        {
            throw new BusinessException("Either AssessmentId or FrameworkId must be provided");
        }
        
        // Create analysis entity
        var analysis = new AIGapAnalysis(
            GuidGenerator.Create(),
            input.AnalysisType,
            input.Title,
            input.Description,
            input.AssessmentId,
            input.FrameworkId,
            CurrentTenant.Id
        );
        
        // Save to database
        await _gapAnalysisRepository.InsertAsync(analysis, autoSave: true);
        
        _logger.LogInformation("Created gap analysis entity: {AnalysisId}", analysis.Id);
        
        // Start AI processing in background (fire and forget)
        _ = Task.Run(async () =>
        {
            try
            {
                // TODO: Restore after migration - call AI engine to process analysis
                // await _aiEngine.ProcessGapAnalysisAsync(analysis.Id);
                
                // Temporary placeholder - mark as manual for now
                analysis.MarkAsCompleted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process gap analysis: {AnalysisId}", analysis.Id);
            }
        });
        
        return ObjectMapper.Map<AIGapAnalysis, AIGapAnalysisDto>(analysis);
    }
    
    /// <summary>
    /// الحصول على تحليل - Get analysis by ID
    /// </summary>
    public virtual async Task<AIGapAnalysisDto> GetAsync(Guid id)
    {
        _logger.LogDebug("Retrieving gap analysis: {AnalysisId}", id);
        
        var analysis = await _gapAnalysisRepository.GetAsync(id);
        return ObjectMapper.Map<AIGapAnalysis, AIGapAnalysisDto>(analysis);
    }
    
    /// <summary>
    /// الحصول على قائمة التحاليل - Get list of analyses
    /// </summary>
    public virtual async Task<List<AIGapAnalysisDto>> GetListAsync(
        Guid? assessmentId = null,
        AIAnalysisStatus? status = null)
    {
        _logger.LogDebug(
            "Retrieving gap analyses - AssessmentId: {AssessmentId}, Status: {Status}",
            assessmentId,
            status
        );
        
        var queryable = await _gapAnalysisRepository.GetQueryableAsync();
        
        if (assessmentId.HasValue)
        {
            queryable = queryable.Where(a => a.AssessmentId == assessmentId.Value);
        }
        
        if (status.HasValue)
        {
            queryable = queryable.Where(a => a.Status == status.Value);
        }
        
        var analyses = queryable
            .OrderByDescending(a => a.CreationTime)
            .ToList();
        
        return ObjectMapper.Map<List<AIGapAnalysis>, List<AIGapAnalysisDto>>(analyses);
    }
    
    /// <summary>
    /// الحصول على التوصيات - Get recommendations by priority
    /// </summary>
    public virtual async Task<List<AIRecommendationDto>> GetRecommendationsAsync(
        Guid analysisId,
        RecommendationPriority? priority = null)
    {
        _logger.LogDebug(
            "Retrieving recommendations - AnalysisId: {AnalysisId}, Priority: {Priority}",
            analysisId,
            priority
        );
        
        var analysis = await _gapAnalysisRepository.GetAsync(analysisId);
        
        var recommendations = analysis.Recommendations;
        
        if (priority.HasValue)
        {
            recommendations = recommendations
                .Where(r => r.Priority == priority.Value)
                .ToList();
        }
        
        return recommendations
            .OrderByDescending(r => r.Priority)
            .Select(r => ObjectMapper.Map<AIRecommendation, AIRecommendationDto>(r))
            .ToList();
    }
    
    /// <summary>
    /// الموافقة على التحليل - Approve analysis
    /// </summary>
    [UnitOfWork]
    public virtual async Task ApproveAsync(Guid id, ApproveGapAnalysisInput input)
    {
        _logger.LogInformation("Approving gap analysis: {AnalysisId}", id);
        
        var analysis = await _gapAnalysisRepository.GetAsync(id);
        analysis.Approve(input.ReviewerNotes);
        
        await _gapAnalysisRepository.UpdateAsync(analysis, autoSave: true);
        
        _logger.LogInformation("Gap analysis approved: {AnalysisId}", id);
    }
    
    /// <summary>
    /// رفض التحليل - Reject analysis
    /// </summary>
    [UnitOfWork]
    public virtual async Task RejectAsync(Guid id, RejectGapAnalysisInput input)
    {
        _logger.LogInformation("Rejecting gap analysis: {AnalysisId}", id);
        
        var analysis = await _gapAnalysisRepository.GetAsync(id);
        analysis.Reject(input.ReviewerNotes);
        
        await _gapAnalysisRepository.UpdateAsync(analysis, autoSave: true);
        
        _logger.LogInformation("Gap analysis rejected: {AnalysisId}", id);
    }
    
    /// <summary>
    /// إلغاء التحليل - Cancel analysis
    /// </summary>
    [UnitOfWork]
    public virtual async Task CancelAsync(Guid id)
    {
        _logger.LogInformation("Cancelling gap analysis: {AnalysisId}", id);
        
        var analysis = await _gapAnalysisRepository.GetAsync(id);
        analysis.Cancel();
        
        await _gapAnalysisRepository.UpdateAsync(analysis, autoSave: true);
        
        _logger.LogInformation("Gap analysis cancelled: {AnalysisId}", id);
    }
    
    /// <summary>
    /// حذف التحليل - Delete analysis
    /// </summary>
    [UnitOfWork]
    public virtual async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting gap analysis: {AnalysisId}", id);
        
        await _gapAnalysisRepository.DeleteAsync(id);
        
        _logger.LogInformation("Gap analysis deleted: {AnalysisId}", id);
    }
}
