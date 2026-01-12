using System.ComponentModel.DataAnnotations;

namespace Grc.Risk.Application.Contracts.Risks;

/// <summary>
/// Input for applying risk treatment
/// </summary>
public class ApplyTreatmentInput
{
    [Required]
    [Range(1, 5)]
    public int ResidualProbability { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int ResidualImpact { get; set; }
    
    [StringLength(2000)]
    public string TreatmentDescription { get; set; }
}

