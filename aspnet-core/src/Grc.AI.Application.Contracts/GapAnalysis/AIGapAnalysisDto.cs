using System;
using System.Collections.Generic;
using Grc.Domain.Shared;
using Grc.Domain.Shared.AI;
using Volo.Abp.Application.Dtos;

namespace Grc.AI.GapAnalysis;

/// <summary>
/// AI Gap Analysis DTO
/// </summary>
public class AIGapAnalysisDto : AuditedEntityDto<Guid>
{
    public Guid? AssessmentId { get; set; }
    public Guid? FrameworkId { get; set; }
    public AIAnalysisType AnalysisType { get; set; }
    public AIAnalysisStatus Status { get; set; }
    public LocalizedString Title { get; set; } = default!;
    public LocalizedString Description { get; set; } = default!;
    public string AIModel { get; set; } = default!;
    public string ModelVersion { get; set; } = default!;
    public decimal ConfidenceScore { get; set; }
    public int TotalGapsIdentified { get; set; }
    public int CriticalGaps { get; set; }
    public int HighPriorityGaps { get; set; }
    public int MediumPriorityGaps { get; set; }
    public int LowPriorityGaps { get; set; }
    public decimal CompliancePercentage { get; set; }
    public int ProcessingTimeSeconds { get; set; }
    public int ControlsAnalyzed { get; set; }
    public List<GapDetailDto> Gaps { get; set; } = new();
    public List<AIRecommendationDto> Recommendations { get; set; } = new();
}

public class GapDetailDto
{
    public Guid? ControlId { get; set; }
    public string ControlNumber { get; set; } = default!;
    public LocalizedString Title { get; set; } = default!;
    public LocalizedString Description { get; set; } = default!;
    public RecommendationPriority Priority { get; set; }
    public decimal CurrentComplianceLevel { get; set; }
    public decimal TargetComplianceLevel { get; set; }
    public decimal GapSize { get; set; }
    public int EstimatedEffortHours { get; set; }
    public LocalizedString RiskIfUnaddressed { get; set; } = default!;
    public decimal Confidence { get; set; }
}

public class AIRecommendationDto
{
    public LocalizedString Title { get; set; } = default!;
    public LocalizedString Recommendation { get; set; } = default!;
    public RecommendationPriority Priority { get; set; }
    public string Category { get; set; } = default!;
    public List<Guid> RelatedControlIds { get; set; } = new();
    public List<LocalizedString> ImplementationSteps { get; set; } = new();
    public LocalizedString ExpectedImpact { get; set; } = default!;
    public int EstimatedDays { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal Confidence { get; set; }
    public string Reasoning { get; set; } = default!;
}

public class StartGapAnalysisInput
{
    public Guid? AssessmentId { get; set; }
    public Guid? FrameworkId { get; set; }
    public AIAnalysisType AnalysisType { get; set; } = AIAnalysisType.GapAnalysis;
    public LocalizedString Title { get; set; } = default!;
    public LocalizedString Description { get; set; } = default!;
}

public class ApproveGapAnalysisInput
{
    public string? ReviewerNotes { get; set; }
}

public class RejectGapAnalysisInput
{
    public string ReviewerNotes { get; set; } = default!;
}
