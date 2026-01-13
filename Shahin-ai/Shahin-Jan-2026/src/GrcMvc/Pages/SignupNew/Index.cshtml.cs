using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;
using GrcMvc.Models.Entities;
using GrcMvc.Data;
using AspNetSignInManager = Microsoft.AspNetCore.Identity.SignInManager<Volo.Abp.Identity.IdentityUser>;
using Volo.Abp.Identity;
using AbpIdentityUser = Volo.Abp.Identity.IdentityUser;

namespace GrcMvc.Pages.SignupNew
{
    /// <summary>
    /// Trial registration using ABP Framework's built-in tenant management
    /// Route: /SignupNew
    /// </summary>
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly AspNetSignInManager _signInManager;
        private readonly GrcDbContext _dbContext;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            ITenantAppService tenantAppService,
            IIdentityUserRepository userRepository,
            ICurrentTenant currentTenant,
            AspNetSignInManager signInManager,
            GrcDbContext dbContext,
            ILogger<IndexModel> logger)
        {
            _tenantAppService = tenantAppService;
            _userRepository = userRepository;
            _currentTenant = currentTenant;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Company name is required")]
            [Display(Name = "Company Name")]
            public string CompanyName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Full name is required")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Work Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters")]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "You must accept the terms")]
            [Display(Name = "I accept the terms and conditions")]
            public bool AcceptTerms { get; set; }
        }

        public void OnGet()
        {
            // Display the registration form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Input.AcceptTerms)
            {
                ModelState.AddModelError("Input.AcceptTerms", "You must accept the terms and conditions");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // 1. Sanitize company name to create tenant slug
                var baseSlug = SanitizeTenantName(Input.CompanyName);
                
                // 2. Generate unique slug
                var tenantSlug = await GenerateUniqueSlugAsync(baseSlug);
                
                // 3. Create tenant using ABP's ITenantAppService
                var createDto = new TenantCreateDto
                {
                    Name = tenantSlug,
                    AdminEmailAddress = Input.Email,
                    AdminPassword = Input.Password
                };

                _logger.LogInformation("SignupNew: Creating ABP tenant - Name={TenantName}, Email={Email}",
                    tenantSlug, Input.Email);

                TenantDto tenantDto;
                AbpIdentityUser? abpUser = null;
                
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    // Create ABP tenant (creates tenant + admin user + role automatically)
                    tenantDto = await _tenantAppService.CreateAsync(createDto);

                    // 4. Create custom Tenant record (synced with ABP tenant)
                    var customTenant = new GrcMvc.Models.Entities.Tenant
                    {
                        Id = tenantDto.Id, // Use same ID as ABP tenant
                        TenantSlug = tenantSlug,
                        OrganizationName = Input.CompanyName,
                        AdminEmail = Input.Email,
                        Email = Input.Email,
                        Status = "Active",
                        IsActive = true,
                        IsTrial = true,
                        TrialEndsAt = DateTime.UtcNow.AddDays(7),
                        OnboardingStatus = "NOT_STARTED",
                        SubscriptionTier = "Trial",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _dbContext.Tenants.Add(customTenant);

                    // 5. Create OnboardingWizard
                    var wizard = new OnboardingWizard
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantDto.Id,
                        WizardStatus = "InProgress",
                        CurrentStep = 1,
                        StartedAt = DateTime.UtcNow,
                        ProgressPercent = 0,
                        OrganizationLegalNameEn = Input.CompanyName,
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _dbContext.OnboardingWizards.Add(wizard);

                    // 6. Find ABP user and create TenantUser linkage
                    using (_currentTenant.Change(tenantDto.Id))
                    {
                        abpUser = await _userRepository.FindByNormalizedEmailAsync(Input.Email.ToUpperInvariant());
                    }

                    if (abpUser != null)
                    {
                        var tenantUser = new TenantUser
                        {
                            Id = Guid.NewGuid(),
                            TenantId = tenantDto.Id,
                            UserId = abpUser.Id.ToString(),
                            RoleCode = "TenantAdmin", // Use string constant
                            Status = "Active",
                            ActivatedAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _dbContext.TenantUsers.Add(tenantUser);
                    }

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                // 7. Auto-login the user (need to get user again after transaction)
                AbpIdentityUser? loginUser = null;
                using (_currentTenant.Change(tenantDto.Id))
                {
                    loginUser = await _userRepository.FindByNormalizedEmailAsync(Input.Email.ToUpperInvariant());
                }

                if (loginUser != null)
                {
                    using (_currentTenant.Change(tenantDto.Id))
                    {
                        await _signInManager.SignInAsync(loginUser, isPersistent: true);
                        _logger.LogInformation("SignupNew: Auto-login completed for {Email}, TenantId={TenantId}",
                            Input.Email, tenantDto.Id);
                    }
                }

                // 8. Redirect to onboarding wizard
                return RedirectToAction("Index", "OnboardingWizard", new { tenantId = tenantDto.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignupNew: Registration failed for {Email}", Input.Email);
                ErrorMessage = "Registration failed. Please try again.";
                return Page();
            }
        }

        private string SanitizeTenantName(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Company name cannot be empty");

            // Remove all non-alphanumeric characters, convert to lowercase
            var sanitized = Regex.Replace(companyName.ToLowerInvariant(), "[^a-z0-9]", "");
            
            // Limit to 50 characters
            if (sanitized.Length > 50)
                sanitized = sanitized.Substring(0, 50);

            if (sanitized.Length < 2)
                throw new ArgumentException("Company name is too short after sanitization");

            return sanitized;
        }

        private async Task<string> GenerateUniqueSlugAsync(string baseSlug)
        {
            var slug = baseSlug;
            var counter = 1;
            const int maxAttempts = 1000;

            while (counter < maxAttempts)
            {
                // Check both ABP tenants and custom tenants
                using (_currentTenant.Change(null))
                {
                    var abpTenantExists = await _dbContext.Set<Volo.Abp.TenantManagement.Tenant>()
                        .AnyAsync(t => t.Name == slug);
                    
                    if (!abpTenantExists)
                    {
                        var customTenantExists = await _dbContext.Tenants
                            .AnyAsync(t => t.TenantSlug == slug);
                        
                        if (!customTenantExists)
                        {
                            return slug; // Unique slug found
                        }
                    }
                }

                // Try next variation
                slug = $"{baseSlug}{counter}";
                counter++;
            }

            // Fallback: add timestamp
            return $"{baseSlug}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    }
}
