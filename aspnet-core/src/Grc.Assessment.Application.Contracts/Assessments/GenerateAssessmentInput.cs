using System;
using System.ComponentModel.DataAnnotations;
using Grc.Enums;

namespace Grc.Assessments;

/// <summary>
/// Input for auto-generating an assessment from organization profile
/// </summary>
public class GenerateAssessmentInput
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; }
    
    [Required]
    public AssessmentType Type { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime TargetEndDate { get; set; }
    
    public bool IncludeOptionalFrameworks { get; set; } = false;
}

