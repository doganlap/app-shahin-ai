using System;
using Grc.Enums;
using Grc.ValueObjects;
using Grc.FrameworkLibrary.Application.Contracts.Regulators;
using Volo.Abp.Application.Dtos;

namespace Grc.FrameworkLibrary.Application.Contracts.Frameworks;

/// <summary>
/// Framework data transfer object
/// </summary>
public class FrameworkDto : FullAuditedEntityDto<Guid>
{
    public Guid RegulatorId { get; set; }
    public RegulatorDto Regulator { get; set; }
    public string Code { get; set; }
    public string Version { get; set; }
    public LocalizedString Title { get; set; }
    public LocalizedString Description { get; set; }
    public FrameworkCategory Category { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? SunsetDate { get; set; }
    public FrameworkStatus Status { get; set; }
    public string OfficialDocumentUrl { get; set; }
    public int TotalControls { get; set; }
    public bool IsActive { get; set; }
}

