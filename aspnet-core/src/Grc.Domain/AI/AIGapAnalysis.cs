using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.ValueObjects;

namespace Grc.AI;

/// <summary>
/// تحليل الفجوات بالذكاء الاصطناعي - AI-Powered Gap Analysis
/// Identifies compliance gaps and generates recommendations
/// </summary>
public class AIGapAnalysis : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    /// <summary>
    /// Assessment being analyzed
    /// </summary>
    public Guid? AssessmentId { get; private set; }
    
    /// <summary>
    /// Framework being analyzed against
    /// </summary>
    public Guid? FrameworkId { get; private set; }
    
    /// <summary>
    /// Analysis type
    /// </summary>
    public AIAnalysisType AnalysisType { get; private set; }
    
    /// <summary>
    /// Current status
    /// </summary>
    public AIAnalysisStatus Status { get; private set; }
    
    /// <summary>
    /// عنوان التحليل - Analysis title (bilingual)
    /// </summary>
    public LocalizedString Title { get; private set; }
    
    /// <summary>
    /// وصف التحليل - Analysis description
    /// </summary>
    public LocalizedString Description { get; private set; }
    
    /// <summary>
    /// When analysis started
    /// </summary>
    public DateTime? StartedAt { get; private set; }
    
    /// <summary>
    /// When analysis completed
    /// </summary>
    public DateTime? CompletedAt { get; private set; }
    
    /// <summary>
    /// AI model used (e.g., "GPT-4", "Claude-3")
    /// </summary>
    public string AIModel { get; private set; } = string.Empty;
    
    /// <summary>
    /// AI model version
    /// </summary>
    public string ModelVersion { get; private set; } = string.Empty;
    
    /// <summary>
    /// Confidence score (0-100)
    /// </summary>
    public decimal ConfidenceScore { get; private set; }
    
    /// <summary>
    /// Number of gaps identified
    /// </summary>
    public int TotalGapsIdentified { get; private set; }
    
    /// <summary>
    /// Critical gaps count
    /// </summary>
    public int CriticalGaps { get; private set; }
    
    /// <summary>
    /// High priority gaps
    /// </summary>
    public int HighPriorityGaps { get; private set; }
    
    /// <summary>
    /// Medium priority gaps
    /// </summary>
    public int MediumPriorityGaps { get; private set; }
    
    /// <summary>
    /// Low priority gaps
    /// </summary>
    public int LowPriorityGaps { get; private set; }
    
    /// <summary>
    /// Identified gaps with details
    /// </summary>
    public List<GapDetail> Gaps { get; private set; }
    
    /// <summary>
    /// AI-generated recommendations
    /// </summary>
    public List<AIRecommendation> Recommendations { get; private set; }
    
    /// <summary>
    /// Overall compliance percentage
    /// </summary>
    public decimal CompliancePercentage { get; private set; }
    
    /// <summary>
    /// Processing time in seconds
    /// </summary>
    public int ProcessingTimeSeconds { get; private set; }
    
    /// <summary>
    /// Number of controls analyzed
    /// </summary>
    public int ControlsAnalyzed { get; private set; }
    
    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Notes from reviewer
    /// </summary>
    public string? ReviewerNotes { get; set; }
    
    protected AIGapAnalysis()
    {
        Gaps = new List<GapDetail>();
        Recommendations = new List<AIRecommendation>();
    }
    
    public AIGapAnalysis(
        Guid id,
        AIAnalysisType analysisType,
        LocalizedString title,
        LocalizedString description,
        Guid? assessmentId = null,
        Guid? frameworkId = null,
        Guid? tenantId = null)
        : base(id)
    {
        Title = Check.NotNull(title, nameof(title));
        AnalysisType = analysisType;
        AssessmentId = assessmentId;
        FrameworkId = frameworkId;
        TenantId = tenantId;
        
        Status = AIAnalysisStatus.Pending;
        Description = new(string.Empty, string.Empty);
        Gaps = new List<GapDetail>();
        Recommendations = new List<AIRecommendation>();
        ConfidenceScore = 0;
    }
    
    /// <summary>
    /// بدء التحليل - Start the analysis
    /// </summary>
    public void Start(string aiModel, string modelVersion)
    {
        if (Status != AIAnalysisStatus.Pending)
            throw new BusinessException("Analysis can only be started from Pending state");
        
        AIModel = Check.NotNullOrWhiteSpace(aiModel, nameof(aiModel));
        ModelVersion = Check.NotNullOrWhiteSpace(modelVersion, nameof(modelVersion));
        Status = AIAnalysisStatus.Running;
        StartedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// إضافة فجوة - Add identified gap
    /// </summary>
    public void AddGap(GapDetail gap)
    {
        Check.NotNull(gap, nameof(gap));
        
        Gaps.Add(gap);
        TotalGapsIdentified = Gaps.Count;
        
        // Update priority counters
        RecalculateGapCounters();
    }
    
    /// <summary>
    /// إضافة توصية - Add AI recommendation
    /// </summary>
    public void AddRecommendation(AIRecommendation recommendation)
    {
        Check.NotNull(recommendation, nameof(recommendation));
        Recommendations.Add(recommendation);
    }
    
    /// <summary>
    /// إكمال التحليل - Complete the analysis
    /// </summary>
    public void Complete(decimal compliancePercentage, decimal confidenceScore, int processingTimeSeconds)
    {
        if (Status != AIAnalysisStatus.Running)
            throw new BusinessException("Analysis must be running to complete");
        
        if (compliancePercentage < 0 || compliancePercentage > 100)
            throw new ArgumentOutOfRangeException(nameof(compliancePercentage), "Must be between 0 and 100");
        
        if (confidenceScore < 0 || confidenceScore > 100)
            throw new ArgumentOutOfRangeException(nameof(confidenceScore), "Must be between 0 and 100");
        
        CompliancePercentage = compliancePercentage;
        ConfidenceScore = confidenceScore;
        ProcessingTimeSeconds = processingTimeSeconds;
        Status = AIAnalysisStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        
        RecalculateGapCounters();
    }
    
    /// <summary>
    /// فشل التحليل - Mark analysis as failed
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        ErrorMessage = Check.NotNullOrWhiteSpace(errorMessage, nameof(errorMessage));
        Status = AIAnalysisStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// إلغاء التحليل - Cancel the analysis
    /// </summary>
    public void Cancel()
    {
        if (Status == AIAnalysisStatus.Completed || Status == AIAnalysisStatus.Approved)
            throw new BusinessException("Cannot cancel completed or approved analysis");
        
        Status = AIAnalysisStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// إرسال للمراجعة - Submit for human review
    /// </summary>
    public void SubmitForReview()
    {
        if (Status != AIAnalysisStatus.Completed)
            throw new BusinessException("Only completed analysis can be submitted for review");
        
        Status = AIAnalysisStatus.UnderReview;
    }
    
    /// <summary>
    /// الموافقة على التحليل - Approve the analysis
    /// </summary>
    public void Approve(string? reviewerNotes = null)
    {
        if (Status != AIAnalysisStatus.UnderReview)
            throw new BusinessException("Only analysis under review can be approved");
        
        ReviewerNotes = reviewerNotes;
        Status = AIAnalysisStatus.Approved;
    }
    
    /// <summary>
    /// رفض التحليل - Reject the analysis
    /// </summary>
    public void Reject(string reviewerNotes)
    {
        if (Status != AIAnalysisStatus.UnderReview)
            throw new BusinessException("Only analysis under review can be rejected");
        
        ReviewerNotes = Check.NotNullOrWhiteSpace(reviewerNotes, nameof(reviewerNotes));
        Status = AIAnalysisStatus.Failed;
    }
    
    private void RecalculateGapCounters()
    {
        CriticalGaps = Gaps.Count(g => g.Priority == RecommendationPriority.Critical);
        HighPriorityGaps = Gaps.Count(g => g.Priority == RecommendationPriority.High);
        MediumPriorityGaps = Gaps.Count(g => g.Priority == RecommendationPriority.Medium);
        LowPriorityGaps = Gaps.Count(g => g.Priority == RecommendationPriority.Low);
        TotalGapsIdentified = Gaps.Count;
    }
}

/// <summary>
/// تفاصيل الفجوة - Gap detail identified by AI
/// </summary>
public class GapDetail
{
    /// <summary>
    /// Control ID related to this gap
    /// </summary>
    public Guid? ControlId { get; set; }
    
    /// <summary>
    /// Control number (e.g., "NCA-1.2.3")
    /// </summary>
    public string ControlNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Gap title (bilingual)
    /// </summary>
    public LocalizedString Title { get; set; } = new(string.Empty, string.Empty);
    
    /// <summary>
    /// Gap description (bilingual)
    /// </summary>
    public LocalizedString Description { get; set; } = new(string.Empty, string.Empty);
    
    /// <summary>
    /// Priority level
    /// </summary>
    public RecommendationPriority Priority { get; set; }
    
    /// <summary>
    /// Current compliance percentage for this control
    /// </summary>
    public decimal CurrentComplianceLevel { get; set; }
    
    /// <summary>
    /// Target compliance percentage
    /// </summary>
    public decimal TargetComplianceLevel { get; set; }
    
    /// <summary>
    /// Gap size (target - current)
    /// </summary>
    public decimal GapSize => TargetComplianceLevel - CurrentComplianceLevel;
    
    /// <summary>
    /// Estimated effort (hours)
    /// </summary>
    public int EstimatedEffortHours { get; set; }
    
    /// <summary>
    /// Risk if not addressed
    /// </summary>
    public LocalizedString RiskIfUnaddressed { get; set; } = new(string.Empty, string.Empty);
    
    /// <summary>
    /// AI confidence in this finding (0-100)
    /// </summary>
    public decimal Confidence { get; set; }
}

/// <summary>
/// التوصية الذكية - AI-Generated Recommendation
/// </summary>
public class AIRecommendation
{
    /// <summary>
    /// Recommendation title (bilingual)
    /// </summary>
    public LocalizedString Title { get; set; } = new(string.Empty, string.Empty);
    
    /// <summary>
    /// Detailed recommendation (bilingual)
    /// </summary>
    public LocalizedString Recommendation { get; set; } = new(string.Empty, string.Empty);
    
    /// <summary>
    /// Priority level
    /// </summary>
    public RecommendationPriority Priority { get; set; }
    
    /// <summary>
    /// Category (e.g., "Technical", "Process", "People", "Documentation")
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Related control IDs
    /// </summary>
    public List<Guid> RelatedControlIds { get; set; } = new List<Guid>();
    
    /// <summary>
    /// Implementation steps (bilingual)
    /// </summary>
    public List<LocalizedString> ImplementationSteps { get; set; } = new();
    
    /// <summary>
    /// Expected impact if implemented
    /// </summary>
    public LocalizedString ExpectedImpact { get; set; } = new(string.Empty, string.Empty);
    
    /// <summary>
    /// Estimated implementation time (days)
    /// </summary>
    public int EstimatedDays { get; set; }
    
    /// <summary>
    /// Estimated cost (SAR)
    /// </summary>
    public decimal EstimatedCost { get; set; }
    
    /// <summary>
    /// AI confidence (0-100)
    /// </summary>
    public decimal Confidence { get; set; }
    
    /// <summary>
    /// Supporting evidence/reasoning
    /// </summary>
    public string Reasoning { get; set; } = string.Empty;
}
