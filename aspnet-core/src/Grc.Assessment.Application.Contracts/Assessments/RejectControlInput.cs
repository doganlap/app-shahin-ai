using System.ComponentModel.DataAnnotations;

namespace Grc.Assessments;

/// <summary>
/// Input for rejecting a control assessment
/// </summary>
public class RejectControlInput
{
    [Required]
    [MinLength(10)]
    [StringLength(2000)]
    public string Reason { get; set; }
}

