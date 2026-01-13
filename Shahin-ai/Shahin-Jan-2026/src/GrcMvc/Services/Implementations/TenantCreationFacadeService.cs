using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using GrcMvc.Models.DTOs;
using GrcMvc.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Identity;
using AbpTenantDto = Volo.Abp.TenantManagement.TenantDto;

namespace GrcMvc.Services.Implementations
{
    /// <summary>
    /// Facade service that wraps ABP's ITenantAppService with security enhancements
    /// Provides unified tenant creation with CAPTCHA validation, fraud detection, and audit logging
    /// </summary>
    public class TenantCreationFacadeService : ITenantCreationFacadeService, ITransientDependency
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly ILogger<TenantCreationFacadeService> _logger;
        private readonly IRecaptchaValidationService _recaptchaService;
        private readonly IFingerprintFraudDetector _fraudDetector;
        private readonly Data.GrcDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public TenantCreationFacadeService(
            ITenantAppService tenantAppService,
            ITenantRepository tenantRepository,
            IIdentityUserRepository userRepository,
            ICurrentTenant currentTenant,
            ILogger<TenantCreationFacadeService> logger,
            IRecaptchaValidationService recaptchaService,
            IFingerprintFraudDetector fraudDetector,
            Data.GrcDbContext dbContext,
            IConfiguration configuration)
        {
            _tenantAppService = tenantAppService;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _currentTenant = currentTenant;
            _logger = logger;
            _recaptchaService = recaptchaService;
            _fraudDetector = fraudDetector;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Creates a new tenant with admin user using ABP's built-in ITenantAppService
        /// </summary>
        public async Task<TenantCreationFacadeResult> CreateTenantWithAdminAsync(TenantCreationFacadeRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation("TenantCreationFacade: Starting tenant creation - TenantName={TenantName}, Email={Email}, IP={IP}",
                request.TenantName, request.AdminEmail, request.IpAddress);

            try
            {
                // PHASE 1: Security Validations
                var recaptchaResult = await ValidateRecaptchaAsync(request);
                var fraudCheck = await ValidateFraudDetectionAsync(request);

                // PHASE 2: Create tenant using ABP's ITenantAppService
                var tenantDto = await CreateTenantViaAbpServiceAsync(request);

                // PHASE 3: Track fingerprint and retrieve created user
                await TrackTenantCreationFingerprintAsync(tenantDto.Id, request, recaptchaResult.Score, fraudCheck);
                var result = await BuildCreationResultAsync(tenantDto, request, fraudCheck);

                _logger.LogInformation("TenantCreationFacade: Tenant created successfully - TenantId={TenantId}, UserId={UserId}",
                    result.TenantId, result.AdminUserId);

                return result;
            }
            catch (SecurityException sex)
            {
                _logger.LogWarning(sex, "TenantCreationFacade: Security validation failed - TenantName={TenantName}, Email={Email}, Reason={Reason}",
                    request.TenantName, request.AdminEmail, sex.Message);
                throw;
            }
            catch (InvalidOperationException iex)
            {
                _logger.LogWarning(iex, "TenantCreationFacade: Business logic error - TenantName={TenantName}, Email={Email}, Error={Error}",
                    request.TenantName, request.AdminEmail, iex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TenantCreationFacade: Unexpected error creating tenant - TenantName={TenantName}, Email={Email}",
                    request.TenantName, request.AdminEmail);
                throw new InvalidOperationException($"Tenant creation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates tenant using ABP's built-in ITenantAppService
        /// This handles tenant creation, admin user creation, and role assignment automatically
        /// </summary>
        private async Task<AbpTenantDto> CreateTenantViaAbpServiceAsync(TenantCreationFacadeRequest request)
        {
            // Sanitize tenant name for ABP requirements
            var sanitizedTenantName = SanitizeTenantName(request.TenantName);

            // Check if tenant already exists
            using (_currentTenant.Change(null)) // Ensure host context
            {
                var existingTenant = await _tenantRepository.FindByNameAsync(sanitizedTenantName);
                if (existingTenant != null)
                {
                    // Append timestamp to make unique
                    sanitizedTenantName = $"{sanitizedTenantName}-{DateTime.UtcNow:HHmmss}";
                    _logger.LogWarning("TenantCreationFacade: Tenant name exists, using modified name: {TenantName}", sanitizedTenantName);
                }
            }

            // Create tenant using ABP's ITenantAppService
            // This automatically creates tenant + admin user + assigns admin role
            var createDto = new TenantCreateDto
            {
                Name = sanitizedTenantName,
                AdminEmailAddress = request.AdminEmail,
                AdminPassword = request.AdminPassword
            };

            _logger.LogInformation("TenantCreationFacade: Calling ABP ITenantAppService.CreateAsync - TenantName={TenantName}",
                sanitizedTenantName);

            var tenantDto = await _tenantAppService.CreateAsync(createDto);

            // Set ExtraProperties for tracking (ABP pattern)
            await SetTenantExtraPropertiesAsync(tenantDto.Id, request);

            return tenantDto;
        }

        /// <summary>
        /// Sets ExtraProperties on the tenant for onboarding tracking and metadata
        /// </summary>
        private async Task SetTenantExtraPropertiesAsync(Guid tenantId, TenantCreationFacadeRequest request)
        {
            try
            {
                using (_currentTenant.Change(null)) // Host context to update tenant
                {
                    var tenant = await _tenantRepository.GetAsync(tenantId);

                    tenant.ExtraProperties["OnboardingStatus"] = "Pending";
                    tenant.ExtraProperties["CreatedByAgent"] = "TenantCreationFacade";
                    tenant.ExtraProperties["CreatedAt"] = DateTime.UtcNow.ToString("O");

                    if (!string.IsNullOrEmpty(request.DeviceFingerprint))
                    {
                        tenant.ExtraProperties["DeviceFingerprint"] = request.DeviceFingerprint;
                    }

                    if (!string.IsNullOrEmpty(request.IpAddress))
                    {
                        tenant.ExtraProperties["CreatedFromIP"] = request.IpAddress;
                    }

                    await _tenantRepository.UpdateAsync(tenant);

                    _logger.LogInformation("TenantCreationFacade: ExtraProperties set for tenant {TenantId}", tenantId);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail tenant creation for metadata issues
                _logger.LogWarning(ex, "TenantCreationFacade: Failed to set ExtraProperties for tenant {TenantId}. Tenant and user created successfully but metadata incomplete.", tenantId);
            }
        }

        /// <summary>
        /// Builds the creation result by retrieving the created user
        /// </summary>
        private async Task<TenantCreationFacadeResult> BuildCreationResultAsync(
            AbpTenantDto tenantDto,
            TenantCreationFacadeRequest request,
            FraudCheckResult fraudCheck)
        {
            // Retrieve the admin user that was created by ABP
            Volo.Abp.Identity.IdentityUser? adminUser = null;

            using (_currentTenant.Change(tenantDto.Id)) // Switch to tenant context
            {
                // ABP's TenantAppService creates the user with the admin email as username
                var users = await _userRepository.GetListAsync();
                adminUser = users.FirstOrDefault(u => u.Email == request.AdminEmail);

                if (adminUser == null)
                {
                    _logger.LogError("TenantCreationFacade: Admin user not found after creation - TenantId={TenantId}, Email={Email}",
                        tenantDto.Id, request.AdminEmail);
                    throw new InvalidOperationException("Tenant created but admin user not found. This is an unexpected error.");
                }

                // Store FirstAdminUserId in tenant ExtraProperties
                using (_currentTenant.Change(null)) // Back to host context
                {
                    try
                    {
                        var tenant = await _tenantRepository.GetAsync(tenantDto.Id);
                        tenant.ExtraProperties["FirstAdminUserId"] = adminUser.Id.ToString();
                        await _tenantRepository.UpdateAsync(tenant);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "TenantCreationFacade: Failed to set FirstAdminUserId in ExtraProperties");
                    }
                }
            }

            return new TenantCreationFacadeResult
            {
                TenantId = tenantDto.Id,
                TenantName = tenantDto.Name,
                AdminEmail = request.AdminEmail,
                AdminUserId = adminUser.Id,
                User = adminUser,
                IsFlaggedForReview = fraudCheck.IsSuspicious,
                FlagReason = fraudCheck.Reason,
                Message = "Tenant created successfully"
            };
        }

        /// <summary>
        /// Sanitizes tenant name according to ABP requirements
        /// </summary>
        private string SanitizeTenantName(string tenantName)
        {
            if (string.IsNullOrWhiteSpace(tenantName))
            {
                throw new ArgumentException("Tenant name cannot be empty", nameof(tenantName));
            }

            var sanitized = tenantName
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("_", "-");

            // Remove any characters that aren't alphanumeric or hyphens
            sanitized = new string(sanitized.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

            // Ensure it doesn't start or end with a hyphen
            sanitized = sanitized.Trim('-');

            // Limit length to 50 characters (ABP tenant name limit is 64, leave room for timestamp suffix)
            if (sanitized.Length > 50)
            {
                sanitized = sanitized.Substring(0, 50);
            }

            if (sanitized.Length < 2)
            {
                throw new ArgumentException("Tenant name is too short after sanitization", nameof(tenantName));
            }

            return sanitized;
        }

        /// <summary>
        /// Validates reCAPTCHA token before tenant creation
        /// CAPTCHA is optional - warns if missing but does not block tenant creation
        /// </summary>
        private async Task<RecaptchaValidationResult> ValidateRecaptchaAsync(TenantCreationFacadeRequest request)
        {
            // CAPTCHA is optional - if token is missing, warn but allow creation
            if (string.IsNullOrEmpty(request.RecaptchaToken))
            {
                _logger.LogWarning("TenantCreationFacade: CAPTCHA token not provided - proceeding without CAPTCHA validation - TenantName={TenantName}, Email={Email}",
                    request.TenantName, request.AdminEmail);
                return new RecaptchaValidationResult { Success = true, Score = 0.5, MeetsThreshold = true };
            }

            // If token is provided, validate it
            try
            {
                var result = await _recaptchaService.ValidateWithScoreAsync(request.RecaptchaToken, request.IpAddress);

                if (!result.Success || !result.MeetsThreshold)
                {
                    _logger.LogWarning("TenantCreationFacade: CAPTCHA validation failed - proceeding anyway - TenantName={TenantName}, Email={Email}, Score={Score}, Error={Error}",
                        request.TenantName, request.AdminEmail, result.Score, result.ErrorMessage);
                    // Don't block - just log the warning
                    return new RecaptchaValidationResult { Success = true, Score = result.Score, MeetsThreshold = true };
                }

                _logger.LogInformation("TenantCreationFacade: CAPTCHA validation passed - Score={Score}", result.Score);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "TenantCreationFacade: CAPTCHA validation error - proceeding without CAPTCHA - TenantName={TenantName}, Email={Email}",
                    request.TenantName, request.AdminEmail);
                return new RecaptchaValidationResult { Success = true, Score = 0.5, MeetsThreshold = true };
            }
        }

        /// <summary>
        /// Validates request for fraudulent patterns
        /// Bypasses validation if FraudDetection:Enabled is false in configuration
        /// </summary>
        private async Task<FraudCheckResult> ValidateFraudDetectionAsync(TenantCreationFacadeRequest request)
        {
            // Check if fraud detection is enabled in configuration
            var fraudDetectionEnabled = _configuration.GetValue<bool>("FraudDetection:Enabled", true);
            
            if (!fraudDetectionEnabled)
            {
                _logger.LogInformation("TenantCreationFacade: Fraud detection BYPASSED (disabled in config) - TenantName={TenantName}, Email={Email}",
                    request.TenantName, request.AdminEmail);
                return new FraudCheckResult { IsSuspicious = false, ShouldBlock = false, RiskScore = 0.0, Reason = "Bypassed" };
            }

            var fraudCheck = await _fraudDetector.CheckAsync(request);

            if (fraudCheck.IsSuspicious)
            {
                _logger.LogWarning("TenantCreationFacade: Suspicious activity detected - TenantName={TenantName}, RiskScore={RiskScore}, Reason={Reason}",
                    request.TenantName, fraudCheck.RiskScore, fraudCheck.Reason);

                if (fraudCheck.ShouldBlock)
                {
                    throw new SecurityException($"Tenant creation blocked: {fraudCheck.Reason}");
                }
            }

            return fraudCheck;
        }

        /// <summary>
        /// Tracks tenant creation fingerprint for fraud detection
        /// </summary>
        private async Task TrackTenantCreationFingerprintAsync(
            Guid tenantId,
            TenantCreationFacadeRequest request,
            double recaptchaScore,
            FraudCheckResult fraudCheck)
        {
            try
            {
                var fingerprint = new Models.Entities.TenantCreationFingerprint
                {
                    TenantId = tenantId,
                    DeviceId = request.DeviceFingerprint ?? string.Empty,
                    IpAddress = request.IpAddress ?? string.Empty,
                    UserAgent = request.UserAgent ?? string.Empty,
                    AdminEmail = request.AdminEmail,
                    TenantName = request.TenantName,
                    RecaptchaScore = recaptchaScore,
                    CreatedAt = DateTime.UtcNow,
                    IsFlagged = fraudCheck.IsSuspicious,
                    FlagReason = fraudCheck.Reason
                };

                _dbContext.TenantCreationFingerprints.Add(fingerprint);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("TenantCreationFacade: Fingerprint tracked - TenantId={TenantId}, IsFlagged={IsFlagged}",
                    tenantId, fraudCheck.IsSuspicious);
            }
            catch (Exception ex)
            {
                // Log error but don't fail tenant creation for tracking issues
                _logger.LogWarning(ex, "TenantCreationFacade: Failed to track fingerprint for tenant {TenantId}", tenantId);
            }
        }

        /// <summary>
        /// Creates tenant using ABP's ITenantAppService directly
        /// NO security checks - only validates registration data and creates records
        /// Use for internal/admin operations or when security is handled elsewhere
        /// </summary>
        public async Task<TenantCreationFacadeResult> CreateTenantWithoutSecurityAsync(
            string tenantName,
            string adminEmail,
            string adminPassword)
        {
            if (string.IsNullOrWhiteSpace(tenantName))
                throw new ArgumentException("Tenant name is required", nameof(tenantName));
            if (string.IsNullOrWhiteSpace(adminEmail))
                throw new ArgumentException("Admin email is required", nameof(adminEmail));
            if (string.IsNullOrWhiteSpace(adminPassword))
                throw new ArgumentException("Admin password is required", nameof(adminPassword));

            _logger.LogInformation("TenantCreationFacade: Creating tenant WITHOUT security checks - TenantName={TenantName}, Email={Email}",
                tenantName, adminEmail);

            try
            {
                // Sanitize tenant name for ABP requirements
                var sanitizedTenantName = SanitizeTenantName(tenantName);

                // Check if tenant already exists (use host context)
                using (_currentTenant.Change(null))
                {
                    var existingTenant = await _tenantRepository.FindByNameAsync(sanitizedTenantName);
                    if (existingTenant != null)
                    {
                        // Append timestamp to make unique
                        sanitizedTenantName = $"{sanitizedTenantName}-{DateTime.UtcNow:HHmmss}";
                        _logger.LogWarning("TenantCreationFacade: Tenant name exists, using modified name: {TenantName}",
                            sanitizedTenantName);
                    }

                    // Check if email already exists across all tenants
                    var allUsers = await _userRepository.GetListAsync();
                    var existingUser = allUsers.FirstOrDefault(u => u.Email == adminEmail);
                    if (existingUser != null)
                    {
                        throw new InvalidOperationException($"Email '{adminEmail}' is already registered");
                    }
                }

                // Create tenant using ABP's ITenantAppService directly
                // This only validates: name format, email format, password requirements
                // NO CAPTCHA, NO fraud detection, NO rate limiting
                var createDto = new Volo.Abp.TenantManagement.TenantCreateDto
                {
                    Name = sanitizedTenantName,
                    AdminEmailAddress = adminEmail,
                    AdminPassword = adminPassword
                };

                _logger.LogInformation("TenantCreationFacade: Calling ABP ITenantAppService.CreateAsync (no security) - TenantName={TenantName}",
                    sanitizedTenantName);

                var tenantDto = await _tenantAppService.CreateAsync(createDto);

                // Get the created admin user
                using (_currentTenant.Change(tenantDto.Id))
                {
                    var users = await _userRepository.GetListAsync();
                    var adminUser = users.FirstOrDefault(u => u.Email == adminEmail);

                    if (adminUser == null)
                    {
                        _logger.LogError("TenantCreationFacade: Admin user not found after creation - TenantId={TenantId}, Email={Email}",
                            tenantDto.Id, adminEmail);
                        throw new InvalidOperationException("Tenant created but admin user not found");
                    }

                    // Build result
                    var result = new TenantCreationFacadeResult
                    {
                        TenantId = tenantDto.Id,
                        TenantName = tenantDto.Name ?? sanitizedTenantName,
                        AdminEmail = adminEmail,
                        AdminUserId = adminUser.Id,
                        User = adminUser,
                        IsFlaggedForReview = false,
                        Message = "Tenant created successfully (no security checks)"
                    };

                    _logger.LogInformation("TenantCreationFacade: Tenant created successfully (no security) - TenantId={TenantId}, UserId={UserId}",
                        result.TenantId, result.AdminUserId);

                    return result;
                }
            }
            catch (InvalidOperationException iex)
            {
                _logger.LogWarning(iex, "TenantCreationFacade: Business logic error - TenantName={TenantName}, Email={Email}, Error={Error}",
                    tenantName, adminEmail, iex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TenantCreationFacade: Unexpected error creating tenant - TenantName={TenantName}, Email={Email}",
                    tenantName, adminEmail);
                throw new InvalidOperationException($"Tenant creation failed: {ex.Message}", ex);
            }
        }

    }
}
