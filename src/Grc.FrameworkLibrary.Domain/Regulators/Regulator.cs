using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.FrameworkLibrary.Regulators;

/// <summary>
/// Regulatory authority that issues compliance frameworks
/// </summary>
public class Regulator : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; private set; }
    public LocalizedString Name { get; private set; }
    public LocalizedString Jurisdiction { get; private set; }
    public string Website { get; private set; }
    public RegulatorCategory Category { get; private set; }
    public string LogoUrl { get; private set; }
    public ContactInfo Contact { get; private set; }
    
    public ICollection<Frameworks.Framework> Frameworks { get; private set; }
    
    protected Regulator() { }
    
    public Regulator(Guid id, string code, LocalizedString name, RegulatorCategory category)
        : base(id)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 20);
        Name = Check.NotNull(name, nameof(name));
        Category = category;
        Frameworks = new Collection<Frameworks.Framework>();
    }
    
    public void SetWebsite(string website)
    {
        Website = website;
    }
    
    public void SetLogo(string logoUrl)
    {
        LogoUrl = logoUrl;
    }
    
    public void SetContact(ContactInfo contact)
    {
        Contact = contact;
    }
    
    public void SetJurisdiction(LocalizedString jurisdiction)
    {
        Jurisdiction = jurisdiction;
    }
}

