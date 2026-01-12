using System;
using Grc.ValueObjects;
using Volo.Abp.Application.Dtos;

namespace Grc.FrameworkLibrary.Regulators;

/// <summary>
/// Regulator data transfer object
/// </summary>
public class RegulatorDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public LocalizedString Name { get; set; }
    public LocalizedString Jurisdiction { get; set; }
    public string Website { get; set; }
    public string Category { get; set; }
    public string LogoUrl { get; set; }
    public ContactInfo Contact { get; set; }
    public int FrameworkCount { get; set; }
}

