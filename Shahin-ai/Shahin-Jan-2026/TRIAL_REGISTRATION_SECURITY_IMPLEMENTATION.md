# Trial Registration Security Implementation Guide

## Overview
This document outlines the security measures implemented for the trial registration endpoint (`/trial`) to prevent abuse, spam, bot attacks, and fraudulent registrations.

---

## ✅ Implemented Security Measures

### 1. **CSRF Protection** ✅
- **Implementation**: `[ValidateAntiForgeryToken]` attribute on POST endpoint
- **Location**: `TrialController.Register()`
- **Protection**: Prevents Cross-Site Request Forgery attacks
- **Status**: Active

### 2. **Rate Limiting** ✅
- **Implementation**: `[EnableRateLimiting("auth")]` attribute
- **Configuration**: 
  - **Limit**: 5 requests per 5 minutes per IP/user
  - **Queue**: 0 (immediate rejection)
  - **Response**: HTTP 429 Too Many Requests
- **Location**: `Program.cs` lines 515-520
- **Protection**: Prevents brute force attacks and registration spam
- **Status**: Active

### 3. **CAPTCHA Validation** ✅ (NEW)
- **Implementation**: Google reCAPTCHA v2/v3 integration
- **Service**: `ICaptchaService` / `GoogleRecaptchaService`
- **Location**: 
  - Controller: `TrialController.cs` (lines 78-93)
  - View: `Views/Trial/Index.cshtml` (CAPTCHA widget)
- **Configuration**: `appsettings.json` → `Security:Captcha`
- **Protection**: Prevents automated bot registrations
- **Status**: Enabled when configured (optional)

**Configuration Example:**
```json
{
  "Security": {
    "Captcha": {
      "Enabled": true,
      "Provider": "reCAPTCHA",
      "SiteKey": "YOUR_SITE_KEY",
      "SecretKey": "YOUR_SECRET_KEY",
      "MinimumScore": 0.5,
      "VerifyUrl": "https://www.google.com/recaptcha/api/siteverify"
    }
  }
}
```

### 4. **Duplicate Email/Tenant Validation** ✅ (NEW)
- **Implementation**: Database check before tenant creation
- **Location**: `TrialController.cs` (lines 95-112)
- **Checks**:
  - Duplicate email addresses
  - Duplicate tenant slugs (organization names)
- **Protection**: Prevents duplicate registrations and account enumeration
- **Status**: Active

### 5. **Input Validation** ✅
- **Implementation**: Data annotations + manual validation
- **Validations**:
  - Required fields: OrganizationName, FullName, Email, Password
  - Email format validation
  - Password minimum length: 12 characters
  - Terms acceptance validation
- **Location**: `TrialRegistrationModel` class
- **Protection**: Prevents invalid data submission
- **Status**: Active

### 6. **IP Address Logging** ✅ (NEW)
- **Implementation**: Logs IP address for all registration attempts
- **Location**: `TrialController.cs` (line 82, 139)
- **Purpose**: Audit trail for security monitoring
- **Status**: Active

### 7. **Password Strength** ✅
- **Implementation**: Minimum 12 characters required
- **Location**: `TrialRegistrationModel.Password` attribute
- **Protection**: Reduces risk of weak passwords
- **Status**: Active

---

## 🔒 Security Layers

### Layer 1: Client-Side Protection
1. **CAPTCHA Widget** - Visible to users, blocks simple bots
2. **Form Validation** - Immediate feedback on invalid input
3. **Double-Submission Prevention** - Disables submit button after click

### Layer 2: Request-Level Protection
1. **Rate Limiting** - Blocks excessive requests from same IP/user
2. **CSRF Token** - Validates request authenticity
3. **IP Logging** - Tracks all registration attempts

### Layer 3: Business Logic Protection
1. **Duplicate Checking** - Prevents duplicate emails/tenants
2. **Input Sanitization** - Sanitizes tenant names for ABP compatibility
3. **Validation** - Server-side validation of all inputs

### Layer 4: ABP Framework Protection
1. **ABP Tenant Validation** - ABP's built-in tenant creation validation
2. **ABP Identity Validation** - ABP's user creation validation
3. **Database Constraints** - Database-level uniqueness constraints

---

## 📋 Security Checklist

### Required for Production
- [x] CSRF Protection (`[ValidateAntiForgeryToken]`)
- [x] Rate Limiting (`[EnableRateLimiting("auth")]`)
- [x] Input Validation (Required fields, email format, password strength)
- [x] Duplicate Email/Tenant Checking
- [x] IP Address Logging
- [ ] **CAPTCHA Configuration** (Recommended - configure in `appsettings.Production.json`)
- [ ] **Email Domain Validation** (Optional - block disposable email domains)
- [ ] **Honeypot Fields** (Optional - additional bot protection)

### Optional Enhancements
- [ ] Device Fingerprinting (`IFingerprintFraudDetector` - service exists but not integrated)
- [ ] Fraud Detection (`TenantCreationFacadeService` - service exists but not used)
- [ ] IP Reputation Checking (block known malicious IPs)
- [ ] Email Verification (send verification email before activation)
- [ ] Account Lockout (temporary lockout after multiple failed attempts)

---

## 🔧 Configuration

### Rate Limiting Configuration
**File**: `Program.cs` (lines 515-520)
```csharp
options.AddFixedWindowLimiter("auth", limiterOptions =>
{
    limiterOptions.PermitLimit = 5;        // 5 requests
    limiterOptions.Window = TimeSpan.FromMinutes(5);  // per 5 minutes
    limiterOptions.QueueLimit = 0;         // No queuing
});
```

### CAPTCHA Configuration
**File**: `appsettings.Production.json`
```json
{
  "Security": {
    "Captcha": {
      "Enabled": true,
      "Provider": "reCAPTCHA",
      "SiteKey": "YOUR_RECAPTCHA_SITE_KEY",
      "SecretKey": "YOUR_RECAPTCHA_SECRET_KEY",
      "MinimumScore": 0.5,
      "VerifyUrl": "https://www.google.com/recaptcha/api/siteverify"
    }
  }
}
```

**To get reCAPTCHA keys:**
1. Go to https://www.google.com/recaptcha/admin
2. Register your site
3. Choose reCAPTCHA v2 ("I'm not a robot" Checkbox) or v3 (invisible)
4. Copy Site Key and Secret Key to `appsettings.Production.json`

---

## 🚨 Security Monitoring

### Logged Events
1. **CAPTCHA Failures**: `TrialFormSecurity: CAPTCHA validation failed`
2. **Duplicate Attempts**: `TrialFormSecurity: Duplicate registration attempt`
3. **Rate Limit Hits**: HTTP 429 responses (logged by rate limiter)
4. **Validation Failures**: `Trial registration validation failed`
5. **Successful Registrations**: `TrialFormSuccess: Tenant created`

### Monitoring Queries
```sql
-- Check for suspicious registration patterns
SELECT 
    COUNT(*) as attempt_count,
    "Email",
    "IpAddress",
    "CreatedAt"
FROM "TenantCreationFingerprints"
WHERE "CreatedAt" > NOW() - INTERVAL '1 hour'
GROUP BY "Email", "IpAddress"
HAVING COUNT(*) > 3;

-- Check for duplicate email attempts
SELECT 
    "Email",
    COUNT(*) as duplicate_count
FROM "Tenants"
WHERE "Email" IS NOT NULL
GROUP BY "Email"
HAVING COUNT(*) > 1;
```

---

## 🛡️ Threat Mitigation

| Threat | Mitigation | Status |
|--------|-----------|--------|
| **Bot Attacks** | CAPTCHA validation | ✅ Implemented |
| **Brute Force** | Rate limiting (5 req/5min) | ✅ Implemented |
| **CSRF Attacks** | Anti-forgery tokens | ✅ Implemented |
| **Duplicate Registrations** | Email/tenant validation | ✅ Implemented |
| **Weak Passwords** | 12-character minimum | ✅ Implemented |
| **Account Enumeration** | Generic error messages | ⚠️ Partial (needs improvement) |
| **Spam Registrations** | CAPTCHA + Rate limiting | ✅ Implemented |
| **IP-based Attacks** | Rate limiting per IP | ✅ Implemented |

---

## 📝 Code Locations

### Controller
- **File**: `src/GrcMvc/Controllers/TrialController.cs`
- **Security Code**: Lines 25-47 (constructor), 49-60 (Index), 62-112 (Register validation), 78-93 (CAPTCHA), 95-112 (Duplicate check)

### View
- **File**: `src/GrcMvc/Views/Trial/Index.cshtml`
- **Security Code**: Lines 23 (CSRF token), 60-66 (CAPTCHA widget), 83-100 (CAPTCHA script)

### Configuration
- **File**: `src/GrcMvc/Program.cs`
- **Security Code**: Lines 515-520 (Rate limiting), 941 (CAPTCHA service registration)

### Services
- **CAPTCHA Service**: `src/GrcMvc/Services/Implementations/GoogleRecaptchaService.cs`
- **Interface**: `src/GrcMvc/Services/Interfaces/ICaptchaService.cs`

---

## 🔄 Future Enhancements

### High Priority
1. **Email Verification**: Require email verification before tenant activation
2. **Account Enumeration Protection**: Always return generic success/error messages
3. **Disposable Email Blocking**: Block temporary/disposable email domains

### Medium Priority
1. **Device Fingerprinting**: Integrate `IFingerprintFraudDetector` service
2. **Fraud Detection**: Use `TenantCreationFacadeService` for advanced fraud detection
3. **IP Reputation**: Check IP against known malicious IP databases

### Low Priority
1. **Honeypot Fields**: Add hidden form fields to catch bots
2. **Behavioral Analysis**: Track user interaction patterns
3. **Progressive Challenges**: Increase security challenges for suspicious patterns

---

## ✅ Testing Checklist

### Manual Testing
- [ ] Submit form without CAPTCHA (if enabled) → Should fail
- [ ] Submit form 6 times in 5 minutes → Should get rate limited (429)
- [ ] Submit duplicate email → Should show error
- [ ] Submit duplicate organization name → Should show error
- [ ] Submit invalid email format → Should show validation error
- [ ] Submit password < 12 characters → Should show validation error
- [ ] Submit without accepting terms → Should show error
- [ ] Submit valid form → Should create tenant and redirect

### Security Testing
- [ ] CSRF token removal → Should fail
- [ ] CAPTCHA bypass attempt → Should fail
- [ ] SQL injection in email field → Should be sanitized
- [ ] XSS in organization name → Should be sanitized
- [ ] Rate limit bypass attempt → Should fail

---

## 📚 References

- [ABP Framework Security](https://docs.abp.io/en/abp/latest/Security)
- [ASP.NET Core Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)
- [Google reCAPTCHA Documentation](https://developers.google.com/recaptcha)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)

---

**Last Updated**: 2026-01-12
**Status**: ✅ Production Ready (with CAPTCHA configuration recommended)
