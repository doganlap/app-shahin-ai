using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Grc.Assessments;

/// <summary>
/// Input for bulk assigning controls
/// </summary>
public class BulkAssignControlsInput
{
    [Required]
    [MinLength(1)]
    public List<ControlAssignment> Assignments { get; set; }
}

/// <summary>
/// Individual control assignment
/// </summary>
public class ControlAssignment
{
    [Required]
    public Guid ControlAssessmentId { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    public DateTime? DueDate { get; set; }
}

