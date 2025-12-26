using System;
using System.Collections.Generic;
using Grc.Enums;
using Grc.ValueObjects;
using Volo.Abp.Application.Dtos;

namespace Grc.FrameworkLibrary.Frameworks;

/// <summary>
/// Control data transfer object
/// </summary>
public class ControlDto : FullAuditedEntityDto<Guid>
{
    public Guid FrameworkId { get; set; }
    public Guid? ParentControlId { get; set; }
    public string ControlNumber { get; set; }
    public string DomainCode { get; set; }
    public LocalizedString Title { get; set; }
    public LocalizedString Requirement { get; set; }
    public LocalizedString ImplementationGuidance { get; set; }
    public ControlType Type { get; set; }
    public ControlCategory? Category { get; set; }
    public int MaturityLevel { get; set; }
    public Priority Priority { get; set; }
    public List<string> EvidenceTypes { get; set; }
    public int EstimatedEffortHours { get; set; }
    public string MappingISO27001 { get; set; }
    public string MappingNIST { get; set; }
    public string MappingCOBIT { get; set; }
    public List<string> Tags { get; set; }
    public FrameworkStatus Status { get; set; }
}

