using System.ComponentModel.DataAnnotations;

namespace Grc.Risk.Application.Contracts.Risks;

/// <summary>
/// Input for assessing a risk
/// </summary>
public class AssessRiskInput
{
    [Required]
    [Range(1, 5)]
    public int Probability { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Impact { get; set; }
}

