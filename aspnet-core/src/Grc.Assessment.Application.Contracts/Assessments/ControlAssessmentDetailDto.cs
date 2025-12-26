using System;
using System.Collections.Generic;
using Grc.Enums;

namespace Grc.Assessments;

/// <summary>
/// Detailed control assessment DTO
/// </summary>
public class ControlAssessmentDetailDto : ControlAssessmentDto
{
    public string ImplementationNotes { get; set; }
    public string RejectionReason { get; set; }
    public DateTime? VerificationDate { get; set; }
    public List<EvidenceDto> Evidences { get; set; }
    public List<CommentDto> Comments { get; set; }
    public List<HistoryDto> History { get; set; }
}

