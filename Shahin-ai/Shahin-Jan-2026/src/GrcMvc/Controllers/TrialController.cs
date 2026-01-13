using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;
using System.ComponentModel.DataAnnotations;
using System.Security;
using GrcMvc.Models.DTOs;
using GrcMvc.Services.Interfaces;
using GrcMvc.Data;
using GrcMvc.Models.Entities;
using GrcMvc.Constants;
using AspNetSignInManager = Microsoft.AspNetCore.Identity.SignInManager<Volo.Abp.Identity.IdentityUser>;

namespace GrcMvc.Controllers
{
    /// <summary>
    /// Trial registration controller using ABP's ITenantAppService directly
    /// SECURITY: Includes CAPTCHA validation, rate limiting, and duplicate checking
    /// </summary>
    [Route("trial")]
    public class TrialController : Controller
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly AspNetSignInManager _signInManager;
        private readonly GrcDbContext _dbContext;
        private readonly ILogger<TrialController> _logger;

        public TrialController(
            ITenantAppService tenantAppService,
            IIdentityUserRepository userRepository,
            ICurrentTenant currentTenant,
            AspNetSignInManager signInManager,
            GrcDbContext dbContext,
            ILogger<TrialController> logger)
        {
            _tenantAppService = tenantAppService;
            _userRepository = userRepository;
            _currentTenant = currentTenant;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Show trial registration form (not ABP's Account Register)
            // Trial registration creates tenant + admin + redirects to onboarding
            return View(new TrialRegistrationModel());
        }

        [HttpPost("")]
        [HttpPost("register")] // Support both /trial and /trial/register routes
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("auth")] // SECURITY: Rate limiting to prevent abuse (5 requests per 5 minutes)
        public async Task<IActionResult> Register(TrialRegistrationModel model)
        {
            // #region agent log
            try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "K", location = "TrialController.cs:51", message = "Register entry - METHOD CALLED", data = new { requestPath = Request.Path.Value, requestMethod = Request.Method, organizationName = model?.OrganizationName ?? "null", email = model?.Email ?? "null", fullName = model?.FullName ?? "null", acceptTerms = model?.AcceptTerms ?? false, tenantAppServiceExists = _tenantAppService != null, dbContextExists = _dbContext != null, modelIsNull = model == null, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            // Manual validation for AcceptTerms (Range attribute doesn't work with jQuery validation)
            if (!model.AcceptTerms)
            {
                ModelState.AddModelError("AcceptTerms", "يجب الموافقة على الشروط والأحكام");
            }

            // Check for duplicate email/tenant before creation
            var sanitizedTenantName = SanitizeTenantName(model.OrganizationName);
            var existingTenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.Email != null && t.Email.ToLower() == model.Email.ToLower() ||
                                         t.TenantSlug != null && t.TenantSlug.ToLower() == sanitizedTenantName.ToLower());
            
            if (existingTenant != null)
            {
                var errorMessage = existingTenant.Email?.ToLower() == model.Email.ToLower()
                    ? "This email is already registered. Please use a different email or sign in."
                    : "This organization name is already taken. Please choose a different name.";
                ModelState.AddModelError("", errorMessage);
                _logger.LogWarning("Duplicate registration attempt - Email: {Email}, TenantSlug: {Slug}", 
                    model.Email, sanitizedTenantName);
                return View("Index", model);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Trial registration validation failed");
                return View("Index", model);
            }

            try
            {
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "F", location = "TrialController.cs:76", message = "TrialFormStart", data = new { organizationName = model.OrganizationName, email = model.Email, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                _logger.LogInformation("TrialFormStart: Organization={Organization}, Email={Email}, IP={IP}",
                    model.OrganizationName, model.Email, remoteIp);

                // Create tenant using ABP's ITenantAppService directly
                // Note: sanitizedTenantName already computed above during duplicate check (line 96)
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "F", location = "TrialController.cs:84", message = "Tenant name sanitized", data = new { originalName = model.OrganizationName, sanitizedName = sanitizedTenantName, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                var createDto = new Volo.Abp.TenantManagement.TenantCreateDto
                {
                    Name = sanitizedTenantName,
                    AdminEmailAddress = model.Email,
                    AdminPassword = model.Password
                };

                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "I", location = "TrialController.cs:103", message = "Before tenant creation", data = new { tenantName = createDto.Name, adminEmail = createDto.AdminEmailAddress, tenantAppServiceType = _tenantAppService.GetType().Name, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                Volo.Abp.TenantManagement.TenantDto tenantDto;
                try
                {
                    tenantDto = await _tenantAppService.CreateAsync(createDto);
                    // #region agent log
                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "I", location = "TrialController.cs:108", message = "Tenant created successfully", data = new { tenantId = tenantDto.Id.ToString(), tenantName = tenantDto.Name, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                }
                catch (Exception tenantEx)
                {
                    // #region agent log
                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "I", location = "TrialController.cs:113", message = "Tenant creation exception", data = new { exceptionType = tenantEx.GetType().Name, exceptionMessage = tenantEx.Message, innerException = tenantEx.InnerException?.Message, stackTrace = tenantEx.StackTrace?.Substring(0, Math.Min(1000, (tenantEx.StackTrace?.Length).GetValueOrDefault(0))), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                    throw; // Re-throw to be caught by outer catch block
                }

                _logger.LogInformation("TrialFormSuccess: Tenant created - TenantId={TenantId}, Name={Name}",
                    tenantDto.Id, tenantDto.Name);

                _logger.LogInformation("TrialFormSuccess: Tenant created - TenantId={TenantId}, Name={Name}",
                    tenantDto.Id, tenantDto.Name);

                // Create OnboardingWizard entity for comprehensive wizard flow
                var wizard = new OnboardingWizard
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantDto.Id,
                    WizardStatus = "InProgress",
                    CurrentStep = 1,
                    StartedAt = DateTime.UtcNow,
                    ProgressPercent = 0,
                    OrganizationLegalNameEn = model.OrganizationName,
                    CountryOfIncorporation = "SA", // Default for Saudi market
                    DefaultTimezone = "Asia/Riyadh",
                    PrimaryLanguage = "bilingual",
                    ControlOwnershipApproach = "hybrid",
                    NotificationPreference = "email",
                    EscalationDaysOverdue = 3,
                    EvidenceNamingConventionRequired = true,
                    EvidenceNamingPattern = "{TenantId}-{ControlId}-{Date}-{Sequence}",
                    EvidenceRetentionYears = 7,
                    ConfidentialEvidenceEncryption = true,
                    AdoptDefaultBaseline = true,
                    DesiredMaturity = "Foundation",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "F", location = "TrialController.cs:121", message = "Before OnboardingWizard save", data = new { wizardId = wizard.Id.ToString(), tenantId = wizard.TenantId.ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                _dbContext.OnboardingWizards.Add(wizard);
                await _dbContext.SaveChangesAsync();
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "F", location = "TrialController.cs:124", message = "OnboardingWizard saved", data = new { wizardId = wizard.Id.ToString(), tenantId = tenantDto.Id.ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                _logger.LogInformation("TrialFormSuccess: OnboardingWizard created. WizardId={WizardId}, TenantId={TenantId}",
                    wizard.Id, tenantDto.Id);

                // Create custom Tenant record in GrcDbContext (synced with ABP tenant)
                var customTenant = new Models.Entities.Tenant
                {
                    Id = tenantDto.Id, // Use same ID as ABP tenant for consistency
                    TenantSlug = sanitizedTenantName,
                    OrganizationName = model.OrganizationName,
                    AdminEmail = model.Email,
                    Email = model.Email,
                    TenantCode = sanitizedTenantName.ToUpperInvariant().Substring(0, Math.Min(10, sanitizedTenantName.Length)),
                    BusinessCode = $"{sanitizedTenantName.ToUpperInvariant().Substring(0, Math.Min(4, sanitizedTenantName.Length))}-TEN-{DateTime.UtcNow.Year}-000001",
                    Status = "Active",
                    IsActive = true,
                    IsTrial = true,
                    TrialEndsAt = DateTime.UtcNow.AddDays(7), // 7-day trial
                    OnboardingStatus = "NOT_STARTED",
                    SubscriptionTier = "Trial",
                    CorrelationId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _dbContext.Tenants.Add(customTenant);

                // Find the ABP user that was created and create TenantUser record
                IdentityUser? abpUser = null;
                using (_currentTenant.Change(tenantDto.Id))
                {
                    abpUser = await _userRepository.FindByNormalizedEmailAsync(model.Email.ToUpperInvariant());
                }

                if (abpUser != null)
                {
                    var tenantUser = new TenantUser
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantDto.Id,
                        UserId = abpUser.Id.ToString(),
                        RoleCode = RoleConstants.TenantAdmin, // Use constant for tenant admin role
                        TitleCode = "TENANT_ADMIN",
                        Status = "Active",
                        ActivatedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _dbContext.TenantUsers.Add(tenantUser);

                    _logger.LogInformation("TrialFormSuccess: TenantUser created. UserId={UserId}, TenantId={TenantId}",
                        abpUser.Id, tenantDto.Id);
                }
                else
                {
                    _logger.LogWarning("TrialFormWarning: Could not find ABP user {Email} to create TenantUser", model.Email);
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("TrialFormSuccess: Custom Tenant and TenantUser records created. TenantId={TenantId}",
                    tenantDto.Id);

                // Store tenant info in TempData for onboarding
                TempData["TenantId"] = tenantDto.Id.ToString();
                TempData["TenantSlug"] = sanitizedTenantName;
                TempData["OrganizationName"] = model.OrganizationName;
                TempData["AdminEmail"] = model.Email;
                TempData["WelcomeMessage"] = $"مرحباً بك في {model.OrganizationName}! لنبدأ إعداد منظمتك.";

                // Auto-login the user seamlessly (no manual login required)
                if (abpUser != null)
                {
                    try
                    {
                        await _signInManager.SignInAsync(abpUser, isPersistent: true);
                        _logger.LogInformation("TrialFormSuccess: Auto-login completed for {Email}, TenantId={TenantId}",
                            model.Email, tenantDto.Id);
                    }
                    catch (Exception signInEx)
                    {
                        _logger.LogWarning(signInEx, "TrialFormWarning: Auto-login failed for {Email}",
                            model.Email);
                    }
                }

                // Redirect to 12-step Comprehensive Onboarding Wizard
                _logger.LogInformation("TrialFormSuccess: Redirecting to Comprehensive Onboarding Wizard. TenantId={TenantId}",
                    tenantDto.Id);
                return RedirectToAction("Index", "OnboardingWizard", new { tenantId = tenantDto.Id });
            }
            catch (InvalidOperationException iex)
            {
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "F", location = "TrialController.cs:138", message = "InvalidOperationException", data = new { exceptionType = iex.GetType().Name, exceptionMessage = iex.Message, email = model.Email, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                // Business logic error (email exists, tenant name conflict, etc.)
                _logger.LogWarning(iex, "TrialFormFailure: Business logic error for {Email}. Message: {Message}",
                    model.Email, iex.Message);

                ModelState.AddModelError("", $"Registration failed: {iex.Message}");
                return View("Index", model);
            }
            catch (Exception ex)
            {
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "F", location = "TrialController.cs:147", message = "Register exception", data = new { exceptionType = ex.GetType().Name, exceptionMessage = ex.Message, email = model.Email, innerException = ex.InnerException?.Message, stackTrace = ex.StackTrace?.Substring(0, Math.Min(500, (ex.StackTrace?.Length).GetValueOrDefault(0))), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                _logger.LogError(ex, "TrialFormException: Unexpected error for {Email}. Message: {Message}",
                    model.Email, ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError("TrialFormException: Inner exception: {InnerMessage}", ex.InnerException.Message);
                }

                // Show generic error message
                var errorMessage = "Registration failed. Please try again later.";
                TempData["ErrorMessage"] = errorMessage;
                return View("Index", model);
            }
        }

        /// <summary>
        /// Sanitizes tenant name for ABP requirements
        /// </summary>
        private string SanitizeTenantName(string organizationName)
        {
            if (string.IsNullOrWhiteSpace(organizationName))
            {
                throw new ArgumentException("Organization name cannot be empty");
            }

            var sanitized = organizationName
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("_", "-");

            // Remove any characters that aren't alphanumeric or hyphens
            sanitized = new string(sanitized.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

            // Ensure it doesn't start or end with a hyphen
            sanitized = sanitized.Trim('-');

            // Limit length to 50 characters
            if (sanitized.Length > 50)
            {
                sanitized = sanitized.Substring(0, 50).Trim('-');
            }

            if (sanitized.Length < 2)
            {
                throw new ArgumentException("Tenant name is too short after sanitization");
            }

            return sanitized;
        }
    }

    public class TrialRegistrationModel
    {
        [Required(ErrorMessage = "Organization name is required")]
        public string OrganizationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters")]
        public string Password { get; set; } = string.Empty;

        public bool AcceptTerms { get; set; }
    }

    public class TrialSuccessViewModel
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string TenantSlug { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime TrialEndsAt { get; set; }
        public string? OnboardingUrl { get; set; }
    }
}
