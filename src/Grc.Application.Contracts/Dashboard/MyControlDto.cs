using System;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Dashboard;

/// <summary>
/// Control assigned to current user
/// </summary>
public class MyControlDto
{
    public Guid Id { get; set; }
    public string ControlNumber { get; set; }
    public LocalizedString ControlTitle { get; set; }
    public string FrameworkCode { get; set; }
    public ControlAssessmentStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsOverdue { get; set; }
    public Priority Priority { get; set; }
}

