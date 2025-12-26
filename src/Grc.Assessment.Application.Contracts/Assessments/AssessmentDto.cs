using System;
using Grc.Enums;
using Volo.Abp.Application.Dtos;

namespace Grc.Assessments;

/// <summary>
/// Assessment data transfer object
/// </summary>
public class AssessmentDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public AssessmentType Type { get; set; }
    public AssessmentStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public Guid? OwnerUserId { get; set; }
    public int TotalControls { get; set; }
    public int CompletedControls { get; set; }
    public decimal CompletionPercentage { get; set; }
    public decimal OverallScore { get; set; }
}

