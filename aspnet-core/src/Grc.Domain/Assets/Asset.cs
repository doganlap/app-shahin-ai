using System;
using System.Collections.Generic;
using Grc.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Assets;

public class Asset : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid? OrganizationId { get; private set; }
    public Guid? OrganizationUnitId { get; private set; }
    public string AssetId { get; private set; }
    public string Name { get; private set; }
    public string NameAr { get; private set; }
    public string Description { get; private set; }
    public AssetType Type { get; private set; }
    public AssetCategory Category { get; private set; }
    public AssetClassification Classification { get; private set; }
    public AssetCriticality Criticality { get; private set; }
    public AssetStatus Status { get; private set; }
    public string Owner { get; private set; }
    public string OwnerEmail { get; private set; }
    public string Custodian { get; private set; }
    public string CustodianEmail { get; private set; }
    public string Location { get; private set; }
    public string Vendor { get; private set; }
    public string Model { get; private set; }
    public string SerialNumber { get; private set; }
    public string IpAddress { get; private set; }
    public string MacAddress { get; private set; }
    public DateTime? PurchaseDate { get; private set; }
    public decimal? PurchaseCost { get; private set; }
    public DateTime? WarrantyExpiry { get; private set; }
    public DateTime? EndOfLife { get; private set; }
    public string Notes { get; private set; }
    public int RiskScore { get; private set; }

    public virtual ICollection<AssetDependency> Dependencies { get; private set; }
    public virtual ICollection<AssetRisk> Risks { get; private set; }

    protected Asset() { }

    public Asset(
        Guid id,
        string assetId,
        string name,
        AssetType type,
        AssetCategory category,
        AssetClassification classification,
        AssetCriticality criticality,
        Guid? tenantId = null)
        : base(id)
    {
        AssetId = assetId;
        Name = name;
        Type = type;
        Category = category;
        Classification = classification;
        Criticality = criticality;
        TenantId = tenantId;
        Status = AssetStatus.Active;
        Dependencies = new List<AssetDependency>();
        Risks = new List<AssetRisk>();
    }

    public void SetOrganization(Guid organizationId, Guid? unitId = null)
    {
        OrganizationId = organizationId;
        OrganizationUnitId = unitId;
    }

    public void SetOwnership(string owner, string ownerEmail, string custodian, string custodianEmail)
    {
        Owner = owner;
        OwnerEmail = ownerEmail;
        Custodian = custodian;
        CustodianEmail = custodianEmail;
    }

    public void SetLocation(string location) => Location = location;

    public void SetTechnicalDetails(string vendor, string model, string serialNumber, string ipAddress, string macAddress)
    {
        Vendor = vendor;
        Model = model;
        SerialNumber = serialNumber;
        IpAddress = ipAddress;
        MacAddress = macAddress;
    }

    public void SetPurchaseInfo(DateTime purchaseDate, decimal cost, DateTime? warrantyExpiry, DateTime? endOfLife)
    {
        PurchaseDate = purchaseDate;
        PurchaseCost = cost;
        WarrantyExpiry = warrantyExpiry;
        EndOfLife = endOfLife;
    }

    public void UpdateRiskScore(int score) => RiskScore = score;
    public void SetStatus(AssetStatus status) => Status = status;
    public void SetDescription(string description, string descriptionAr = null)
    {
        Description = description;
        NameAr = descriptionAr;
    }
}

public class AssetDependency : FullAuditedEntity<Guid>
{
    public Guid AssetId { get; private set; }
    public Guid DependsOnAssetId { get; private set; }
    public string DependencyType { get; private set; }
    public string Description { get; private set; }
    public bool IsCritical { get; private set; }

    protected AssetDependency() { }

    public AssetDependency(Guid id, Guid assetId, Guid dependsOnAssetId, string dependencyType, bool isCritical = false)
        : base(id)
    {
        AssetId = assetId;
        DependsOnAssetId = dependsOnAssetId;
        DependencyType = dependencyType;
        IsCritical = isCritical;
    }
}

public class AssetRisk : FullAuditedEntity<Guid>
{
    public Guid AssetId { get; private set; }
    public Guid RiskId { get; private set; }
    public string ImpactDescription { get; private set; }
    public int ImpactLevel { get; private set; }

    protected AssetRisk() { }

    public AssetRisk(Guid id, Guid assetId, Guid riskId, int impactLevel)
        : base(id)
    {
        AssetId = assetId;
        RiskId = riskId;
        ImpactLevel = impactLevel;
    }
}
