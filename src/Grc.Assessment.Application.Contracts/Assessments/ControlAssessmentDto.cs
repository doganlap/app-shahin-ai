using System;
using Grc.Enums;
using Volo.Abp.Application.Dtos;

namespace Grc.Assessments;

/// <summary>
/// Control assessment data transfer object
/// </summary>
public class ControlAssessmentDto : FullAuditedEntityDto<Guid>
{
    public Guid AssessmentId { get; set; }
    public Guid ControlId { get; set; }
    public ControlDto Control { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public ControlAssessmentStatus Status { get; set; }
    public decimal? SelfScore { get; set; }
    public decimal? VerifiedScore { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsOverdue { get; set; }
    public Priority Priority { get; set; }
    public int EvidenceCount { get; set; }
}

