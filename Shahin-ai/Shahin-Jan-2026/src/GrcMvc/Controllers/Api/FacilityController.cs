using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GrcMvc.Models.DTOs;
using GrcMvc.Services.Interfaces;

namespace GrcMvc.Controllers.Api
{
    /// <summary>
    /// API Controller for Facility Management
    /// Accessible to both Platform Admins and Tenant Admins
    /// </summary>
    [ApiController]
    [Route("api/facilities")]
    [Authorize]
    public class FacilityController : ControllerBase
    {
        private readonly IFacilityService _facilityService;
        private readonly ILogger<FacilityController> _logger;

        public FacilityController(IFacilityService facilityService, ILogger<FacilityController> logger)
        {
            _facilityService = facilityService;
            _logger = logger;
        }

        /// <summary>
        /// Get all facilities for current tenant (or all for platform admin)
        /// GET: api/facilities
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<FacilityListDto>>> GetFacilities([FromQuery] Guid? tenantId = null)
        {
            try
            {
                // Platform admin can view all facilities or filter by tenant
                if (User.IsInRole("PlatformAdmin") || User.IsInRole("Admin"))
                {
                    if (tenantId.HasValue)
                    {
                        var facilities = await _facilityService.GetFacilitiesAsync(tenantId.Value);
                        return Ok(facilities);
                    }
                    else
                    {
                        var allFacilities = await _facilityService.GetAllFacilitiesAsync();
                        return Ok(allFacilities);
                    }
                }

                // Tenant users can only view their own facilities
                var userTenantId = GetCurrentTenantId();
                if (!userTenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var tenantFacilities = await _facilityService.GetFacilitiesAsync(userTenantId.Value);
                return Ok(tenantFacilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving facilities");
                return StatusCode(500, "Error retrieving facilities");
            }
        }

        /// <summary>
        /// Get facility by ID
        /// GET: api/facilities/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityDto>> GetFacility(Guid id)
        {
            try
            {
                Guid? tenantId = null;

                // Non-platform admins can only access their own tenant's facilities
                if (!User.IsInRole("PlatformAdmin") && !User.IsInRole("Admin"))
                {
                    tenantId = GetCurrentTenantId();
                    if (!tenantId.HasValue)
                    {
                        return BadRequest("Tenant ID not found for current user");
                    }
                }

                var facility = await _facilityService.GetFacilityByIdAsync(id, tenantId);

                if (facility == null)
                {
                    return NotFound($"Facility {id} not found");
                }

                return Ok(facility);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving facility {FacilityId}", id);
                return StatusCode(500, "Error retrieving facility");
            }
        }

        /// <summary>
        /// Create a new facility
        /// POST: api/facilities
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<FacilityDto>> CreateFacility([FromBody] CreateFacilityDto dto)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                if (!tenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

                var facility = await _facilityService.CreateFacilityAsync(dto, tenantId.Value, userName);

                return CreatedAtAction(nameof(GetFacility), new { id = facility.Id }, facility);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating facility");
                return StatusCode(500, "Error creating facility");
            }
        }

        /// <summary>
        /// Update an existing facility
        /// PUT: api/facilities/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<FacilityDto>> UpdateFacility(Guid id, [FromBody] UpdateFacilityDto dto)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                if (!tenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

                var facility = await _facilityService.UpdateFacilityAsync(id, dto, tenantId.Value, userName);

                return Ok(facility);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Facility not found for update: {FacilityId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating facility {FacilityId}", id);
                return StatusCode(500, "Error updating facility");
            }
        }

        /// <summary>
        /// Delete a facility
        /// DELETE: api/facilities/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFacility(Guid id)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                if (!tenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var result = await _facilityService.DeleteFacilityAsync(id, tenantId.Value);

                if (!result)
                {
                    return NotFound($"Facility {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting facility {FacilityId}", id);
                return StatusCode(500, "Error deleting facility");
            }
        }

        /// <summary>
        /// Get facility statistics
        /// GET: api/facilities/stats
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<FacilityStatsDto>> GetFacilityStats([FromQuery] Guid? tenantId = null)
        {
            try
            {
                // Platform admin can view stats for all or specific tenant
                if (User.IsInRole("PlatformAdmin") || User.IsInRole("Admin"))
                {
                    if (tenantId.HasValue)
                    {
                        var stats = await _facilityService.GetFacilityStatsAsync(tenantId.Value);
                        return Ok(stats);
                    }
                    else
                    {
                        var platformStats = await _facilityService.GetPlatformFacilityStatsAsync();
                        return Ok(platformStats);
                    }
                }

                // Tenant users can only view their own stats
                var userTenantId = GetCurrentTenantId();
                if (!userTenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var tenantStats = await _facilityService.GetFacilityStatsAsync(userTenantId.Value);
                return Ok(tenantStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving facility stats");
                return StatusCode(500, "Error retrieving facility stats");
            }
        }

        /// <summary>
        /// Get facility summaries
        /// GET: api/facilities/summaries
        /// </summary>
        [HttpGet("summaries")]
        public async Task<ActionResult<List<FacilitySummaryDto>>> GetFacilitySummaries([FromQuery] Guid? tenantId = null)
        {
            try
            {
                var effectiveTenantId = tenantId ?? GetCurrentTenantId();

                if (!effectiveTenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found");
                }

                var summaries = await _facilityService.GetFacilitySummariesAsync(effectiveTenantId.Value);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving facility summaries");
                return StatusCode(500, "Error retrieving facility summaries");
            }
        }

        /// <summary>
        /// Get facilities by type
        /// GET: api/facilities/by-type/{facilityType}
        /// </summary>
        [HttpGet("by-type/{facilityType}")]
        public async Task<ActionResult<List<FacilityListDto>>> GetFacilitiesByType(string facilityType)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                if (!tenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var facilities = await _facilityService.GetFacilitiesByTypeAsync(tenantId.Value, facilityType);
                return Ok(facilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving facilities by type {FacilityType}", facilityType);
                return StatusCode(500, "Error retrieving facilities");
            }
        }

        /// <summary>
        /// Get facilities by status
        /// GET: api/facilities/by-status/{status}
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<List<FacilityListDto>>> GetFacilitiesByStatus(string status)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                if (!tenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var facilities = await _facilityService.GetFacilitiesByStatusAsync(tenantId.Value, status);
                return Ok(facilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving facilities by status {Status}", status);
                return StatusCode(500, "Error retrieving facilities");
            }
        }

        /// <summary>
        /// Get facilities due for inspection
        /// GET: api/facilities/due-for-inspection
        /// </summary>
        [HttpGet("due-for-inspection")]
        public async Task<ActionResult<List<FacilityListDto>>> GetFacilitiesDueForInspection()
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                if (!tenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var facilities = await _facilityService.GetFacilitiesDueForInspectionAsync(tenantId.Value);
                return Ok(facilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving facilities due for inspection");
                return StatusCode(500, "Error retrieving facilities");
            }
        }

        /// <summary>
        /// Get facilities due for audit
        /// GET: api/facilities/due-for-audit
        /// </summary>
        [HttpGet("due-for-audit")]
        public async Task<ActionResult<List<FacilityListDto>>> GetFacilitiesDueForAudit()
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                if (!tenantId.HasValue)
                {
                    return BadRequest("Tenant ID not found for current user");
                }

                var facilities = await _facilityService.GetFacilitiesDueForAuditAsync(tenantId.Value);
                return Ok(facilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving facilities due for audit");
                return StatusCode(500, "Error retrieving facilities");
            }
        }

        // Helper method to get current tenant ID from claims
        private Guid? GetCurrentTenantId()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return tenantId;
            }
            return null;
        }
    }
}
