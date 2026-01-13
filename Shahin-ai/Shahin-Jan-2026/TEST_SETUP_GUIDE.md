# Test Setup Guide - ABP Framework Integration

## 🚀 Quick Start Testing

### Step 1: Verify Current Branch

```bash
cd /home/Shahin-ai/Shahin-Jan-2026
git branch --show-current
# Expected: claude/add-tenant-management-packages-RMC7N
```

### Step 2: Pull Latest Changes

```bash
git fetch origin
git pull origin claude/add-tenant-management-packages-RMC7N
```

### Step 3: Restore NuGet Packages

```bash
cd src/GrcMvc
dotnet restore
```

**Expected Output:**
```
✅ Restored packages successfully
```

### Step 4: Verify ABP Packages

```bash
grep "Volo.Abp" GrcMvc.csproj | wc -l
# Expected: 14+ ABP packages
```

### Step 5: Build Project

```bash
dotnet build
```

**Expected Output:**
```
✅ Build succeeded
```

### Step 6: Check Database Connection

```bash
# Verify .env file has connection string
grep "CONNECTION_STRING" ../.env || echo "Check .env file"
```

### Step 7: Create Database Migration (If Needed)

```bash
# Check if migration exists
dotnet ef migrations list

# If ABP tables don't exist, create migration:
dotnet ef migrations add AddAbpFrameworkTables

# Apply migration:
dotnet ef database update
```

### Step 8: Run Application

```bash
dotnet run
```

**Expected Output:**
```
Now listening on: http://localhost:5010
Application started. Press Ctrl+C to shut down.
```

---

## 🧪 Test Scenarios

### Test 1: Trial Registration via /trial

**URL:** http://localhost:5010/trial

**Steps:**
1. Open browser → http://localhost:5010/trial
2. Fill form:
   - Organization Name: `Test Company`
   - Full Name: `John Doe`
   - Email: `john@testcompany.com`
   - Password: `SecurePass123!`
   - Accept Terms: ✓
3. Click "Start Free Trial"
4. **Expected Results:**
   - ✅ Form submits successfully
   - ✅ Auto-login occurs
   - ✅ Redirect to `/OnboardingWizard/Index?tenantId={guid}`
   - ✅ Onboarding wizard displays

**Database Verification:**
```sql
-- Check ABP tenant
SELECT * FROM "AbpTenants" WHERE "Name" = 'test-company';

-- Check ABP user
SELECT * FROM "AbpUsers" WHERE "Email" = 'john@testcompany.com';

-- Check custom tenant
SELECT * FROM "Tenants" WHERE "TenantSlug" = 'test-company';

-- Check onboarding wizard
SELECT * FROM "OnboardingWizards" WHERE "TenantId" = '{tenant-guid}';
```

### Test 2: Trial Registration via /SignupNew

**URL:** http://localhost:5010/SignupNew

**Steps:**
1. Open browser → http://localhost:5010/SignupNew
2. Fill form:
   - Company Name: `Acme Corporation`
   - Full Name: `Jane Smith`
   - Email: `jane@acme.com`
   - Password: `SecurePass123!`
   - Accept Terms: ✓
3. Click "Start Free Trial"
4. **Expected Results:**
   - ✅ Modern UI displays
   - ✅ Form validation works
   - ✅ Auto-login occurs
   - ✅ Redirect to onboarding wizard

### Test 3: Onboarding Enforcement

**Steps:**
1. After registration, try accessing workspace:
   - http://localhost:5010/t/{tenant-slug}/workspace
2. **Expected Results:**
   - ✅ Redirected to onboarding wizard
   - ✅ Cannot access workspace until onboarding complete

### Test 4: Complete Onboarding Flow

**Steps:**
1. Complete all 12 onboarding steps
2. **Expected Results:**
   - ✅ `Tenant.OnboardingStatus` = "COMPLETED"
   - ✅ Workspace access granted
   - ✅ Can access `/t/{slug}/workspace`

---

## 🔍 Verification Checklist

### Code Verification

- [ ] ABP packages installed (14+ packages)
- [ ] `GrcMvcWebModule.cs` exists
- [ ] `GrcDbContext` inherits from `AbpDbContext`
- [ ] `TrialController` uses `ITenantAppService`
- [ ] `OnboardingEnforcementMiddleware` registered
- [ ] `/SignupNew` Razor Page exists

### Database Verification

- [ ] ABP tables created (AbpTenants, AbpUsers, AbpRoles, etc.)
- [ ] Custom Tenants table has records
- [ ] OnboardingWizards table has records
- [ ] TenantUsers table has linkages

### Runtime Verification

- [ ] Application starts without errors
- [ ] `/trial` page loads
- [ ] `/SignupNew` page loads
- [ ] Trial registration works
- [ ] Auto-login works
- [ ] Onboarding redirect works
- [ ] Workspace blocked until onboarding complete

---

## 🐛 Troubleshooting

### Issue: "Package restore failed"

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore
```

### Issue: "Migration not found"

**Solution:**
```bash
# Create migration
dotnet ef migrations add AddAbpFrameworkTables

# Apply migration
dotnet ef database update
```

### Issue: "ITenantAppService not found"

**Solution:**
- Verify `GrcMvcWebModule.cs` has `[DependsOn(typeof(AbpTenantManagementApplicationModule))]`
- Check `Program.cs` registers ABP module

### Issue: "Database connection failed"

**Solution:**
- Check `.env` file has `CONNECTION_STRING`
- Verify PostgreSQL is running: `docker ps | grep postgres`
- Test connection: `psql -h localhost -U postgres -d GrcMvcDb`

### Issue: "Onboarding redirect not working"

**Solution:**
- Verify `OnboardingEnforcementMiddleware` is registered in `Program.cs`
- Check middleware order (should be after `UseAuthentication()`)
- Verify `Tenant.OnboardingStatus` is checked correctly

---

## 📊 Test Results Template

```
Test Date: ___________
Tester: ___________

Test 1: /trial Registration
[ ] Pass
[ ] Fail
Notes: ___________

Test 2: /SignupNew Registration
[ ] Pass
[ ] Fail
Notes: ___________

Test 3: Onboarding Enforcement
[ ] Pass
[ ] Fail
Notes: ___________

Test 4: Complete Onboarding
[ ] Pass
[ ] Fail
Notes: ___________

Overall Status: [ ] Ready for Production [ ] Needs Fixes
```

---

## 🎯 Quick Test Commands

```bash
# 1. Full setup and test
cd /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc
dotnet restore && dotnet build && dotnet run

# 2. Check ABP packages
grep "Volo.Abp" GrcMvc.csproj

# 3. Verify migrations
dotnet ef migrations list

# 4. Check database tables
psql -h localhost -U postgres -d GrcMvcDb -c "\dt" | grep -i abp

# 5. Test endpoints
curl http://localhost:5010/trial
curl http://localhost:5010/SignupNew
```

---

## ✅ Success Criteria

All tests pass when:
- ✅ Trial registration creates ABP tenant + user
- ✅ Auto-login works seamlessly
- ✅ Onboarding wizard redirects correctly
- ✅ Workspace access blocked until onboarding complete
- ✅ No errors in application logs
- ✅ Database records created correctly

Ready to test! 🚀
