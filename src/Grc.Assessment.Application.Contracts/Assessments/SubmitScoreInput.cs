using System.ComponentModel.DataAnnotations;

namespace Grc.Assessments;

/// <summary>
/// Input for submitting self-score
/// </summary>
public class SubmitScoreInput
{
    [Required]
    [Range(0, 100)]
    public decimal Score { get; set; }
    
    [StringLength(4000)]
    public string Notes { get; set; }
}

