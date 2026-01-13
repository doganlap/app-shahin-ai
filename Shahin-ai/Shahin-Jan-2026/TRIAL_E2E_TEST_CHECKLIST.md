# Trial Registration - End-to-End Test Checklist

## Overview
This checklist provides step-by-step manual testing procedures for the Trial Registration flow.

## Prerequisites
- Application running (localhost or test environment)
- Database accessible
- Clean test data (or ability to clean up after tests)

---

## Test Scenarios

### ✅ Scenario 1: Happy Path (Success)

**Steps:**
1. Navigate to `/trial`
2. Fill in form:
   - Organization Name: `Test Organization`
   - Full Name: `Test User`
   - Email: `testuser@example.com`
   - Password: `ValidPassword123!` (12+ characters)
   - Accept Terms: ✓ checked
3. Click "Start Free Trial"

**Expected Results:**
- ✅ Form submits successfully
- ✅ Tenant created in database
- ✅ Admin user created
- ✅ User automatically signed in
- ✅ Redirected to `/onboarding/wizard/fast-start`
- ✅ Logs show: `TrialFormSuccess`

**Verify in Database:**
```sql
-- Check tenant created
SELECT * FROM "Tenants" WHERE "Name" LIKE 'test-organization%';

-- Check user created
SELECT * FROM "AspNetUsers" WHERE "Email" = 'testuser@example.com';

-- Check user has TenantAdmin role
SELECT u."Email", r."Name" 
FROM "AspNetUsers" u
JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
WHERE u."Email" = 'testuser@example.com';
```

---

### ❌ Scenario 2: Validation Errors (Empty Fields)

**Steps:**
1. Navigate to `/trial`
2. Leave all fields empty
3. Click "Start Free Trial"

**Expected Results:**
- ✅ Form does NOT submit
- ✅ Validation errors shown:
  - Organization Name is required
  - Full Name is required
  - Email is required
  - Password is required
  - Terms must be accepted
- ✅ No tenant or user created

---

### ❌ Scenario 3: Invalid Email Format

**Steps:**
1. Navigate to `/trial`
2. Fill in form with invalid email: `invalid-email`
3. Click "Start Free Trial"

**Expected Results:**
- ✅ Form does NOT submit (client-side validation)
- ✅ Error shown: "Invalid email address"
- ✅ No tenant or user created

---

### ❌ Scenario 4: Weak Password

**Steps:**
1. Navigate to `/trial`
2. Fill in form with password: `short` (less than 12 characters)
3. Click "Start Free Trial"

**Expected Results:**
- ✅ Form does NOT submit (client-side validation)
- ✅ Error shown: "Password must be at least 12 characters"
- ✅ No tenant or user created

---

### ❌ Scenario 5: Terms Not Accepted

**Steps:**
1. Navigate to `/trial`
2. Fill in all fields correctly
3. DO NOT check "Accept Terms"
4. Click "Start Free Trial"

**Expected Results:**
- ✅ Form submits (server-side validation)
- ✅ Error shown: "يجب الموافقة على الشروط والأحكام" (or localized equivalent)
- ✅ No tenant or user created

---

### ❌ Scenario 6: Duplicate Email

**Steps:**
1. Navigate to `/trial`
2. Fill in form with email that already exists in tenant context
3. Click "Start Free Trial"

**Expected Results:**
- ✅ Form submits
- ✅ Error shown: "An account with this email already exists."
- ✅ Tenant created but rolled back
- ✅ No user created
- ✅ Logs show: `TrialFormFailure: Email exists`

**Verify in Database:**
```sql
-- Tenant should NOT exist (rolled back)
SELECT * FROM "Tenants" WHERE "Name" LIKE 'test-organization%';
-- Should return 0 rows
```

---

### ⚠️ Scenario 7: Rate Limiting

**Steps:**
1. Navigate to `/trial`
2. Submit form 6 times rapidly (within 5 minutes)

**Expected Results:**
- ✅ First 5 submissions: Process normally
- ✅ 6th submission: `429 Too Many Requests`
- ✅ Error message: "Too many requests. Please try again later."

---

### ⚠️ Scenario 8: Double Submission Prevention

**Steps:**
1. Navigate to `/trial`
2. Fill in form correctly
3. Click "Start Free Trial" multiple times rapidly

**Expected Results:**
- ✅ Submit button disabled after first click
- ✅ Loading spinner shown: "Processing..."
- ✅ Only ONE tenant created
- ✅ Only ONE user created

---

### ⚠️ Scenario 9: CSRF Token Expired

**Steps:**
1. Navigate to `/trial`
2. Wait 20+ minutes (or adjust CSRF timeout)
3. Fill in form and submit

**Expected Results:**
- ✅ `400 Bad Request` (anti-forgery token validation failed)
- ✅ Generic error page shown
- ✅ User must refresh form and resubmit

---

## Automated Test Commands

### Run All Tests
```bash
cd /home/Shahin-ai/Shahin-Jan-2026
dotnet test tests/GrcMvc.Tests/ --filter "FullyQualifiedName~TrialControllerTests"
```

### Check Logs
```bash
# View application logs (if using Serilog file sink)
tail -f logs/grcmvc-*.log | grep -i "trialform"

# Expected log patterns:
# - TrialFormStart: Organization={Organization}, Email={Email}
# - TrialFormTenantCreated: TenantName={TenantName}, TenantId={TenantId}
# - TrialFormUserCreated: Email={Email}, UserId={UserId}, TenantId={TenantId}
# - TrialFormSuccess: User signed in, redirecting to onboarding
# - TrialFormFailure: Email exists: {Email}
# - TrialFormException: Registration failed for {Email}
```

### Database Verification Queries

```sql
-- List all tenants created in last hour
SELECT "Id", "Name", "OrganizationName", "CreatedDate"
FROM "Tenants"
WHERE "CreatedDate" > NOW() - INTERVAL '1 hour'
ORDER BY "CreatedDate" DESC;

-- List all users created in last hour
SELECT u."Id", u."Email", u."UserName", u."EmailConfirmed", u."CreatedDate"
FROM "AspNetUsers" u
WHERE u."CreatedDate" > NOW() - INTERVAL '1 hour'
ORDER BY u."CreatedDate" DESC;

-- Check tenant-user relationships
SELECT t."Name" AS TenantName, u."Email", tu."RoleCode", tu."Status"
FROM "Tenants" t
JOIN "TenantUsers" tu ON t."Id" = tu."TenantId"
JOIN "AspNetUsers" u ON tu."UserId" = u."Id"
WHERE t."CreatedDate" > NOW() - INTERVAL '1 hour';
```

---

## Test Coverage Summary

| Scenario | Status | Test Type | Priority |
|----------|--------|-----------|----------|
| Happy Path | ✅ | Manual/E2E | High |
| Validation Errors | ✅ | Manual/E2E | High |
| Invalid Email | ✅ | Manual/E2E | High |
| Weak Password | ✅ | Manual/E2E | High |
| Terms Not Accepted | ✅ | Manual/E2E | High |
| Duplicate Email | ✅ | Manual/E2E | Medium |
| Rate Limiting | ⚠️ | Manual/E2E | High |
| Double Submission | ✅ | Manual/E2E | High |
| CSRF Token Expired | ⚠️ | Manual/E2E | Low |

---

## Notes

1. **Full E2E Tests**: Setting up `WebApplicationFactory` with ABP Framework requires:
   - Program class to be accessible (currently top-level statements)
   - Database setup (in-memory or test database)
   - ABP Framework initialization
   - Mocking or test fixtures for dependencies

2. **Current Tests**: `TrialControllerTests.cs` contains documentation-style tests that verify test scenarios are documented.

3. **Manual Testing**: Use this checklist for manual testing until full E2E test suite is implemented.

4. **Future Enhancement**: Consider creating a separate test project for E2E tests with proper WebApplicationFactory setup.

---

## References

- **Scenario Analysis**: `TRIAL_SCENARIOS_ANALYSIS.md`
- **View Scenarios**: `TRIAL_VIEW_SCENARIOS.md`
- **Controller**: `src/GrcMvc/Controllers/TrialController.cs`
- **View**: `src/GrcMvc/Views/Trial/Index.cshtml`
