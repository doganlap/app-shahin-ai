using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Grc.Enums;

namespace Grc.Assessments;

/// <summary>
/// Input for creating a new assessment
/// </summary>
public class CreateAssessmentInput
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; }
    
    [StringLength(2000)]
    public string Description { get; set; }
    
    [Required]
    public AssessmentType Type { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime TargetEndDate { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<Guid> FrameworkIds { get; set; }
}

