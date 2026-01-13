# Trial Registration Flow - Scenario Analysis

## Overview
This document analyzes all possible scenarios that can occur during the trial registration flow, including user actions, system behaviors, and error conditions.

## Current Implementation Status

### ✅ Implemented Protections
- **CSRF Protection**: `[ValidateAntiForgeryToken]` on Register action
- **Client-Side Validation**: jQuery validation via `_ValidationScriptsPartial`
- **Server-Side Validation**: Data annotations on `TrialRegistrationModel`
- **Rollback Logic**: Tenant deletion on user creation failure
- **Exception Handling**: Try-catch with error logging
- **Structured Logging**: Telemetry at each step (TrialFormStart, TrialFormTenantCreated, etc.)

### ❌ Missing Protections
- **Rate Limiting**: Not applied to TrialController.Register (should use `[EnableRateLimiting("auth")]`)
- **CAPTCHA**: ICaptchaService exists but not used in TrialController
- **Double Submission Prevention**: No idempotency token or submit button disabling
- **Email Existence Check**: Only checks in tenant context, not host context

---

## Scenario Analysis

### 👤 User-Initiated Scenarios

#### ✅ Scenario 1: Happy Path (Success)
**Flow**: User fills form correctly → Tenant created → Admin user created → Auto-login → Redirect to onboarding

**Current Status**: ✅ Fully Implemented

**Handled By**:
- Form validation (client + server)
- Tenant creation with ABP `ITenantManager`
- User creation with ABP `IdentityUserManager`
- Automatic sign-in with `SignInManager`
- Redirect to `/onboarding/wizard/fast-start`

**Logging**:
```
TrialFormStart: Organization={Organization}, Email={Email}
TrialFormTenantCreated: TenantName={TenantName}, TenantId={TenantId}
TrialFormUserCreated: Email={Email}, UserId={UserId}, TenantId={TenantId}
TrialFormSuccess: User signed in, redirecting to onboarding
```

---

#### ❌ Scenario 2: Validation Errors (Client-Side)
**Flow**: User submits invalid data → Client-side validation blocks submission

**Current Status**: ✅ Fully Implemented

**Validation Rules**:
- OrganizationName: Required, not empty
- FullName: Required, not empty
- Email: Required, valid email format
- Password: Required, minimum 12 characters
- AcceptTerms: Required (checked)

**Handled By**: 
- jQuery validation scripts (`_ValidationScriptsPartial`)
- Data annotations on `TrialRegistrationModel`
- `asp-validation-summary="All"` in view
- `asp-validation-for` on individual fields

**User Experience**: 
- Form submission blocked before POST
- Error messages shown inline
- No server round-trip

---

#### ❌ Scenario 3: Validation Errors (Server-Side)
**Flow**: User bypasses client-side validation → Server-side validation fails → Form rerendered with errors

**Current Status**: ✅ Fully Implemented

**Handled By**:
- `ModelState.IsValid` check (line 62)
- Detailed error logging (lines 64-75)
- View rerendered with model (line 76)
- Model values preserved (user doesn't lose input)

**Logging**:
```
Trial registration validation failed. Errors:
  - OrganizationName: The Organization Name field is required
  - Email: The Email field is required
  ...
```

---

#### ❌ Scenario 4: Duplicate Email (Tenant Context)
**Flow**: Email already exists in tenant → User creation fails → Tenant rolled back → Error shown

**Current Status**: ✅ Fully Implemented

**Handled By**:
- Email existence check in tenant context (line 115)
- `ModelState.AddModelError("Email", "...")` (line 119)
- Tenant rollback via `_tenantRepository.DeleteAsync(tenant)` (line 124)
- View rerendered with error (line 127)

**Logging**:
```
TrialFormFailure: Email exists: {Email}
```

**Gap**: ⚠️ Only checks in tenant context, not host context. User could exist in host but not in tenant.

---

#### ❌ Scenario 5: Weak Password
**Flow**: Password fails Identity validation → User creation fails → Tenant rolled back → Error shown

**Current Status**: ✅ Fully Implemented

**Password Requirements** (ABP Identity defaults):
- Minimum length: 12 characters (custom requirement)
- May include uppercase, lowercase, digits, symbols (Identity requirements)

**Handled By**:
- `_userManager.CreateAsync()` validation (line 142)
- Error aggregation from `IdentityResult.Errors` (line 145)
- `ModelState.AddModelError("", ...)` for general errors (line 148)
- Tenant rollback (lines 150-154)

**Logging**:
```
TrialFormFailure: Password validation failed for {Email}. Errors: {Errors}
```

---

#### ❌ Scenario 6: CAPTCHA Failed
**Flow**: CAPTCHA validation fails → Submission blocked → Error shown

**Current Status**: ❌ **NOT IMPLEMENTED**

**Gap**: ICaptchaService exists but not used in TrialController

**Required Implementation**:
```csharp
if (_captchaService?.IsEnabled == true)
{
    var captchaToken = Request.Form["g-recaptcha-response"];
    if (!await _captchaService.ValidateCaptchaAsync(captchaToken, Request.HttpContext.Connection.RemoteIpAddress?.ToString()))
    {
        ModelState.AddModelError("", "CAPTCHA verification failed. Please try again.");
        return View("Index", model);
    }
}
```

---

#### ⚠️ Scenario 7: CSRF Token Expired
**Flow**: Form idle for too long → Anti-forgery token expires → POST rejected → 400 Bad Request

**Current Status**: ✅ Partially Implemented

**Handled By**:
- `[ValidateAntiForgeryToken]` attribute (line 53)
- `@Html.AntiForgeryToken()` in view (line 23)

**User Experience**: 
- ASP.NET Core returns 400 Bad Request
- User sees generic error page (not user-friendly)
- User must refresh form and resubmit

**Gap**: ⚠️ No custom error handling for expired CSRF tokens

---

#### ⚠️ Scenario 8: Double Submission (Race Condition)
**Flow**: User double-clicks submit → Two POST requests → Possible duplicate tenants

**Current Status**: ❌ **NOT PROTECTED**

**Risk**: 
- Two tenants created with same organization name (different timestamps)
- Two admin users created
- Orphaned tenant if first succeeds and second fails at user creation

**Required Implementation**:
1. **Client-Side**: Disable submit button on click, show loading state
2. **Server-Side**: Idempotency token or tenant name uniqueness check (already exists but not sufficient)

**Current Protection**:
- Tenant name uniqueness check (line 92-96) - adds timestamp suffix if duplicate
- **Not sufficient** - both requests could pass check before either tenant is saved

---

### 🛠️ System-Level Scenarios

#### ⚠️ Scenario 9: Tenant Creation Error (Database Failure)
**Flow**: Tenant insert fails (DB constraint, connection error) → Exception caught → Error shown

**Current Status**: ✅ Fully Implemented

**Possible Causes**:
- Database connection failure
- Unique constraint violation (tenant name)
- Null constraint violation
- Transaction timeout

**Handled By**:
- Try-catch block (line 159)
- Exception logging (lines 183-189)
- `TempData["ErrorMessage"]` shown to user (line 198)

**Logging**:
```
TrialFormException: Registration failed for {Email}. Message: {Message}
TrialFormException: Inner exception: {InnerMessage}
```

**User Experience**:
- Generic error message shown
- Form rerendered (user keeps input)
- User can retry

---

#### ⚠️ Scenario 10: Admin User Creation Failure (After Tenant)
**Flow**: Tenant created → User creation fails → Tenant rolled back → Error shown

**Current Status**: ✅ Fully Implemented

**Handled By**:
- Tenant rollback in catch block (line 124 or 153)
- Error logging
- View rerendered with error

**Gap**: ⚠️ If rollback fails (e.g., DB connection lost), orphaned tenant remains

---

#### ⚠️ Scenario 11: Email Send Failure
**Flow**: Welcome/verification email fails to send → User creation succeeds → No email sent

**Current Status**: ❌ **NOT IMPLEMENTED** (No email sending in current flow)

**Impact**: 
- User created successfully
- No welcome email
- No verification email
- User can still sign in (email confirmation not required immediately)

**Gap**: ⚠️ Email sending not implemented in TrialController

---

#### ⚠️ Scenario 12: Rate Limiting Exceeded
**Flow**: Too many registration attempts → Rate limit exceeded → 429 Too Many Requests

**Current Status**: ❌ **NOT PROTECTED**

**Gap**: 
- Rate limiting configured in `Program.cs` with "auth" policy (5 requests per 5 minutes)
- But `TrialController.Register` doesn't have `[EnableRateLimiting("auth")]` attribute

**Required Implementation**:
```csharp
[HttpPost("")]
[AllowAnonymous]
[ValidateAntiForgeryToken]
[EnableRateLimiting("auth")] // Add this
public async Task<IActionResult> Register(TrialRegistrationModel model)
```

---

#### ⚠️ Scenario 13: Slow Server Response / Timeout
**Flow**: Long-running operation → User abandons → Partial state

**Current Status**: ⚠️ **PARTIAL** (No timeout handling)

**Risks**:
- User closes browser mid-operation
- Database timeout (60 seconds configured in Program.cs)
- Network timeout

**Handled By**:
- ABP Unit of Work pattern (auto-rollback on exception)
- Database command timeout (60 seconds)

**Gap**: ⚠️ No explicit timeout handling for long-running operations

---

#### 🧼 Scenario 14: XSS/Input Injection Attack
**Flow**: Malicious script in form fields → Razor encoding prevents execution

**Current Status**: ✅ Fully Protected

**Protections**:
- Razor HTML encoding (automatic)
- ABP input validation
- ASP.NET Core request validation

**Example Attack**: 
```html
<input value="<script>alert('XSS')</script>">
```

**Protection**: Razor automatically encodes output:
```html
<input value="&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;">
```

---

#### 🧼 Scenario 15: SQL Injection Attack
**Flow**: SQL code in form fields → EF Core parameterized queries prevent injection

**Current Status**: ✅ Fully Protected

**Protections**:
- EF Core parameterized queries (automatic)
- ABP repository pattern
- No raw SQL queries in TrialController

**Example Attack**:
```
OrganizationName: "'; DROP TABLE Tenants; --"
```

**Protection**: EF Core uses parameterized queries:
```sql
SELECT * FROM "Tenants" WHERE "Name" = @p0
-- @p0 = "'; DROP TABLE Tenants; --"
```

---

### 🔁 Edge Cases

#### 🔁 Scenario 16: Retry After Error
**Flow**: User sees error → Fixes input → Submits again

**Current Status**: ✅ Fully Supported

**Handled By**:
- Model values preserved on error
- Form rerendered with user input
- Validation errors shown inline

---

#### 🔁 Scenario 17: Multi-Tab Submission
**Flow**: User opens form in two tabs → Submits both → Race condition

**Current Status**: ❌ **NOT PROTECTED**

**Risk**: Same as double submission (Scenario 8)

**Required Implementation**: Same as Scenario 8 (idempotency token)

---

#### 🔁 Scenario 18: Browser Back Button
**Flow**: User submits form → Redirected → Clicks back → Resubmits form

**Current Status**: ✅ Protected (CSRF token prevents resubmission)

**Protection**:
- CSRF token is one-time use
- Back button resubmission fails CSRF validation

---

## Summary: Scenario Coverage

| Scenario | Status | Risk Level | Priority to Fix |
|----------|--------|------------|-----------------|
| 1. Happy Path | ✅ Complete | - | - |
| 2. Validation Errors (Client) | ✅ Complete | - | - |
| 3. Validation Errors (Server) | ✅ Complete | - | - |
| 4. Duplicate Email | ⚠️ Partial | Medium | Medium |
| 5. Weak Password | ✅ Complete | - | - |
| 6. CAPTCHA Failed | ❌ Missing | Low | Low |
| 7. CSRF Expired | ⚠️ Partial | Low | Low |
| 8. Double Submission | ❌ Missing | **High** | **High** |
| 9. Tenant Creation Error | ✅ Complete | - | - |
| 10. User Creation Failure | ✅ Complete | - | - |
| 11. Email Send Failure | ❌ Missing | Low | Low |
| 12. Rate Limiting | ❌ Missing | **High** | **High** |
| 13. Timeout | ⚠️ Partial | Medium | Medium |
| 14. XSS Attack | ✅ Complete | - | - |
| 15. SQL Injection | ✅ Complete | - | - |
| 16. Retry After Error | ✅ Complete | - | - |
| 17. Multi-Tab Submission | ❌ Missing | High | High |
| 18. Browser Back Button | ✅ Complete | - | - |

---

## Recommended Fixes (Priority Order)

### 🔴 High Priority

1. **Add Rate Limiting** (Scenario 12)
   - Add `[EnableRateLimiting("auth")]` to `TrialController.Register`
   - Effort: 5 minutes

2. **Prevent Double Submission** (Scenarios 8, 17)
   - Client-side: Disable submit button, show loading
   - Server-side: Add idempotency token or improve tenant name uniqueness check
   - Effort: 2 hours

### 🟡 Medium Priority

3. **Improve Duplicate Email Check** (Scenario 4)
   - Check email in both host and tenant context
   - Effort: 30 minutes

4. **Add Timeout Handling** (Scenario 13)
   - Explicit timeout handling for long-running operations
   - Effort: 1 hour

### 🟢 Low Priority

5. **Add CAPTCHA** (Scenario 6)
   - Integrate existing `ICaptchaService`
   - Effort: 1 hour

6. **Add Email Sending** (Scenario 11)
   - Send welcome email after registration
   - Effort: 2 hours

7. **Improve CSRF Error Handling** (Scenario 7)
   - Custom error page for expired CSRF tokens
   - Effort: 30 minutes

---

## Testing Recommendations

### Unit Tests
- Model validation (all fields)
- Password validation rules
- Tenant name generation

### Integration Tests
- Happy path (full flow)
- Duplicate email handling
- Rollback on user creation failure
- Rate limiting enforcement

### E2E Tests
- Form submission flow
- Error handling and retry
- Double submission prevention
- CSRF token validation

---

## Monitoring & Observability

### Key Metrics to Track
- Registration success rate
- Validation error rate (by field)
- Duplicate email attempts
- Rate limit violations
- Tenant creation failures
- User creation failures
- Average registration time

### Logging Events
- `TrialFormStart`: Form submitted
- `TrialFormTenantCreated`: Tenant created
- `TrialFormUserCreated`: User created
- `TrialFormSuccess`: Registration successful
- `TrialFormFailure`: Validation/creation failure
- `TrialFormException`: Exception caught
- `TrialFormDuplicateTenant`: Duplicate tenant name
- `TrialFormRateLimited`: Rate limit exceeded

---

## References

- **Controller**: `src/GrcMvc/Controllers/TrialController.cs`
- **View**: `src/GrcMvc/Views/Trial/Index.cshtml`
- **Model**: `TrialRegistrationModel` (in TrialController.cs)
- **Rate Limiting**: `Program.cs` (line 488-521)
- **CAPTCHA Service**: `Services/Interfaces/ICaptchaService.cs`
- **View Scenarios**: `TRIAL_VIEW_SCENARIOS.md`
