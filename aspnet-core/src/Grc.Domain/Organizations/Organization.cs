using System;
using System.Collections.Generic;
using Grc.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Organizations;

public class Organization : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; private set; }
    public string NameAr { get; private set; }
    public string Code { get; private set; }
    public string Industry { get; private set; }
    public OrganizationSize Size { get; private set; }
    public string Country { get; private set; }
    public string City { get; private set; }
    public string Address { get; private set; }
    public string Website { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string LogoUrl { get; private set; }
    public string Description { get; private set; }
    public int EmployeeCount { get; private set; }
    public DateTime? EstablishedDate { get; private set; }
    public string FiscalYearEnd { get; private set; }
    public string PrimaryContact { get; private set; }
    public string PrimaryContactEmail { get; private set; }
    public bool IsActive { get; private set; }

    public virtual ICollection<OrganizationUnit> Units { get; private set; }
    public virtual ICollection<OrganizationFramework> Frameworks { get; private set; }

    protected Organization() { }

    public Organization(
        Guid id,
        string name,
        string code,
        string industry,
        OrganizationSize size,
        string country,
        Guid? tenantId = null)
        : base(id)
    {
        Name = name;
        Code = code;
        Industry = industry;
        Size = size;
        Country = country;
        TenantId = tenantId;
        IsActive = true;
        Units = new List<OrganizationUnit>();
        Frameworks = new List<OrganizationFramework>();
    }

    public void SetDetails(
        string nameAr,
        string city,
        string address,
        string website,
        string email,
        string phone,
        string description,
        int employeeCount)
    {
        NameAr = nameAr;
        City = city;
        Address = address;
        Website = website;
        Email = email;
        Phone = phone;
        Description = description;
        EmployeeCount = employeeCount;
    }

    public void SetLogo(string logoUrl) => LogoUrl = logoUrl;
    public void SetEstablishedDate(DateTime date) => EstablishedDate = date;
    public void SetFiscalYearEnd(string fiscalYearEnd) => FiscalYearEnd = fiscalYearEnd;

    public void SetPrimaryContact(string name, string email)
    {
        PrimaryContact = name;
        PrimaryContactEmail = email;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

public class OrganizationUnit : FullAuditedEntity<Guid>
{
    public Guid OrganizationId { get; private set; }
    public Guid? ParentUnitId { get; private set; }
    public string Name { get; private set; }
    public string NameAr { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public string ManagerName { get; private set; }
    public string ManagerEmail { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    protected OrganizationUnit() { }

    public OrganizationUnit(Guid id, Guid organizationId, string name, string code)
        : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Code = code;
        IsActive = true;
    }

    public void SetParent(Guid? parentUnitId) => ParentUnitId = parentUnitId;
    public void SetManager(string name, string email)
    {
        ManagerName = name;
        ManagerEmail = email;
    }
}

public class OrganizationFramework : FullAuditedEntity<Guid>
{
    public Guid OrganizationId { get; private set; }
    public Guid FrameworkId { get; private set; }
    public DateTime AdoptedDate { get; private set; }
    public DateTime? CertificationDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string CertificationBody { get; private set; }
    public string CertificateNumber { get; private set; }
    public ComplianceStatus Status { get; private set; }
    public int CompliancePercentage { get; private set; }

    protected OrganizationFramework() { }

    public OrganizationFramework(Guid id, Guid organizationId, Guid frameworkId, DateTime adoptedDate)
        : base(id)
    {
        OrganizationId = organizationId;
        FrameworkId = frameworkId;
        AdoptedDate = adoptedDate;
        Status = ComplianceStatus.NotStarted;
    }

    public void SetCertification(DateTime certDate, DateTime expiryDate, string body, string certNumber)
    {
        CertificationDate = certDate;
        ExpiryDate = expiryDate;
        CertificationBody = body;
        CertificateNumber = certNumber;
        Status = ComplianceStatus.Compliant;
    }

    public void UpdateCompliance(int percentage, ComplianceStatus status)
    {
        CompliancePercentage = percentage;
        Status = status;
    }
}
