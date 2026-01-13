using System;
using System.Linq;
using System.Threading.Tasks;
using GrcMvc.Data;
using GrcMvc.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using AbpTenantDto = Volo.Abp.TenantManagement.TenantDto;

namespace GrcMvc.EventHandlers
{
    /// <summary>
    /// Event handler that automatically creates a tenant when a new user registers
    /// This enables self-registration flow where users can create their own tenant
    /// </summary>
    public class UserCreatedEventHandler : 
        ILocalEventHandler<EntityCreatedEventData<IdentityUser>>, 
        ITransientDependency
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly GrcDbContext _dbContext;
        private readonly ILogger<UserCreatedEventHandler> _logger;

        public UserCreatedEventHandler(
            ITenantAppService tenantAppService,
            ITenantRepository tenantRepository,
            ICurrentTenant currentTenant,
            GrcDbContext dbContext,
            ILogger<UserCreatedEventHandler> logger)
        {
            _tenantAppService = tenantAppService;
            _tenantRepository = tenantRepository;
            _currentTenant = currentTenant;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task HandleEventAsync(EntityCreatedEventData<IdentityUser> eventData)
        {
            // #region agent log
            try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:46", message = "HandleEventAsync entry", data = new { userId = eventData.Entity.Id.ToString(), userEmail = eventData.Entity.Email, userName = eventData.Entity.UserName, currentTenantId = _currentTenant.Id?.ToString() ?? "null", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            var user = eventData.Entity;

            // Only create tenant if user registered at host level (no tenant context)
            // Skip if user was created within an existing tenant context
            if (_currentTenant.Id.HasValue)
            {
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:52", message = "Skipping tenant creation - user in tenant context", data = new { userEmail = user.Email, tenantId = _currentTenant.Id.ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                _logger.LogDebug("UserCreatedEventHandler: User {Email} created in tenant context {TenantId}, skipping tenant creation",
                    user.Email, _currentTenant.Id);
                return;
            }

            // Check if user already has tenant association
            var userIdString = user.Id.ToString();
            var existingTenantUser = await _dbContext.TenantUsers
                .FirstOrDefaultAsync(tu => tu.UserId == userIdString && !tu.IsDeleted);

            if (existingTenantUser != null)
            {
                _logger.LogDebug("UserCreatedEventHandler: User {Email} already has tenant association, skipping tenant creation",
                    user.Email);
                return;
            }

            try
            {
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:71", message = "Starting tenant creation", data = new { userEmail = user.Email, tenantAppServiceExists = _tenantAppService != null, tenantRepositoryExists = _tenantRepository != null, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                _logger.LogInformation("UserCreatedEventHandler: Creating tenant for new user {Email}", user.Email);

                // Generate tenant name from email (before @ symbol)
                var tenantName = user.Email?.Split('@')[0]?.ToLowerInvariant() ?? user.UserName?.ToLowerInvariant() ?? Guid.NewGuid().ToString("N")[..8];
                
                // Sanitize tenant name (remove special characters, ensure valid format)
                tenantName = SanitizeTenantName(tenantName);
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:79", message = "Tenant name sanitized", data = new { sanitizedTenantName = tenantName, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                // Ensure tenant name is unique by checking existing tenants
                using (_currentTenant.Change(null)) // Ensure host context
                {
                    var existingTenant = await _tenantRepository.FindByNameAsync(tenantName);

                    if (existingTenant != null)
                    {
                        // Append timestamp to make unique
                        tenantName = $"{tenantName}-{DateTime.UtcNow:HHmmss}";
                        _logger.LogWarning("UserCreatedEventHandler: Tenant name exists, using modified name: {TenantName}", tenantName);
                    }
                }

                // Create tenant using ABP's ITenantAppService
                // Note: We need to generate a temporary password or use a secure flow
                // For now, we'll create tenant but user will need to set password via password reset
                var createDto = new TenantCreateDto
                {
                    Name = tenantName,
                    AdminEmailAddress = user.Email ?? user.UserName,
                    AdminPassword = GenerateTemporaryPassword() // User should change on first login
                };

                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:104", message = "Before tenant creation", data = new { tenantName = createDto.Name, adminEmail = createDto.AdminEmailAddress, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                var tenantDto = await _tenantAppService.CreateAsync(createDto);
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:106", message = "Tenant created", data = new { tenantId = tenantDto.Id.ToString(), tenantName = tenantDto.Name, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                _logger.LogInformation("UserCreatedEventHandler: Tenant created successfully - TenantId={TenantId}, TenantName={TenantName}, UserEmail={Email}",
                    tenantDto.Id, tenantDto.Name, user.Email);

                // Create or update custom Tenant entity with trial flags
                // Note: Tenant.Id should match ABP tenant ID for consistency
                var customTenant = await _dbContext.Tenants
                    .FirstOrDefaultAsync(t => t.AbpTenantId == tenantDto.Id);

                if (customTenant == null)
                {
                    // Create custom Tenant entity with trial flags
                    customTenant = new GrcMvc.Models.Entities.Tenant
                    {
                        Id = tenantDto.Id, // Use same ID as ABP tenant for consistency
                        TenantSlug = tenantDto.Name,
                        OrganizationName = tenantDto.Name,
                        IsTrial = true,
                        TrialStartsAt = DateTime.UtcNow,
                        TrialEndsAt = DateTime.UtcNow.AddDays(7),
                        BillingStatus = "Trialing",
                        IsActive = true,
                        Status = "Active",
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };
                    _dbContext.Tenants.Add(customTenant);
                    _logger.LogInformation("UserCreatedEventHandler: Created custom Tenant entity with trial flags - TenantId={TenantId}, TrialEndsAt={TrialEndsAt}",
                        tenantDto.Id, customTenant.TrialEndsAt);
                }
                else
                {
                    // Update existing tenant to be trial
                    customTenant.IsTrial = true;
                    customTenant.TrialStartsAt = DateTime.UtcNow;
                    customTenant.TrialEndsAt = DateTime.UtcNow.AddDays(7);
                    customTenant.BillingStatus = "Trialing";
                    customTenant.ModifiedDate = DateTime.UtcNow;
                    _logger.LogInformation("UserCreatedEventHandler: Updated existing Tenant entity with trial flags - TenantId={TenantId}, TrialEndsAt={TrialEndsAt}",
                        tenantDto.Id, customTenant.TrialEndsAt);
                }

                await _dbContext.SaveChangesAsync();

                // Create OnboardingWizard entity for comprehensive wizard flow
                var wizard = new OnboardingWizard
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantDto.Id,
                    WizardStatus = "InProgress",
                    CurrentStep = 1,
                    StartedAt = DateTime.UtcNow,
                    ProgressPercent = 0,
                    OrganizationLegalNameEn = tenantName,
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
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:136", message = "Before OnboardingWizard save", data = new { wizardId = wizard.Id.ToString(), tenantId = wizard.TenantId.ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                _dbContext.OnboardingWizards.Add(wizard);
                await _dbContext.SaveChangesAsync();
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:139", message = "OnboardingWizard saved", data = new { wizardId = wizard.Id.ToString(), tenantId = tenantDto.Id.ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                _logger.LogInformation("UserCreatedEventHandler: OnboardingWizard created - WizardId={WizardId}, TenantId={TenantId}",
                    wizard.Id, tenantDto.Id);
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:144", message = "HandleEventAsync exit success", data = new { timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            catch (Exception ex)
            {
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "B", location = "UserCreatedEventHandler.cs:147", message = "HandleEventAsync exception", data = new { exceptionType = ex.GetType().Name, exceptionMessage = ex.Message, userEmail = user.Email, stackTrace = ex.StackTrace?.Substring(0, Math.Min(500, (ex.StackTrace?.Length).GetValueOrDefault(0))), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                _logger.LogError(ex, "UserCreatedEventHandler: Failed to create tenant for user {Email}", user.Email);
                // Don't throw - allow user creation to succeed even if tenant creation fails
                // Admin can manually create tenant later
            }
        }

        /// <summary>
        /// Sanitizes tenant name for ABP requirements
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

            // Limit length to 50 characters
            if (sanitized.Length > 50)
            {
                sanitized = sanitized.Substring(0, 50).Trim('-');
            }

            if (sanitized.Length < 2)
            {
                // Fallback to GUID if too short
                sanitized = Guid.NewGuid().ToString("N")[..8];
            }

            return sanitized;
        }

        /// <summary>
        /// Generates a temporary password for the tenant admin
        /// User should be prompted to change password on first login
        /// </summary>
        private string GenerateTemporaryPassword()
        {
            // Generate a secure temporary password
            // In production, consider sending password via email or requiring password reset
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            
            return password;
        }
    }
}
