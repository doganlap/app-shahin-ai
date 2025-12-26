using System;
using Grc.Enums;
using Volo.Abp.Application.Dtos;

namespace Grc.Risk.Application.Contracts.Risks;

/// <summary>
/// Input for getting risk list
/// </summary>
public class GetRiskListInput : PagedAndSortedResultRequestDto
{
    public RiskCategory? Category { get; set; }
    public RiskStatus? Status { get; set; }
    public RiskLevel? RiskLevel { get; set; }
    public Guid? RiskOwnerId { get; set; }
    public string Filter { get; set; }
}

