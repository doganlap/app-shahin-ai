using System;
using System.Collections.Generic;
using Grc.Enums;
using Grc.ValueObjects;
using Volo.Abp.Application.Dtos;

namespace Grc.Risk.Application.Contracts.Risks;

/// <summary>
/// Risk data transfer object
/// </summary>
public class RiskDto : FullAuditedEntityDto<Guid>
{
    public string RiskCode { get; set; }
    public LocalizedString Title { get; set; }
    public LocalizedString Description { get; set; }
    public RiskCategory Category { get; set; }
    public int InherentProbability { get; set; }
    public int InherentImpact { get; set; }
    public RiskLevel InherentRiskLevel { get; set; }
    public int? ResidualProbability { get; set; }
    public int? ResidualImpact { get; set; }
    public RiskLevel? ResidualRiskLevel { get; set; }
    public Guid? RiskOwnerId { get; set; }
    public RiskStatus Status { get; set; }
    public List<RiskTreatmentDto> Treatments { get; set; }
}

