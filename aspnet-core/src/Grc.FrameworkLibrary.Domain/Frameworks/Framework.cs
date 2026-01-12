using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.FrameworkLibrary.Domain.Frameworks;

/// <summary>
/// Regulatory compliance framework
/// </summary>
public class Framework : FullAuditedAggregateRoot<Guid>
{
    public Guid RegulatorId { get; private set; }
    public string Code { get; private set; }
    public string Version { get; private set; }
    public LocalizedString Title { get; private set; }
    public LocalizedString Description { get; private set; }
    public FrameworkCategory Category { get; private set; }
    public bool IsMandatory { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? SunsetDate { get; private set; }
    public FrameworkStatus Status { get; private set; }
    public string OfficialDocumentUrl { get; private set; }
    
    public ICollection<FrameworkDomain> Domains { get; private set; }
    public ICollection<Control> Controls { get; private set; }
    public ICollection<ApplicabilityCriteria> ApplicabilityCriteria { get; private set; }
    
    public int TotalControls => Controls?.Count ?? 0;
    
    public bool IsActive => Status == FrameworkStatus.Active && 
                           EffectiveDate <= DateTime.UtcNow && 
                           (SunsetDate == null || SunsetDate > DateTime.UtcNow);
    
    protected Framework() { }
    
    public Framework(Guid id, Guid regulatorId, string code, string version, 
                    LocalizedString title, FrameworkCategory category, DateTime effectiveDate)
        : base(id)
    {
        RegulatorId = regulatorId;
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 30);
        Version = Check.NotNullOrWhiteSpace(version, nameof(version), maxLength: 20);
        Title = Check.NotNull(title, nameof(title));
        Category = category;
        EffectiveDate = effectiveDate;
        Status = FrameworkStatus.Active;
        IsMandatory = true;
        Domains = new Collection<FrameworkDomain>();
        Controls = new Collection<Control>();
        ApplicabilityCriteria = new Collection<ApplicabilityCriteria>();
    }
    
    public Control AddControl(Guid controlId, string controlNumber, string domainCode, LocalizedString title, 
                              LocalizedString requirement, ControlType type)
    {
        var control = new Control(
            controlId, 
            Id, 
            controlNumber, 
            domainCode, 
            title, 
            requirement, 
            type);
        Controls.Add(control);
        return control;
    }
    
    public void Deprecate()
    {
        Status = FrameworkStatus.Deprecated;
        SunsetDate = DateTime.UtcNow;
    }
    
    public void SetDescription(LocalizedString description)
    {
        Description = description;
    }
    
    public void SetOfficialDocumentUrl(string url)
    {
        OfficialDocumentUrl = url;
    }
    
    public void SetMandatory(bool isMandatory)
    {
        IsMandatory = isMandatory;
    }
}

