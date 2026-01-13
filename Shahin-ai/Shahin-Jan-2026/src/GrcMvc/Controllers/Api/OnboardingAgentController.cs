using System;
using System.Security;
using System.Threading.Tasks;
using GrcMvc.Models.DTOs;
using GrcMvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;

namespace GrcMvc.Controllers.Api
{
    /// <summary>
    /// API Controller for tenant creation via agents, bots, or automation
    /// Supports GPT/bot-triggered tenant creation, trial flows, and onboarding wizards
    /// </summary>
    [ApiController]
    [Route("api/agent/tenant")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [IgnoreAntiforgeryToken] // API endpoints don't require CSRF tokens
    [EnableRateLimiting("auth")] // Apply auth rate limiter: 5 requests per 5 minutes
    public class OnboardingAgentController : ControllerBase
    {
        private readonly ITenantCreationFacadeService _tenantCreationFacadeService;
        private readonly ILogger<OnboardingAgentController> _logger;

        public OnboardingAgentController(
            ITenantCreationFacadeService tenantCreationFacadeService,
            ILogger<OnboardingAgentController> logger)
        {
            _tenantCreationFacadeService = tenantCreationFacadeService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new tenant with admin user
        /// Used by GPT agents, bots, trial flows, and onboarding wizards
        /// </summary>
        /// <param name="dto">Tenant creation request</param>
        /// <returns>Tenant ID and creation details</returns>
        /// <response code="200">Tenant created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="409">Tenant or user already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("create")]
        [AllowAnonymous] // Allow unauthenticated access for trial registration
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantAgentDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { error = "Request body is required" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("OnboardingAgent: Creating tenant - Name={TenantName}, Email={AdminEmail}",
                    dto.TenantName, dto.AdminEmail);

                // Create tenant using facade service with security features
                var request = new TenantCreationFacadeRequest
                {
                    TenantName = dto.TenantName,
                    AdminEmail = dto.AdminEmail,
                    AdminPassword = dto.AdminPassword,
                    RecaptchaToken = dto.RecaptchaToken,
                    DeviceFingerprint = dto.DeviceFingerprint,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                };

                var result = await _tenantCreationFacadeService.CreateTenantWithAdminAsync(request);

                _logger.LogInformation("OnboardingAgent: Tenant created successfully - TenantId={TenantId}, Name={TenantName}, IsFlagged={IsFlagged}",
                    result.TenantId, result.TenantName, result.IsFlaggedForReview);

                var response = new CreateTenantResponseDto
                {
                    TenantId = result.TenantId,
                    TenantName = result.TenantName,
                    AdminEmail = result.AdminEmail,
                    Message = result.Message
                };

                return Ok(response);
            }
            catch (SecurityException sex)
            {
                // Security validation failed (CAPTCHA, fraud detection, rate limiting)
                _logger.LogWarning(sex, "OnboardingAgent: Security validation failed - Name={TenantName}, Email={AdminEmail}",
                    dto.TenantName, dto.AdminEmail);
                return BadRequest(new { error = $"Security validation failed: {sex.Message}" });
            }
            catch (InvalidOperationException iex)
            {
                // Business logic error (tenant exists, email exists, etc.)
                _logger.LogWarning(iex, "OnboardingAgent: Tenant creation failed - {Error}", iex.Message);
                return Conflict(new { error = iex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnboardingAgent: Unexpected error creating tenant - Name={TenantName}, Email={AdminEmail}",
                    dto.TenantName, dto.AdminEmail);
                return StatusCode(500, new { error = "An error occurred while creating the tenant. Please try again later." });
            }
        }
    }
}
