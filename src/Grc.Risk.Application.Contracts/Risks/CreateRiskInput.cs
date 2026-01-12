using System.ComponentModel.DataAnnotations;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Risk.Application.Contracts.Risks;

/// <summary>
/// Input for creating a risk
/// </summary>
public class CreateRiskInput
{
    [Required]
    [StringLength(30)]
    public string RiskCode { get; set; }
    
    [Required]
    public LocalizedString Title { get; set; }
    
    public LocalizedString Description { get; set; }
    
    [Required]
    public RiskCategory Category { get; set; }
}

