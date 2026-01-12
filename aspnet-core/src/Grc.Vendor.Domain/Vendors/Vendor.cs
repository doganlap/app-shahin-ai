using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.ValueObjects;

namespace Grc.Vendor.Domain.Vendors;

/// <summary>
/// Vendor entity for vendor management
/// </summary>
public class Vendor : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string VendorName { get; private set; }
    public LocalizedString Name { get; private set; }
    public string? Category { get; private set; }
    public int RiskScore { get; private set; }
    public string Status { get; private set; }
    public ContactInfo? Contact { get; private set; }
    public string? Website { get; private set; }
    public string? Description { get; private set; }
    
    protected Vendor() { }
    
    public Vendor(Guid id, string vendorName, LocalizedString name, string? category = null)
        : base(id)
    {
        VendorName = Check.NotNullOrWhiteSpace(vendorName, nameof(vendorName), maxLength: 200);
        Name = Check.NotNull(name, nameof(name));
        Category = category;
        RiskScore = 0;
        Status = "Active";
    }
    
    public void SetCategory(string? category)
    {
        Category = category;
    }
    
    public void SetRiskScore(int riskScore)
    {
        if (riskScore < 0 || riskScore > 10)
            throw new ArgumentOutOfRangeException(nameof(riskScore), "Risk score must be between 0 and 10");
        RiskScore = riskScore;
    }
    
    public void SetStatus(string status)
    {
        Status = Check.NotNullOrWhiteSpace(status, nameof(status), maxLength: 50);
    }
    
    public void SetContact(ContactInfo? contact)
    {
        Contact = contact;
    }
    
    public void SetWebsite(string? website)
    {
        Website = website;
    }
    
    public void SetDescription(string? description)
    {
        Description = description;
    }
}

