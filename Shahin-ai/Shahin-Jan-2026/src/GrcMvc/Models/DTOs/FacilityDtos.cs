using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrcMvc.Models.DTOs
{
    /// <summary>
    /// DTO for facility listing
    /// </summary>
    public class FacilityListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? FacilityCode { get; set; }
        public string FacilityType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? SecurityLevel { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerEmail { get; set; }
        public int? Capacity { get; set; }
        public int? CurrentOccupancy { get; set; }
        public DateTime? LastInspectionDate { get; set; }
        public DateTime? NextInspectionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? BusinessCode { get; set; }
    }

    /// <summary>
    /// DTO for facility details
    /// </summary>
    public class FacilityDto
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }

        // Basic Information
        public string Name { get; set; } = string.Empty;
        public string? FacilityCode { get; set; }
        public string FacilityType { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public string? Description { get; set; }

        // Location
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? Region { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? TimeZone { get; set; }

        // Facility Details
        public decimal? Area { get; set; }
        public int? Floors { get; set; }
        public int? Capacity { get; set; }
        public int? CurrentOccupancy { get; set; }
        public bool Is24x7 { get; set; }
        public string? OperatingHours { get; set; }

        // Management
        public string? ManagerName { get; set; }
        public string? ManagerEmail { get; set; }
        public string? ManagerPhone { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? SecurityContactName { get; set; }
        public string? SecurityContactPhone { get; set; }

        // Security & Compliance
        public string? SecurityLevel { get; set; }
        public List<string>? Certifications { get; set; }
        public List<string>? ComplianceFrameworks { get; set; }
        public bool HasAccessControl { get; set; }
        public bool HasSurveillance { get; set; }
        public bool HasFireSuppression { get; set; }
        public bool HasBackupPower { get; set; }
        public bool HasEnvironmentalMonitoring { get; set; }

        // Financial
        public string? OwnershipType { get; set; }
        public DateTime? LeaseExpirationDate { get; set; }
        public decimal? MonthlyCost { get; set; }
        public decimal? AnnualBudget { get; set; }
        public string? CostCenterCode { get; set; }

        // Audit
        public DateTime? OpenedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? LastInspectionDate { get; set; }
        public DateTime? NextInspectionDate { get; set; }
        public DateTime? LastAuditDate { get; set; }
        public DateTime? NextAuditDate { get; set; }
        public string? Notes { get; set; }

        // Hierarchy
        public Guid? ParentFacilityId { get; set; }
        public string? ParentFacilityName { get; set; }

        // Metadata
        public string? BusinessCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    /// <summary>
    /// DTO for creating a facility
    /// </summary>
    public class CreateFacilityDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? FacilityCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string FacilityType { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Location
        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [MaxLength(100)]
        public string? TimeZone { get; set; }

        // Details
        public decimal? Area { get; set; }
        public int? Floors { get; set; }
        public int? Capacity { get; set; }
        public int? CurrentOccupancy { get; set; }
        public bool Is24x7 { get; set; }

        [MaxLength(200)]
        public string? OperatingHours { get; set; }

        // Management
        [MaxLength(200)]
        public string? ManagerName { get; set; }

        [MaxLength(200)]
        [EmailAddress]
        public string? ManagerEmail { get; set; }

        [MaxLength(50)]
        public string? ManagerPhone { get; set; }

        [MaxLength(200)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(50)]
        public string? EmergencyContactPhone { get; set; }

        [MaxLength(200)]
        public string? SecurityContactName { get; set; }

        [MaxLength(50)]
        public string? SecurityContactPhone { get; set; }

        // Security
        [MaxLength(50)]
        public string? SecurityLevel { get; set; }

        public List<string>? Certifications { get; set; }
        public List<string>? ComplianceFrameworks { get; set; }
        public bool HasAccessControl { get; set; }
        public bool HasSurveillance { get; set; }
        public bool HasFireSuppression { get; set; }
        public bool HasBackupPower { get; set; }
        public bool HasEnvironmentalMonitoring { get; set; }

        // Financial
        [MaxLength(50)]
        public string? OwnershipType { get; set; }

        public DateTime? LeaseExpirationDate { get; set; }
        public decimal? MonthlyCost { get; set; }
        public decimal? AnnualBudget { get; set; }

        [MaxLength(100)]
        public string? CostCenterCode { get; set; }

        // Dates
        public DateTime? OpenedDate { get; set; }
        public DateTime? NextInspectionDate { get; set; }
        public DateTime? NextAuditDate { get; set; }

        public string? Notes { get; set; }

        // Hierarchy
        public Guid? ParentFacilityId { get; set; }
    }

    /// <summary>
    /// DTO for updating a facility
    /// </summary>
    public class UpdateFacilityDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? FacilityCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string FacilityType { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Location
        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [MaxLength(100)]
        public string? TimeZone { get; set; }

        // Details
        public decimal? Area { get; set; }
        public int? Floors { get; set; }
        public int? Capacity { get; set; }
        public int? CurrentOccupancy { get; set; }
        public bool Is24x7 { get; set; }

        [MaxLength(200)]
        public string? OperatingHours { get; set; }

        // Management
        [MaxLength(200)]
        public string? ManagerName { get; set; }

        [MaxLength(200)]
        [EmailAddress]
        public string? ManagerEmail { get; set; }

        [MaxLength(50)]
        public string? ManagerPhone { get; set; }

        [MaxLength(200)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(50)]
        public string? EmergencyContactPhone { get; set; }

        [MaxLength(200)]
        public string? SecurityContactName { get; set; }

        [MaxLength(50)]
        public string? SecurityContactPhone { get; set; }

        // Security
        [MaxLength(50)]
        public string? SecurityLevel { get; set; }

        public List<string>? Certifications { get; set; }
        public List<string>? ComplianceFrameworks { get; set; }
        public bool HasAccessControl { get; set; }
        public bool HasSurveillance { get; set; }
        public bool HasFireSuppression { get; set; }
        public bool HasBackupPower { get; set; }
        public bool HasEnvironmentalMonitoring { get; set; }

        // Financial
        [MaxLength(50)]
        public string? OwnershipType { get; set; }

        public DateTime? LeaseExpirationDate { get; set; }
        public decimal? MonthlyCost { get; set; }
        public decimal? AnnualBudget { get; set; }

        [MaxLength(100)]
        public string? CostCenterCode { get; set; }

        // Dates
        public DateTime? OpenedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? LastInspectionDate { get; set; }
        public DateTime? NextInspectionDate { get; set; }
        public DateTime? LastAuditDate { get; set; }
        public DateTime? NextAuditDate { get; set; }

        public string? Notes { get; set; }

        // Hierarchy
        public Guid? ParentFacilityId { get; set; }
    }

    /// <summary>
    /// DTO for facility statistics
    /// </summary>
    public class FacilityStatsDto
    {
        public int TotalFacilities { get; set; }
        public int ActiveFacilities { get; set; }
        public int InactiveFacilities { get; set; }
        public int FacilitiesByType { get; set; }
        public decimal TotalArea { get; set; }
        public int TotalCapacity { get; set; }
        public int TotalOccupancy { get; set; }
        public decimal TotalMonthlyCost { get; set; }
        public int FacilitiesDueForInspection { get; set; }
        public int FacilitiesDueForAudit { get; set; }
        public Dictionary<string, int> FacilitiesByCountry { get; set; } = new();
        public Dictionary<string, int> FacilitiesBySecurityLevel { get; set; } = new();
        public Dictionary<string, int> FacilitiesByOwnership { get; set; } = new();
    }

    /// <summary>
    /// DTO for facility summary card
    /// </summary>
    public class FacilitySummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FacilityType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty; // City, Country
        public string? ManagerName { get; set; }
        public int? Capacity { get; set; }
        public int? CurrentOccupancy { get; set; }
        public decimal OccupancyRate { get; set; }
        public bool IsInspectionDue { get; set; }
        public bool IsAuditDue { get; set; }
        public string? SecurityLevel { get; set; }
    }
}
