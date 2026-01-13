using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GrcMvc.Data;
using GrcMvc.Models.DTOs;
using GrcMvc.Models.Entities;
using GrcMvc.Services.Interfaces;

namespace GrcMvc.Services.Implementations
{
    /// <summary>
    /// Service implementation for facility management
    /// </summary>
    public class FacilityService : IFacilityService
    {
        private readonly GrcDbContext _context;
        private readonly ILogger<FacilityService> _logger;

        public FacilityService(GrcDbContext context, ILogger<FacilityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<FacilityListDto>> GetFacilitiesAsync(Guid tenantId)
        {
            return await _context.Set<Facility>()
                .Where(f => f.TenantId == tenantId && !f.IsDeleted)
                .OrderBy(f => f.Name)
                .Select(f => MapToListDto(f))
                .ToListAsync();
        }

        public async Task<List<FacilityListDto>> GetAllFacilitiesAsync()
        {
            return await _context.Set<Facility>()
                .Where(f => !f.IsDeleted)
                .OrderBy(f => f.Name)
                .Select(f => MapToListDto(f))
                .ToListAsync();
        }

        public async Task<FacilityDto?> GetFacilityByIdAsync(Guid id, Guid? tenantId = null)
        {
            var query = _context.Set<Facility>()
                .Where(f => f.Id == id && !f.IsDeleted);

            if (tenantId.HasValue)
            {
                query = query.Where(f => f.TenantId == tenantId.Value);
            }

            var facility = await query
                .Include(f => f.ParentFacility)
                .FirstOrDefaultAsync();

            return facility == null ? null : MapToDto(facility);
        }

        public async Task<FacilityDto> CreateFacilityAsync(CreateFacilityDto dto, Guid tenantId, string createdBy)
        {
            var facility = new Facility
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = dto.Name,
                FacilityCode = dto.FacilityCode,
                FacilityType = dto.FacilityType,
                Status = FacilityStatus.Active,
                Description = dto.Description,

                // Location
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Region = dto.Region,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                TimeZone = dto.TimeZone,

                // Details
                Area = dto.Area,
                Floors = dto.Floors,
                Capacity = dto.Capacity,
                CurrentOccupancy = dto.CurrentOccupancy,
                Is24x7 = dto.Is24x7,
                OperatingHours = dto.OperatingHours,

                // Management
                ManagerName = dto.ManagerName,
                ManagerEmail = dto.ManagerEmail,
                ManagerPhone = dto.ManagerPhone,
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyContactPhone = dto.EmergencyContactPhone,
                SecurityContactName = dto.SecurityContactName,
                SecurityContactPhone = dto.SecurityContactPhone,

                // Security
                SecurityLevel = dto.SecurityLevel,
                CertificationsJson = dto.Certifications != null ? JsonSerializer.Serialize(dto.Certifications) : null,
                ComplianceFrameworksJson = dto.ComplianceFrameworks != null ? JsonSerializer.Serialize(dto.ComplianceFrameworks) : null,
                HasAccessControl = dto.HasAccessControl,
                HasSurveillance = dto.HasSurveillance,
                HasFireSuppression = dto.HasFireSuppression,
                HasBackupPower = dto.HasBackupPower,
                HasEnvironmentalMonitoring = dto.HasEnvironmentalMonitoring,

                // Financial
                OwnershipType = dto.OwnershipType,
                LeaseExpirationDate = dto.LeaseExpirationDate,
                MonthlyCost = dto.MonthlyCost,
                AnnualBudget = dto.AnnualBudget,
                CostCenterCode = dto.CostCenterCode,

                // Dates
                OpenedDate = dto.OpenedDate,
                NextInspectionDate = dto.NextInspectionDate,
                NextAuditDate = dto.NextAuditDate,
                Notes = dto.Notes,

                // Hierarchy
                ParentFacilityId = dto.ParentFacilityId,

                // Audit
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            _context.Set<Facility>().Add(facility);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Facility created: {FacilityId} - {FacilityName} for tenant {TenantId}",
                facility.Id, facility.Name, tenantId);

            return MapToDto(facility);
        }

        public async Task<FacilityDto> UpdateFacilityAsync(Guid id, UpdateFacilityDto dto, Guid tenantId, string modifiedBy)
        {
            var facility = await _context.Set<Facility>()
                .FirstOrDefaultAsync(f => f.Id == id && f.TenantId == tenantId && !f.IsDeleted);

            if (facility == null)
            {
                throw new InvalidOperationException($"Facility {id} not found for tenant {tenantId}");
            }

            // Update properties
            facility.Name = dto.Name;
            facility.FacilityCode = dto.FacilityCode;
            facility.FacilityType = dto.FacilityType;
            facility.Status = dto.Status;
            facility.Description = dto.Description;

            // Location
            facility.Address = dto.Address;
            facility.City = dto.City;
            facility.State = dto.State;
            facility.Country = dto.Country;
            facility.PostalCode = dto.PostalCode;
            facility.Region = dto.Region;
            facility.Latitude = dto.Latitude;
            facility.Longitude = dto.Longitude;
            facility.TimeZone = dto.TimeZone;

            // Details
            facility.Area = dto.Area;
            facility.Floors = dto.Floors;
            facility.Capacity = dto.Capacity;
            facility.CurrentOccupancy = dto.CurrentOccupancy;
            facility.Is24x7 = dto.Is24x7;
            facility.OperatingHours = dto.OperatingHours;

            // Management
            facility.ManagerName = dto.ManagerName;
            facility.ManagerEmail = dto.ManagerEmail;
            facility.ManagerPhone = dto.ManagerPhone;
            facility.EmergencyContactName = dto.EmergencyContactName;
            facility.EmergencyContactPhone = dto.EmergencyContactPhone;
            facility.SecurityContactName = dto.SecurityContactName;
            facility.SecurityContactPhone = dto.SecurityContactPhone;

            // Security
            facility.SecurityLevel = dto.SecurityLevel;
            facility.CertificationsJson = dto.Certifications != null ? JsonSerializer.Serialize(dto.Certifications) : null;
            facility.ComplianceFrameworksJson = dto.ComplianceFrameworks != null ? JsonSerializer.Serialize(dto.ComplianceFrameworks) : null;
            facility.HasAccessControl = dto.HasAccessControl;
            facility.HasSurveillance = dto.HasSurveillance;
            facility.HasFireSuppression = dto.HasFireSuppression;
            facility.HasBackupPower = dto.HasBackupPower;
            facility.HasEnvironmentalMonitoring = dto.HasEnvironmentalMonitoring;

            // Financial
            facility.OwnershipType = dto.OwnershipType;
            facility.LeaseExpirationDate = dto.LeaseExpirationDate;
            facility.MonthlyCost = dto.MonthlyCost;
            facility.AnnualBudget = dto.AnnualBudget;
            facility.CostCenterCode = dto.CostCenterCode;

            // Dates
            facility.OpenedDate = dto.OpenedDate;
            facility.ClosedDate = dto.ClosedDate;
            facility.LastInspectionDate = dto.LastInspectionDate;
            facility.NextInspectionDate = dto.NextInspectionDate;
            facility.LastAuditDate = dto.LastAuditDate;
            facility.NextAuditDate = dto.NextAuditDate;
            facility.Notes = dto.Notes;

            // Hierarchy
            facility.ParentFacilityId = dto.ParentFacilityId;

            // Audit
            facility.ModifiedDate = DateTime.UtcNow;
            facility.ModifiedBy = modifiedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Facility updated: {FacilityId} - {FacilityName}", facility.Id, facility.Name);

            return MapToDto(facility);
        }

        public async Task<bool> DeleteFacilityAsync(Guid id, Guid tenantId)
        {
            var facility = await _context.Set<Facility>()
                .FirstOrDefaultAsync(f => f.Id == id && f.TenantId == tenantId && !f.IsDeleted);

            if (facility == null)
            {
                return false;
            }

            facility.IsDeleted = true;
            facility.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Facility deleted: {FacilityId} - {FacilityName}", facility.Id, facility.Name);

            return true;
        }

        public async Task<FacilityStatsDto> GetFacilityStatsAsync(Guid tenantId)
        {
            var facilities = await _context.Set<Facility>()
                .Where(f => f.TenantId == tenantId && !f.IsDeleted)
                .ToListAsync();

            return CalculateStats(facilities);
        }

        public async Task<FacilityStatsDto> GetPlatformFacilityStatsAsync()
        {
            var facilities = await _context.Set<Facility>()
                .Where(f => !f.IsDeleted)
                .ToListAsync();

            return CalculateStats(facilities);
        }

        public async Task<List<FacilitySummaryDto>> GetFacilitySummariesAsync(Guid tenantId)
        {
            var today = DateTime.UtcNow;

            return await _context.Set<Facility>()
                .Where(f => f.TenantId == tenantId && !f.IsDeleted)
                .OrderBy(f => f.Name)
                .Select(f => new FacilitySummaryDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    FacilityType = f.FacilityType,
                    Status = f.Status,
                    Location = f.City != null && f.Country != null ? $"{f.City}, {f.Country}" : f.City ?? f.Country ?? "N/A",
                    ManagerName = f.ManagerName,
                    Capacity = f.Capacity,
                    CurrentOccupancy = f.CurrentOccupancy,
                    OccupancyRate = f.Capacity.HasValue && f.Capacity.Value > 0 && f.CurrentOccupancy.HasValue
                        ? (decimal)f.CurrentOccupancy.Value / f.Capacity.Value * 100
                        : 0,
                    IsInspectionDue = f.NextInspectionDate.HasValue && f.NextInspectionDate.Value <= today,
                    IsAuditDue = f.NextAuditDate.HasValue && f.NextAuditDate.Value <= today,
                    SecurityLevel = f.SecurityLevel
                })
                .ToListAsync();
        }

        public async Task<List<FacilityListDto>> GetFacilitiesByTypeAsync(Guid tenantId, string facilityType)
        {
            return await _context.Set<Facility>()
                .Where(f => f.TenantId == tenantId && !f.IsDeleted && f.FacilityType == facilityType)
                .OrderBy(f => f.Name)
                .Select(f => MapToListDto(f))
                .ToListAsync();
        }

        public async Task<List<FacilityListDto>> GetFacilitiesByStatusAsync(Guid tenantId, string status)
        {
            return await _context.Set<Facility>()
                .Where(f => f.TenantId == tenantId && !f.IsDeleted && f.Status == status)
                .OrderBy(f => f.Name)
                .Select(f => MapToListDto(f))
                .ToListAsync();
        }

        public async Task<List<FacilityListDto>> GetFacilitiesByCountryAsync(Guid tenantId, string country)
        {
            return await _context.Set<Facility>()
                .Where(f => f.TenantId == tenantId && !f.IsDeleted && f.Country == country)
                .OrderBy(f => f.Name)
                .Select(f => MapToListDto(f))
                .ToListAsync();
        }

        public async Task<List<FacilityListDto>> GetFacilitiesDueForInspectionAsync(Guid tenantId)
        {
            var today = DateTime.UtcNow;
            return await _context.Set<Facility>()
                .Where(f => f.TenantId == tenantId && !f.IsDeleted &&
                           f.NextInspectionDate.HasValue && f.NextInspectionDate.Value <= today)
                .OrderBy(f => f.NextInspectionDate)
                .Select(f => MapToListDto(f))
                .ToListAsync();
        }

        public async Task<List<FacilityListDto>> GetFacilitiesDueForAuditAsync(Guid tenantId)
        {
            var today = DateTime.UtcNow;
            return await _context.Set<Facility>()
                .Where(f => f.TenantId == tenantId && !f.IsDeleted &&
                           f.NextAuditDate.HasValue && f.NextAuditDate.Value <= today)
                .OrderBy(f => f.NextAuditDate)
                .Select(f => MapToListDto(f))
                .ToListAsync();
        }

        // Helper methods
        private static FacilityListDto MapToListDto(Facility f)
        {
            return new FacilityListDto
            {
                Id = f.Id,
                Name = f.Name,
                FacilityCode = f.FacilityCode,
                FacilityType = f.FacilityType,
                Status = f.Status,
                City = f.City,
                Country = f.Country,
                Region = f.Region,
                SecurityLevel = f.SecurityLevel,
                ManagerName = f.ManagerName,
                ManagerEmail = f.ManagerEmail,
                Capacity = f.Capacity,
                CurrentOccupancy = f.CurrentOccupancy,
                LastInspectionDate = f.LastInspectionDate,
                NextInspectionDate = f.NextInspectionDate,
                CreatedDate = f.CreatedDate,
                BusinessCode = f.BusinessCode
            };
        }

        private static FacilityDto MapToDto(Facility facility)
        {
            return new FacilityDto
            {
                Id = facility.Id,
                TenantId = facility.TenantId,
                Name = facility.Name,
                FacilityCode = facility.FacilityCode,
                FacilityType = facility.FacilityType,
                Status = facility.Status,
                Description = facility.Description,

                // Location
                Address = facility.Address,
                City = facility.City,
                State = facility.State,
                Country = facility.Country,
                PostalCode = facility.PostalCode,
                Region = facility.Region,
                Latitude = facility.Latitude,
                Longitude = facility.Longitude,
                TimeZone = facility.TimeZone,

                // Details
                Area = facility.Area,
                Floors = facility.Floors,
                Capacity = facility.Capacity,
                CurrentOccupancy = facility.CurrentOccupancy,
                Is24x7 = facility.Is24x7,
                OperatingHours = facility.OperatingHours,

                // Management
                ManagerName = facility.ManagerName,
                ManagerEmail = facility.ManagerEmail,
                ManagerPhone = facility.ManagerPhone,
                EmergencyContactName = facility.EmergencyContactName,
                EmergencyContactPhone = facility.EmergencyContactPhone,
                SecurityContactName = facility.SecurityContactName,
                SecurityContactPhone = facility.SecurityContactPhone,

                // Security
                SecurityLevel = facility.SecurityLevel,
                Certifications = string.IsNullOrEmpty(facility.CertificationsJson)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(facility.CertificationsJson),
                ComplianceFrameworks = string.IsNullOrEmpty(facility.ComplianceFrameworksJson)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(facility.ComplianceFrameworksJson),
                HasAccessControl = facility.HasAccessControl,
                HasSurveillance = facility.HasSurveillance,
                HasFireSuppression = facility.HasFireSuppression,
                HasBackupPower = facility.HasBackupPower,
                HasEnvironmentalMonitoring = facility.HasEnvironmentalMonitoring,

                // Financial
                OwnershipType = facility.OwnershipType,
                LeaseExpirationDate = facility.LeaseExpirationDate,
                MonthlyCost = facility.MonthlyCost,
                AnnualBudget = facility.AnnualBudget,
                CostCenterCode = facility.CostCenterCode,

                // Dates
                OpenedDate = facility.OpenedDate,
                ClosedDate = facility.ClosedDate,
                LastInspectionDate = facility.LastInspectionDate,
                NextInspectionDate = facility.NextInspectionDate,
                LastAuditDate = facility.LastAuditDate,
                NextAuditDate = facility.NextAuditDate,
                Notes = facility.Notes,

                // Hierarchy
                ParentFacilityId = facility.ParentFacilityId,
                ParentFacilityName = facility.ParentFacility?.Name,

                // Metadata
                BusinessCode = facility.BusinessCode,
                CreatedDate = facility.CreatedDate,
                ModifiedDate = facility.ModifiedDate,
                CreatedBy = facility.CreatedBy,
                ModifiedBy = facility.ModifiedBy
            };
        }

        private static FacilityStatsDto CalculateStats(List<Facility> facilities)
        {
            var today = DateTime.UtcNow;

            return new FacilityStatsDto
            {
                TotalFacilities = facilities.Count,
                ActiveFacilities = facilities.Count(f => f.Status == FacilityStatus.Active),
                InactiveFacilities = facilities.Count(f => f.Status == FacilityStatus.Inactive),
                TotalArea = facilities.Sum(f => f.Area ?? 0),
                TotalCapacity = facilities.Sum(f => f.Capacity ?? 0),
                TotalOccupancy = facilities.Sum(f => f.CurrentOccupancy ?? 0),
                TotalMonthlyCost = facilities.Sum(f => f.MonthlyCost ?? 0),
                FacilitiesDueForInspection = facilities.Count(f => f.NextInspectionDate.HasValue && f.NextInspectionDate.Value <= today),
                FacilitiesDueForAudit = facilities.Count(f => f.NextAuditDate.HasValue && f.NextAuditDate.Value <= today),
                FacilitiesByCountry = facilities
                    .Where(f => !string.IsNullOrEmpty(f.Country))
                    .GroupBy(f => f.Country!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                FacilitiesBySecurityLevel = facilities
                    .Where(f => !string.IsNullOrEmpty(f.SecurityLevel))
                    .GroupBy(f => f.SecurityLevel!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                FacilitiesByOwnership = facilities
                    .Where(f => !string.IsNullOrEmpty(f.OwnershipType))
                    .GroupBy(f => f.OwnershipType!)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
    }
}
