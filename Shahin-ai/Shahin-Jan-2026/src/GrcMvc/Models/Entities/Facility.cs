using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrcMvc.Models.Entities
{
    /// <summary>
    /// Represents a physical or virtual facility/location managed by a tenant
    /// Examples: Office buildings, data centers, warehouses, branches, remote sites
    /// </summary>
    public class Facility : BaseEntity
    {
        /// <summary>
        /// Facility name (e.g., "Riyadh Main Office", "Dubai Data Center")
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Facility code/identifier (e.g., "RYD-HQ", "DXB-DC01")
        /// </summary>
        [MaxLength(50)]
        public string? FacilityCode { get; set; }

        /// <summary>
        /// Type of facility
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FacilityType { get; set; } = string.Empty; // Office, DataCenter, Warehouse, Branch, Remote, Cloud, Hybrid

        /// <summary>
        /// Facility status
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active"; // Active, Inactive, UnderConstruction, Decommissioned

        /// <summary>
        /// Description of the facility
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        // =====================================================================
        // LOCATION INFORMATION
        // =====================================================================

        /// <summary>
        /// Street address
        /// </summary>
        [MaxLength(500)]
        public string? Address { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [MaxLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// State/Province
        /// </summary>
        [MaxLength(100)]
        public string? State { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        [MaxLength(100)]
        public string? Country { get; set; }

        /// <summary>
        /// Postal/ZIP code
        /// </summary>
        [MaxLength(20)]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Region (e.g., "Middle East", "North America")
        /// </summary>
        [MaxLength(100)]
        public string? Region { get; set; }

        /// <summary>
        /// Latitude for mapping
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Longitude for mapping
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Time zone (e.g., "Asia/Riyadh", "Asia/Dubai")
        /// </summary>
        [MaxLength(100)]
        public string? TimeZone { get; set; }

        // =====================================================================
        // FACILITY DETAILS
        // =====================================================================

        /// <summary>
        /// Total area in square meters
        /// </summary>
        public decimal? Area { get; set; }

        /// <summary>
        /// Number of floors
        /// </summary>
        public int? Floors { get; set; }

        /// <summary>
        /// Maximum capacity (people)
        /// </summary>
        public int? Capacity { get; set; }

        /// <summary>
        /// Current occupancy count
        /// </summary>
        public int? CurrentOccupancy { get; set; }

        /// <summary>
        /// Whether facility operates 24/7
        /// </summary>
        public bool Is24x7 { get; set; } = false;

        /// <summary>
        /// Operating hours (e.g., "8:00 AM - 6:00 PM")
        /// </summary>
        [MaxLength(200)]
        public string? OperatingHours { get; set; }

        // =====================================================================
        // MANAGEMENT & CONTACTS
        // =====================================================================

        /// <summary>
        /// Facility manager name
        /// </summary>
        [MaxLength(200)]
        public string? ManagerName { get; set; }

        /// <summary>
        /// Facility manager email
        /// </summary>
        [MaxLength(200)]
        public string? ManagerEmail { get; set; }

        /// <summary>
        /// Facility manager phone
        /// </summary>
        [MaxLength(50)]
        public string? ManagerPhone { get; set; }

        /// <summary>
        /// Emergency contact name
        /// </summary>
        [MaxLength(200)]
        public string? EmergencyContactName { get; set; }

        /// <summary>
        /// Emergency contact phone
        /// </summary>
        [MaxLength(50)]
        public string? EmergencyContactPhone { get; set; }

        /// <summary>
        /// Security contact name
        /// </summary>
        [MaxLength(200)]
        public string? SecurityContactName { get; set; }

        /// <summary>
        /// Security contact phone
        /// </summary>
        [MaxLength(50)]
        public string? SecurityContactPhone { get; set; }

        // =====================================================================
        // COMPLIANCE & SECURITY
        // =====================================================================

        /// <summary>
        /// Security level (Low, Medium, High, Critical)
        /// </summary>
        [MaxLength(50)]
        public string? SecurityLevel { get; set; }

        /// <summary>
        /// Certifications held (JSON array: ["ISO 27001", "SOC 2", "PCI DSS"])
        /// </summary>
        public string? CertificationsJson { get; set; }

        /// <summary>
        /// Compliance frameworks applicable to this facility
        /// </summary>
        public string? ComplianceFrameworksJson { get; set; }

        /// <summary>
        /// Whether facility has access control systems
        /// </summary>
        public bool HasAccessControl { get; set; } = false;

        /// <summary>
        /// Whether facility has surveillance/CCTV
        /// </summary>
        public bool HasSurveillance { get; set; } = false;

        /// <summary>
        /// Whether facility has fire suppression systems
        /// </summary>
        public bool HasFireSuppression { get; set; } = false;

        /// <summary>
        /// Whether facility has backup power/UPS
        /// </summary>
        public bool HasBackupPower { get; set; } = false;

        /// <summary>
        /// Whether facility has environmental monitoring
        /// </summary>
        public bool HasEnvironmentalMonitoring { get; set; } = false;

        // =====================================================================
        // FINANCIAL & OPERATIONAL
        // =====================================================================

        /// <summary>
        /// Ownership type (Owned, Leased, Shared, Cloud)
        /// </summary>
        [MaxLength(50)]
        public string? OwnershipType { get; set; }

        /// <summary>
        /// Lease expiration date (if leased)
        /// </summary>
        public DateTime? LeaseExpirationDate { get; set; }

        /// <summary>
        /// Monthly operational cost
        /// </summary>
        public decimal? MonthlyCost { get; set; }

        /// <summary>
        /// Annual budget allocated
        /// </summary>
        public decimal? AnnualBudget { get; set; }

        /// <summary>
        /// Cost center code for accounting
        /// </summary>
        [MaxLength(100)]
        public string? CostCenterCode { get; set; }

        // =====================================================================
        // AUDIT & TRACKING
        // =====================================================================

        /// <summary>
        /// Date facility was opened/commissioned
        /// </summary>
        public DateTime? OpenedDate { get; set; }

        /// <summary>
        /// Date facility was closed/decommissioned
        /// </summary>
        public DateTime? ClosedDate { get; set; }

        /// <summary>
        /// Last inspection date
        /// </summary>
        public DateTime? LastInspectionDate { get; set; }

        /// <summary>
        /// Next scheduled inspection date
        /// </summary>
        public DateTime? NextInspectionDate { get; set; }

        /// <summary>
        /// Last audit date
        /// </summary>
        public DateTime? LastAuditDate { get; set; }

        /// <summary>
        /// Next scheduled audit date
        /// </summary>
        public DateTime? NextAuditDate { get; set; }

        /// <summary>
        /// Notes and additional information
        /// </summary>
        public string? Notes { get; set; }

        // =====================================================================
        // RELATIONSHIPS
        // =====================================================================

        /// <summary>
        /// Parent facility ID (for multi-level hierarchy)
        /// </summary>
        public Guid? ParentFacilityId { get; set; }

        /// <summary>
        /// Parent facility (navigation property)
        /// </summary>
        public virtual Facility? ParentFacility { get; set; }

        /// <summary>
        /// Child facilities (navigation property)
        /// </summary>
        public virtual ICollection<Facility> ChildFacilities { get; set; } = new List<Facility>();

        /// <summary>
        /// Assets located in this facility
        /// </summary>
        public virtual ICollection<Asset>? Assets { get; set; }

        /// <summary>
        /// Risks associated with this facility
        /// </summary>
        public virtual ICollection<Risk>? Risks { get; set; }

        /// <summary>
        /// Users assigned to this facility
        /// </summary>
        public virtual ICollection<TenantUser>? AssignedUsers { get; set; }
    }

    /// <summary>
    /// Predefined facility types
    /// </summary>
    public static class FacilityTypes
    {
        public const string Office = "Office";
        public const string DataCenter = "DataCenter";
        public const string Warehouse = "Warehouse";
        public const string Branch = "Branch";
        public const string RemoteSite = "RemoteSite";
        public const string CloudEnvironment = "CloudEnvironment";
        public const string HybridEnvironment = "HybridEnvironment";
        public const string ManufacturingPlant = "ManufacturingPlant";
        public const string RetailStore = "RetailStore";
        public const string Laboratory = "Laboratory";
        public const string Hospital = "Hospital";
        public const string Other = "Other";
    }

    /// <summary>
    /// Facility status values
    /// </summary>
    public static class FacilityStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string UnderConstruction = "UnderConstruction";
        public const string Decommissioned = "Decommissioned";
        public const string Maintenance = "Maintenance";
        public const string Suspended = "Suspended";
    }

    /// <summary>
    /// Security level values
    /// </summary>
    public static class SecurityLevels
    {
        public const string Low = "Low";
        public const string Medium = "Medium";
        public const string High = "High";
        public const string Critical = "Critical";
    }

    /// <summary>
    /// Ownership types
    /// </summary>
    public static class OwnershipTypes
    {
        public const string Owned = "Owned";
        public const string Leased = "Leased";
        public const string Shared = "Shared";
        public const string Cloud = "Cloud";
    }
}
