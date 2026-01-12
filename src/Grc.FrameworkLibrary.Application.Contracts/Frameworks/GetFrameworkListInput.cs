using System;
using Grc.Enums;
using Volo.Abp.Application.Dtos;

namespace Grc.FrameworkLibrary.Frameworks;

/// <summary>
/// Input for getting framework list
/// </summary>
public class GetFrameworkListInput : PagedAndSortedResultRequestDto
{
    public Guid? RegulatorId { get; set; }
    public FrameworkCategory? Category { get; set; }
    public FrameworkStatus? Status { get; set; }
    public bool? IsMandatory { get; set; }
    public string Filter { get; set; }
}

