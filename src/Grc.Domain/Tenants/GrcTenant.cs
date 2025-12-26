using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Tenants;

/// <summary>
/// Extended tenant configuration with organization profile
/// </summary>
public class GrcTenant : Entity<Guid>
{
    public Guid TenantId { get; private set; }
    
    public LocalizedString OrganizationName { get; private set; }
    
    public string CommercialRegistration { get; private set; }
    
    public string VatNumber { get; private set; }
    
    public IndustrySector IndustrySector { get; private set; }
    
    public LegalEntityType LegalEntityType { get; private set; }
    
    public int? EmployeeCount { get; private set; }
    
    public decimal? AnnualRevenue { get; private set; }
    
    public SubscriptionTier SubscriptionTier { get; private set; }
    
    public DatabaseStrategy DatabaseStrategy { get; private set; }
    
    public string ConnectionString { get; private set; }
    
    public List<string> OperatingLicenses { get; private set; }
    
    public List<string> DataTypesProcessed { get; private set; }
    
    public bool ProcessesPayments { get; private set; }
    
    public string CloudEnvironment { get; private set; }
    
    public string LogoUrl { get; private set; }
    
    public string PrimaryColor { get; private set; }
    
    public Dictionary<string, object> Settings { get; private set; }
    
    public Dictionary<string, object> Quotas { get; private set; }
    
    public Dictionary<string, object> BillingInfo { get; private set; }
    
    public DateTime CreationTime { get; private set; }
    
    public DateTime? LastModificationTime { get; private set; }
    
    protected GrcTenant() { }
    
    public GrcTenant(
        Guid id,
        Guid tenantId,
        LocalizedString organizationName,
        IndustrySector industrySector,
        LegalEntityType legalEntityType)
    {
        Id = id;
        TenantId = tenantId;
        OrganizationName = organizationName ?? throw new ArgumentNullException(nameof(organizationName));
        IndustrySector = industrySector;
        LegalEntityType = legalEntityType;
        SubscriptionTier = SubscriptionTier.Standard;
        DatabaseStrategy = DatabaseStrategy.Shared;
        OperatingLicenses = new List<string>();
        DataTypesProcessed = new List<string>();
        Settings = new Dictionary<string, object>();
        Quotas = new Dictionary<string, object>();
        BillingInfo = new Dictionary<string, object>();
        CreationTime = DateTime.UtcNow;
    }
    
    public void UpdateOrganizationName(LocalizedString organizationName)
    {
        OrganizationName = organizationName ?? throw new ArgumentNullException(nameof(organizationName));
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void UpdateIndustrySector(IndustrySector industrySector)
    {
        IndustrySector = industrySector;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void UpdateSubscriptionTier(SubscriptionTier subscriptionTier)
    {
        SubscriptionTier = subscriptionTier;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void UpdateDatabaseStrategy(DatabaseStrategy databaseStrategy, string connectionString = null)
    {
        DatabaseStrategy = databaseStrategy;
        ConnectionString = connectionString;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void SetCommercialRegistration(string commercialRegistration)
    {
        CommercialRegistration = commercialRegistration;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void SetVatNumber(string vatNumber)
    {
        VatNumber = vatNumber;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void SetEmployeeCount(int? employeeCount)
    {
        EmployeeCount = employeeCount;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void SetAnnualRevenue(decimal? annualRevenue)
    {
        AnnualRevenue = annualRevenue;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void AddOperatingLicense(string license)
    {
        if (!string.IsNullOrWhiteSpace(license) && !OperatingLicenses.Contains(license))
        {
            OperatingLicenses.Add(license);
            LastModificationTime = DateTime.UtcNow;
        }
    }
    
    public void AddDataTypeProcessed(string dataType)
    {
        if (!string.IsNullOrWhiteSpace(dataType) && !DataTypesProcessed.Contains(dataType))
        {
            DataTypesProcessed.Add(dataType);
            LastModificationTime = DateTime.UtcNow;
        }
    }
    
    public void SetProcessesPayments(bool processesPayments)
    {
        ProcessesPayments = processesPayments;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void SetCloudEnvironment(string cloudEnvironment)
    {
        CloudEnvironment = cloudEnvironment;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void SetLogoUrl(string logoUrl)
    {
        LogoUrl = logoUrl;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void SetPrimaryColor(string primaryColor)
    {
        PrimaryColor = primaryColor;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void UpdateSetting(string key, object value)
    {
        Settings[key] = value;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void UpdateQuota(string key, object value)
    {
        Quotas[key] = value;
        LastModificationTime = DateTime.UtcNow;
    }
    
    public void UpdateBillingInfo(string key, object value)
    {
        BillingInfo[key] = value;
        LastModificationTime = DateTime.UtcNow;
    }
}

