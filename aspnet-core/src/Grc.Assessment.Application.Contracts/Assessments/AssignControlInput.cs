using System;
using System.ComponentModel.DataAnnotations;
using Grc.Enums;

namespace Grc.Assessments;

/// <summary>
/// Input for assigning a control to a user
/// </summary>
public class AssignControlInput
{
    [Required]
    public Guid UserId { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public Priority? Priority { get; set; }
}

