using System.ComponentModel.DataAnnotations;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.FrameworkLibrary.Application.Contracts.Regulators;

public class CreateUpdateRegulatorDto
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; }

    [Required]
    public LocalizedString Name { get; set; }

    public LocalizedString Jurisdiction { get; set; }

    [StringLength(500)]
    public string Website { get; set; }

    [Required]
    public RegulatorCategory Category { get; set; }

    [StringLength(500)]
    public string LogoUrl { get; set; }

    public ContactInfo Contact { get; set; }
}

