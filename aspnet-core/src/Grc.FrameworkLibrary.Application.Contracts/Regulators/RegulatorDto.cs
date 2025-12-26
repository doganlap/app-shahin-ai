using System;
using Grc.Enums;
using Grc.ValueObjects;
using Volo.Abp.Application.Dtos;

namespace Grc.FrameworkLibrary.Application.Contracts.Regulators;

public class RegulatorDto : EntityDto<Guid>
{
    public string Code { get; set; }
    public LocalizedString Name { get; set; }
    public LocalizedString Jurisdiction { get; set; }
    public string Website { get; set; }
    public RegulatorCategory Category { get; set; }
    public string LogoUrl { get; set; }
    public ContactInfo Contact { get; set; }
    public DateTime CreationTime { get; set; }
}
