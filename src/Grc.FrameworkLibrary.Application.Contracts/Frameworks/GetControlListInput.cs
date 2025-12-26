using System;
using Grc.Enums;
using Volo.Abp.Application.Dtos;

namespace Grc.FrameworkLibrary.Frameworks;

/// <summary>
/// Input for getting control list
/// </summary>
public class GetControlListInput : PagedAndSortedResultRequestDto
{
    public Guid FrameworkId { get; set; }
    public string DomainCode { get; set; }
    public ControlType? ControlType { get; set; }
    public ControlCategory? Category { get; set; }
    public Priority? Priority { get; set; }
    public int? MinMaturityLevel { get; set; }
    public string Filter { get; set; }
}

