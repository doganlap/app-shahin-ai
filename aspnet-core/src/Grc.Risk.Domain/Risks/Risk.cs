using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Risk.Domain.Risks;

/// <summary>
/// Risk register entry
/// </summary>
public class Risk : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string RiskCode { get; private set; }
    public LocalizedString Title { get; private set; }
    public LocalizedString Description { get; private set; }
    public RiskCategory Category { get; private set; }
    public int InherentProbability { get; private set; }
    public int InherentImpact { get; private set; }
    public RiskLevel InherentRiskLevel { get; private set; }
    public int? ResidualProbability { get; private set; }
    public int? ResidualImpact { get; private set; }
    public RiskLevel? ResidualRiskLevel { get; private set; }
    public Guid? RiskOwnerId { get; private set; }
    public RiskStatus Status { get; private set; }
    
    public ICollection<RiskTreatment> Treatments { get; private set; }
    public ICollection<RiskControlLink> LinkedControls { get; private set; }
    
    protected Risk() { }
    
    public Risk(Guid id, string riskCode, LocalizedString title, RiskCategory category)
        : base(id)
    {
        RiskCode = Check.NotNullOrWhiteSpace(riskCode, nameof(riskCode), maxLength: 30);
        Title = Check.NotNull(title, nameof(title));
        Category = category;
        Status = RiskStatus.Identified;
        Treatments = new Collection<RiskTreatment>();
        LinkedControls = new Collection<RiskControlLink>();
    }
    
    public void Assess(int probability, int impact)
    {
        if (probability < 1 || probability > 5)
            throw new ArgumentOutOfRangeException(nameof(probability), "Must be between 1 and 5");
        if (impact < 1 || impact > 5)
            throw new ArgumentOutOfRangeException(nameof(impact), "Must be between 1 and 5");
        
        InherentProbability = probability;
        InherentImpact = impact;
        InherentRiskLevel = CalculateRiskLevel(probability, impact);
        Status = RiskStatus.Assessed;
    }
    
    public void ApplyTreatment(int residualProbability, int residualImpact)
    {
        if (residualProbability < 1 || residualProbability > 5)
            throw new ArgumentOutOfRangeException(nameof(residualProbability), "Must be between 1 and 5");
        if (residualImpact < 1 || residualImpact > 5)
            throw new ArgumentOutOfRangeException(nameof(residualImpact), "Must be between 1 and 5");
        
        ResidualProbability = residualProbability;
        ResidualImpact = residualImpact;
        ResidualRiskLevel = CalculateRiskLevel(residualProbability, residualImpact);
        Status = RiskStatus.Treated;
    }
    
    public void Accept()
    {
        Status = RiskStatus.Accepted;
    }
    
    public void Close()
    {
        Status = RiskStatus.Closed;
    }
    
    public void SetOwner(Guid? ownerId)
    {
        RiskOwnerId = ownerId;
    }
    
    public void SetDescription(LocalizedString description)
    {
        Description = description;
    }
    
    private RiskLevel CalculateRiskLevel(int probability, int impact)
    {
        var score = probability * impact;
        return score switch
        {
            >= 20 => RiskLevel.Critical,
            >= 12 => RiskLevel.High,
            >= 6 => RiskLevel.Medium,
            >= 3 => RiskLevel.Low,
            _ => RiskLevel.VeryLow
        };
    }
}

