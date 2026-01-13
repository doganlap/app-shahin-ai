# 📋 Trial Registration Flow - Complete Report

**Date:** 2026-01-22  
**Endpoint:** `POST /trial/register`  
**Question:** What should happen after registration?

---

## 🔄 Complete Registration Flow

### Step 1: User Submits Registration Form
**URL:** `http://localhost:5137/trial/register`  
**Method:** `POST`  
**Data:**
- Organization Name
- Full Name
- Email
- Password
- Accept Terms (checkbox)

---

### Step 2: Server-Side Processing (TrialController.Register)

#### 2.1. Validation
- ✅ Check `AcceptTerms` is true
- ✅ Validate email format
- ✅ Check for duplicate email/tenant name
- ✅ Validate password strength
- ✅ Model validation

#### 2.2. Create ABP Tenant
```csharp
// Line 124: Create tenant using ABP's ITenantAppService
tenantDto = await _tenantAppService.CreateAsync(createDto);
```
**Creates:**
- ABP Tenant in `AbpTenants` table
- ABP Admin User in `AbpUsers` table
- ABP Role assignment

#### 2.3. Create Custom Tenant Record
```csharp
// Line 202: Create custom Tenant entity
_dbContext.Tenants.Add(customTenant);
```
**Creates:**
- Custom `Tenant` record in `Tenants` table
- Links to ABP tenant (same ID)
- Sets `OnboardingStatus = "NOT_STARTED"`

#### 2.4. Create OnboardingWizard
```csharp
// Line 144-179: Create OnboardingWizard entity
var wizard = new OnboardingWizard
{
    Id = Guid.NewGuid(),
    TenantId = tenantDto.Id,
    WizardStatus = "InProgress",
    CurrentStep = 1,
    IsCompleted = false,
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
};
_dbContext.OnboardingWizards.Add(wizard);
```
**Creates:**
- `OnboardingWizard` record
- `WizardStatus = "InProgress"`
- `CurrentStep = 1`

#### 2.5. Create TenantUser Linkage
```csharp
// Line 213-225: Create TenantUser record
var tenantUser = new TenantUser
{
    TenantId = tenantDto.Id,
    UserId = abpUser.Id.ToString(),
    RoleCode = "TenantAdmin",
    Status = "Active"
};
_dbContext.TenantUsers.Add(tenantUser);
```
**Creates:**
- Links ABP user to custom tenant
- Sets role as `TenantAdmin`

#### 2.6. Save All Changes
```csharp
// Line 235: Save to database
await _dbContext.SaveChangesAsync();
```

#### 2.7. Store TempData for Onboarding
```csharp
// Line 241-245: Store tenant info in TempData
TempData["TenantId"] = tenantDto.Id.ToString();
TempData["TenantSlug"] = sanitizedTenantName;
TempData["OrganizationName"] = model.OrganizationName;
TempData["AdminEmail"] = model.Email;
TempData["WelcomeMessage"] = $"مرحباً بك في {model.OrganizationName}! لنبدأ إعداد منظمتك.";
```

#### 2.8. Auto-Login User
```csharp
// Line 248-261: Auto-sign in the user
await _signInManager.SignInAsync(abpUser, isPersistent: true);
```
**Result:**
- User is automatically logged in
- No manual login required
- Session created with tenant context

---

### Step 3: Redirect to Onboarding Wizard

#### 3.1. Redirect Action
```csharp
// Line 266: Redirect to OnboardingWizard
return RedirectToAction("Index", "OnboardingWizard", new { tenantId = tenantDto.Id });
```

#### 3.2. Expected URL
**Route:** `/OnboardingWizard/Index?tenantId={guid}`

**Or based on routing:**
- `/OnboardingWizard?tenantId={guid}`
- `/t/{tenantSlug}/onboarding/start` (if tenant-based routing)

---

## 📊 Database Records Created

After successful registration, the following records are created:

### 1. ABP Framework Tables (GrcMvcDb)

| Table | Record | Details |
|-------|--------|---------|
| `AbpTenants` | 1 tenant | ABP tenant with name = sanitized org name |
| `AbpUsers` | 1 user | Admin user with email and password |
| `AbpRoles` | 1 role | TenantAdmin role (if not exists) |
| `AbpUserRoles` | 1 link | User assigned to TenantAdmin role |

### 2. Custom Application Tables (GrcMvcDb)

| Table | Record | Details |
|-------|--------|---------|
| `Tenants` | 1 tenant | Custom tenant with full org details |
| `TenantUsers` | 1 link | Links ABP user to custom tenant |
| `OnboardingWizards` | 1 wizard | Onboarding wizard initialized at step 1 |

---

## 🎯 What User Sees After Registration

### Success Flow:
1. **Form Submission** → User clicks "Register" button
2. **Processing** → Server creates tenant, user, and wizard
3. **Auto-Login** → User is automatically signed in
4. **Redirect** → User is redirected to Onboarding Wizard
5. **Onboarding Wizard** → User sees step 1 of 12-step onboarding

### Expected Redirect URL:
```
/OnboardingWizard/Index?tenantId={guid}
```

### Onboarding Wizard Features:
- **12-step comprehensive wizard**
- **Current Step:** 1 (first step)
- **Status:** InProgress
- **Welcome Message:** "مرحباً بك في {OrganizationName}! لنبدأ إعداد منظمتك."

---

## ⚠️ Error Handling

### If Registration Fails:

#### 1. Validation Errors
- **Action:** Return to registration form
- **Display:** Validation error messages
- **Status:** 200 OK (form with errors)

#### 2. Duplicate Email/Tenant
- **Action:** Return to registration form
- **Error:** "This email is already registered" or "Organization name already taken"
- **Status:** 200 OK (form with error)

#### 3. Database/Server Error
- **Action:** Return to registration form
- **Error:** "Registration failed. Please try again later."
- **Status:** 200 OK (form with error)
- **Logs:** Full exception logged

---

## 🔍 Code References

### Key Files:
- **Controller:** `src/GrcMvc/Controllers/TrialController.cs`
  - **Method:** `Register()` (Line 63-298)
  - **Redirect:** Line 266

- **Onboarding Controller:** `src/GrcMvc/Controllers/OnboardingWizardController.cs`
  - **Action:** `Index(tenantId)`

### Key Database Operations:
1. **Line 124:** `_tenantAppService.CreateAsync()` - Create ABP tenant
2. **Line 202:** `_dbContext.Tenants.Add()` - Create custom tenant
3. **Line 173:** `_dbContext.OnboardingWizards.Add()` - Create wizard
4. **Line 225:** `_dbContext.TenantUsers.Add()` - Create user link
5. **Line 235:** `_dbContext.SaveChangesAsync()` - Save all
6. **Line 252:** `_signInManager.SignInAsync()` - Auto-login
7. **Line 266:** `RedirectToAction()` - Redirect to onboarding

---

## ✅ Summary

**After `/trial/register` POST:**

1. ✅ **ABP Tenant Created** → In `AbpTenants` table
2. ✅ **ABP User Created** → In `AbpUsers` table  
3. ✅ **Custom Tenant Created** → In `Tenants` table
4. ✅ **OnboardingWizard Created** → Step 1, Status: InProgress
5. ✅ **TenantUser Link Created** → Links user to tenant
6. ✅ **User Auto-Logged In** → Session created
7. ✅ **Redirect to Onboarding** → `/OnboardingWizard/Index?tenantId={guid}`

**Expected Result:**
- User is redirected to the **12-step Onboarding Wizard**
- User is **automatically logged in**
- User can start configuring their organization
- No manual login required

---

## 🧪 Testing Checklist

- [ ] Submit registration form
- [ ] Verify tenant created in database
- [ ] Verify user created in ABP
- [ ] Verify onboarding wizard created
- [ ] Verify user is auto-logged in
- [ ] Verify redirect to onboarding wizard
- [ ] Verify onboarding wizard displays step 1
- [ ] Verify welcome message displays

---

## 📝 Notes

- **Onboarding Status:** Set to `"NOT_STARTED"` initially
- **Wizard Status:** Set to `"InProgress"` when wizard is created
- **Current Step:** Starts at `1`
- **TempData:** Used to pass tenant info to onboarding wizard
- **Auto-Login:** Uses ABP Identity's `SignInManager`
- **Redirect:** Uses `RedirectToAction` with `tenantId` parameter
