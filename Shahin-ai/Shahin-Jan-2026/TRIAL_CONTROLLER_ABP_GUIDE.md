# /trial Controller - ABP Framework Integration Guide

## ✅ Current Status: Fully Integrated!

The `/trial` controller is **already fully integrated** with ABP Framework 8.3.6. It uses ABP's built-in services for tenant and user management.

---

## 🏗️ Architecture Overview

### ABP Services Used

| Service | Purpose | Location |
|--------|---------|----------|
| `ITenantAppService` | Creates ABP tenant + admin user automatically | Line 26 |
| `IIdentityUserRepository` | Finds ABP identity users | Line 27 |
| `ICurrentTenant` | Switches tenant context for queries | Line 28 |
| `SignInManager<IdentityUser>` | Auto-login with ABP identity | Line 29 |
| `GrcDbContext` | Custom tenant table sync | Line 30 |

### Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│ 1. User submits /trial form                                 │
│    • Organization Name: "Acme Corp"                          │
│    • Email: "admin@acme.com"                               │
│    • Password: "SecurePass123!"                            │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│ 2. Sanitize tenant name                                      │
│    "Acme Corp" → "acme-corp"                                │
│    Check for duplicates in custom Tenants table             │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│ 3. Create ABP Tenant (ITenantAppService.CreateAsync)        │
│    ✅ Creates AbpTenants record                             │
│    ✅ Creates AbpUsers record (admin user)                │
│    ✅ Creates AbpRoles record (admin role)                 │
│    ✅ Creates AbpUserRoles linkage                          │
│    ✅ All in one atomic operation!                          │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│ 4. Create OnboardingWizard                                   │
│    • TenantId = ABP tenant ID                              │
│    • WizardStatus = "InProgress"                            │
│    • CurrentStep = 1                                        │
│    • 12-step wizard configuration                           │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│ 5. Create Custom Tenant Record                              │
│    • Id = ABP tenant ID (same GUID)                        │
│    • TenantSlug = "acme-corp"                               │
│    • IsTrial = true                                         │
│    • TrialEndsAt = +7 days                                  │
│    • OnboardingStatus = "NOT_STARTED"                       │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│ 6. Create TenantUser Linkage                                │
│    • TenantId ↔ UserId mapping                              │
│    • RoleCode = "TenantAdmin"                               │
│    • Status = "Active"                                      │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│ 7. Auto-Login User                                           │
│    • SignInManager.SignInAsync(abpUser)                     │
│    • Tenant context automatically set                        │
│    • ABP tenant resolution works immediately                │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│ 8. Redirect to Onboarding Wizard                            │
│    → /OnboardingWizard/Index?tenantId={guid}                │
│    • User must complete 12 steps                            │
│    • OnboardingEnforcementMiddleware blocks workspace        │
└─────────────────────────────────────────────────────────────┘
```

---

## 📋 Code Walkthrough

### Step 1: Controller Dependencies (Lines 26-31)

```csharp
private readonly ITenantAppService _tenantAppService;        // ABP tenant creation
private readonly IIdentityUserRepository _userRepository;     // ABP user lookup
private readonly ICurrentTenant _currentTenant;              // Tenant context switching
private readonly AspNetSignInManager _signInManager;         // ABP identity sign-in
private readonly GrcDbContext _dbContext;                    // Custom tenant table
```

**Key Point:** All ABP services are injected via dependency injection. No manual configuration needed!

### Step 2: Tenant Name Sanitization (Lines 303-335)

```csharp
private string SanitizeTenantName(string organizationName)
{
    // "Acme Corp & Co.!" → "acme-corp-co"
    var sanitized = organizationName
        .ToLowerInvariant()
        .Replace(" ", "-")
        .Replace(".", "")
        .Replace(",", "")
        .Replace("_", "-");
    
    // Remove non-alphanumeric (except hyphens)
    sanitized = new string(sanitized
        .Where(c => char.IsLetterOrDigit(c) || c == '-')
        .ToArray());
    
    // Limit to 50 characters
    if (sanitized.Length > 50)
        sanitized = sanitized.Substring(0, 50).Trim('-');
    
    return sanitized;
}
```

**Why:** ABP tenant names must be URL-safe slugs.

### Step 3: ABP Tenant Creation (Lines 111-135)

```csharp
var createDto = new TenantCreateDto
{
    Name = sanitizedTenantName,        // "acme-corp"
    AdminEmailAddress = model.Email,   // "admin@acme.com"
    AdminPassword = model.Password       // "SecurePass123!"
};

// ABP automatically creates:
// ✅ AbpTenants record
// ✅ AbpUsers record (admin user)
// ✅ AbpRoles record (admin role)
// ✅ AbpUserRoles linkage
// ✅ AbpUserClaims (if needed)
var tenantDto = await _tenantAppService.CreateAsync(createDto);
```

**Key Point:** `ITenantAppService.CreateAsync()` is **atomic** - it creates tenant + admin user + role in one transaction!

### Step 4: Find ABP User (Lines 205-209)

```csharp
IdentityUser? abpUser = null;
using (_currentTenant.Change(tenantDto.Id))  // Switch to new tenant context
{
    // ABP automatically scopes queries to current tenant
    abpUser = await _userRepository.FindByNormalizedEmailAsync(
        model.Email.ToUpperInvariant()
    );
}
```

**Why `ICurrentTenant.Change()`?** ABP's `IIdentityUserRepository` is tenant-aware. We must switch context to find the user in the new tenant.

### Step 5: Auto-Login (Lines 248-261)

```csharp
if (abpUser != null)
{
    await _signInManager.SignInAsync(abpUser, isPersistent: true);
    // ✅ User is now logged in
    // ✅ Tenant context is automatically set via ABP middleware
    // ✅ TenantId claim is added by ClaimsTransformationService
}
```

**Key Point:** ABP's `SignInManager` automatically sets tenant context. No manual claim addition needed!

### Step 6: Redirect to Onboarding (Line 266)

```csharp
return RedirectToAction("Index", "OnboardingWizard", 
    new { tenantId = tenantDto.Id });
```

**What happens next:**
1. `OnboardingEnforcementMiddleware` checks `Tenant.OnboardingStatus`
2. If `!= "COMPLETED"` → User must complete wizard
3. After completion → Workspace access granted

---

## 🗄️ Database Records Created

After successful registration, you'll find:

### 1. ABP Tables (Created by ITenantAppService)

```sql
-- AbpTenants
SELECT * FROM "AbpTenants" WHERE "Name" = 'acme-corp';
-- Result: {Id: guid, Name: 'acme-corp', ...}

-- AbpUsers (admin user)
SELECT * FROM "AbpUsers" WHERE "Email" = 'admin@acme.com';
-- Result: {Id: guid, Email: 'admin@acme.com', TenantId: {guid}, ...}

-- AbpRoles (admin role)
SELECT * FROM "AbpRoles" WHERE "Name" = 'admin';
-- Result: {Id: guid, Name: 'admin', TenantId: {guid}, ...}

-- AbpUserRoles (linkage)
SELECT * FROM "AbpUserRoles" 
WHERE "UserId" = '{user-guid}' AND "RoleId" = '{role-guid}';
```

### 2. Custom Tables (Created by TrialController)

```sql
-- Tenants (custom GRC table)
SELECT * FROM "Tenants" WHERE "TenantSlug" = 'acme-corp';
-- Result: {Id: {same-guid}, TenantSlug: 'acme-corp', IsTrial: true, ...}

-- OnboardingWizards
SELECT * FROM "OnboardingWizards" WHERE "TenantId" = '{tenant-guid}';
-- Result: {Id: guid, TenantId: {guid}, WizardStatus: 'InProgress', ...}

-- TenantUsers (linkage)
SELECT * FROM "TenantUsers" 
WHERE "TenantId" = '{tenant-guid}' AND "UserId" = '{user-guid}';
```

---

## 🧪 Testing the /trial Controller

### Quick Test

```bash
# 1. Start application
cd src/GrcMvc
dotnet run

# 2. Navigate to trial page
open http://localhost:5010/trial

# 3. Fill form:
#    Organization: "Test Company"
#    Full Name: "John Doe"
#    Email: "john@testcompany.com"
#    Password: "SecurePass123!"
#    Accept Terms: ✓

# 4. Submit form

# 5. Verify:
#    ✅ Auto-login successful
#    ✅ Redirect to onboarding wizard
#    ✅ ABP tenant created
#    ✅ Custom tenant created
#    ✅ User can access onboarding
```

### Database Verification

```sql
-- Check ABP tenant
SELECT 
    t."Id",
    t."Name",
    t."IsActive",
    t."CreationTime"
FROM "AbpTenants" t
WHERE t."Name" = 'test-company';

-- Check ABP user
SELECT 
    u."Id",
    u."Email",
    u."TenantId",
    u."IsActive"
FROM "AbpUsers" u
WHERE u."Email" = 'john@testcompany.com';

-- Check custom tenant
SELECT 
    t."Id",
    t."TenantSlug",
    t."OrganizationName",
    t."IsTrial",
    t."OnboardingStatus"
FROM "Tenants" t
WHERE t."TenantSlug" = 'test-company';

-- Verify linkage
SELECT 
    tu."TenantId",
    tu."UserId",
    tu."RoleCode",
    tu."Status"
FROM "TenantUsers" tu
JOIN "Tenants" t ON tu."TenantId" = t."Id"
WHERE t."TenantSlug" = 'test-company';
```

---

## 🔍 Key Differences: /trial vs /SignupNew

| Feature | /trial Controller | /SignupNew Razor Page |
|---------|-------------------|----------------------|
| **ABP Service** | `ITenantAppService` | `ITenantManager` |
| **Tenant Creation** | ABP-first (atomic) | ABP-first (manual) |
| **User Creation** | Automatic (by ABP) | Manual (after tenant) |
| **Error Handling** | Try-catch with rollback | Transaction rollback |
| **UI** | Standard MVC form | Modern Razor Page |
| **Route** | `/trial` | `/SignupNew` |
| **Use Case** | Production-ready | New implementations |

**Recommendation:** Use `/trial` for production - it's more robust and uses ABP's atomic tenant creation!

---

## 🚀 Advantages of Current Implementation

### ✅ ABP-First Approach

- **Atomic Operations:** `ITenantAppService.CreateAsync()` creates tenant + user + role in one transaction
- **No Manual Rollback:** If tenant creation fails, nothing is created
- **Automatic User Management:** ABP handles user creation, password hashing, email confirmation
- **Tenant Context:** ABP automatically sets tenant context after login

### ✅ Dual Tenant Management

- **ABP Tables:** For ABP Framework features (permissions, settings, audit)
- **Custom Tables:** For GRC-specific fields (trial dates, onboarding status, etc.)
- **Synchronized:** Both use same GUID for consistency

### ✅ Security Features

- **Rate Limiting:** `[EnableRateLimiting("auth")]` prevents abuse
- **CAPTCHA Support:** Optional bot protection
- **Duplicate Checking:** Prevents email/tenant name conflicts
- **Password Policy:** Minimum 12 characters enforced

---

## 📝 Configuration Requirements

### 1. ABP Module Configuration (Already Done)

The `GrcMvcModule.cs` already configures:

```csharp
// Replace ABP DbContexts with GrcDbContext
options.ReplaceDbContext<ITenantManagementDbContext>();
options.ReplaceDbContext<IIdentityDbContext>();
options.ReplaceDbContext<IPermissionManagementDbContext>();
```

### 2. Program.cs Registration (Already Done)

ABP services are automatically registered via `GrcMvcWebModule`.

### 3. Database Migration (Required)

```bash
# Create migration for ABP tables
cd src/GrcMvc
dotnet ef migrations add AddAbpFrameworkTables

# Apply migration
dotnet ef database update
```

---

## 🐛 Troubleshooting

### Issue: "ITenantAppService not found"

**Solution:** Ensure ABP module is properly configured:

```csharp
// In GrcMvcModule.cs
[DependsOn(typeof(AbpTenantManagementApplicationModule))]
public class GrcMvcWebModule : AbpModule
{
    // ...
}
```

### Issue: "User not found after tenant creation"

**Solution:** Use `ICurrentTenant.Change()` to switch context:

```csharp
using (_currentTenant.Change(tenantDto.Id))
{
    abpUser = await _userRepository.FindByNormalizedEmailAsync(email);
}
```

### Issue: "Tenant context not set after login"

**Solution:** ABP's `SignInManager` automatically sets tenant context. Verify:

1. `ClaimsTransformationService` is registered
2. ABP middleware is in pipeline
3. User has `TenantId` claim

---

## ✅ Summary

The `/trial` controller is **production-ready** and fully integrated with ABP Framework:

- ✅ Uses `ITenantAppService` for atomic tenant creation
- ✅ Automatically creates admin user + role
- ✅ Syncs with custom `Tenants` table
- ✅ Auto-logs in user with tenant context
- ✅ Redirects to mandatory onboarding wizard
- ✅ Includes security features (rate limiting, CAPTCHA, validation)

**No additional implementation needed!** Just test it and use it! 🚀
