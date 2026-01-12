using System.ComponentModel.DataAnnotations;

namespace Grc.Assessments;

/// <summary>
/// Input for verifying a control
/// </summary>
public class VerifyControlInput
{
    [Required]
    [Range(0, 100)]
    public decimal Score { get; set; }
    
    [StringLength(2000)]
    public string Notes { get; set; }
}

