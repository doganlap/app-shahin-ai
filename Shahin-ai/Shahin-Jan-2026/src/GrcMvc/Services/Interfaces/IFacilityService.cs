using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrcMvc.Models.DTOs;
using GrcMvc.Models.Entities;

namespace GrcMvc.Services.Interfaces
{
    /// <summary>
    /// Service interface for facility management
    /// </summary>
    public interface IFacilityService
    {
        /// <summary>
        /// Get all facilities for a tenant
        /// </summary>
        Task<List<FacilityListDto>> GetFacilitiesAsync(Guid tenantId);

        /// <summary>
        /// Get all facilities (platform admin)
        /// </summary>
        Task<List<FacilityListDto>> GetAllFacilitiesAsync();

        /// <summary>
        /// Get facility by ID
        /// </summary>
        Task<FacilityDto?> GetFacilityByIdAsync(Guid id, Guid? tenantId = null);

        /// <summary>
        /// Create a new facility
        /// </summary>
        Task<FacilityDto> CreateFacilityAsync(CreateFacilityDto dto, Guid tenantId, string createdBy);

        /// <summary>
        /// Update an existing facility
        /// </summary>
        Task<FacilityDto> UpdateFacilityAsync(Guid id, UpdateFacilityDto dto, Guid tenantId, string modifiedBy);

        /// <summary>
        /// Delete a facility (soft delete)
        /// </summary>
        Task<bool> DeleteFacilityAsync(Guid id, Guid tenantId);

        /// <summary>
        /// Get facility statistics for a tenant
        /// </summary>
        Task<FacilityStatsDto> GetFacilityStatsAsync(Guid tenantId);

        /// <summary>
        /// Get facility statistics for platform (all tenants)
        /// </summary>
        Task<FacilityStatsDto> GetPlatformFacilityStatsAsync();

        /// <summary>
        /// Get facility summary cards
        /// </summary>
        Task<List<FacilitySummaryDto>> GetFacilitySummariesAsync(Guid tenantId);

        /// <summary>
        /// Get facilities by type
        /// </summary>
        Task<List<FacilityListDto>> GetFacilitiesByTypeAsync(Guid tenantId, string facilityType);

        /// <summary>
        /// Get facilities by status
        /// </summary>
        Task<List<FacilityListDto>> GetFacilitiesByStatusAsync(Guid tenantId, string status);

        /// <summary>
        /// Get facilities by country
        /// </summary>
        Task<List<FacilityListDto>> GetFacilitiesByCountryAsync(Guid tenantId, string country);

        /// <summary>
        /// Get facilities due for inspection
        /// </summary>
        Task<List<FacilityListDto>> GetFacilitiesDueForInspectionAsync(Guid tenantId);

        /// <summary>
        /// Get facilities due for audit
        /// </summary>
        Task<List<FacilityListDto>> GetFacilitiesDueForAuditAsync(Guid tenantId);
    }
}
