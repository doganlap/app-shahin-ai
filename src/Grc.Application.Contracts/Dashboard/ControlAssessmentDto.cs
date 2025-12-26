using System;
using Grc.Enums;

namespace Grc.Dashboard;

/// <summary>
/// Control assessment DTO for dashboard
/// </summary>
public class ControlAssessmentDto
{
    public Guid Id { get; set; }
    public Guid AssessmentId { get; set; }
    public Guid ControlId { get; set; }
    public string ControlNumber { get; set; }
    public string ControlTitle { get; set; }
    public ControlAssessmentStatus Status { get; set; }
    public decimal? SelfScore { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
}

