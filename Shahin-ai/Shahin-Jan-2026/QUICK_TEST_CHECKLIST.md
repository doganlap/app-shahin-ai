# Quick Test Checklist - Ready to Test! ✅

## Current Status

✅ **Branch:** `claude/add-tenant-management-packages-RMC7N`
✅ **ABP Packages:** 21 packages installed
✅ **TrialController:** Fully integrated with ABP
✅ **Trial View:** Exists at `Views/Trial/Index.cshtml`

---

## 🚀 Quick Test Steps

### 1. Restore & Build (2 minutes)

```bash
cd /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc
dotnet restore
dotnet build
```

**Expected:** ✅ Build succeeded

### 2. Check Database Connection

```bash
# Verify PostgreSQL is running
docker ps | grep postgres

# Or check connection string in .env
grep CONNECTION_STRING ../.env
```

**Expected:** ✅ Database connection available

### 3. Run Application

```bash
dotnet run
```

**Expected Output:**
```
Now listening on: http://localhost:5010
Application started.
```

### 4. Test Trial Registration

**Open Browser:**
```
http://localhost:5010/trial
```

**Fill Form:**
- Organization Name: `Test Company`
- Full Name: `John Doe`
- Email: `test@testcompany.com`
- Password: `SecurePass123!`
- Accept Terms: ✓

**Click:** "Start Free Trial"

**Expected Results:**
- ✅ Form submits successfully
- ✅ No errors in console
- ✅ Auto-login occurs
- ✅ Redirects to onboarding wizard

### 5. Verify Database Records

```sql
-- Connect to database
psql -h localhost -U postgres -d GrcMvcDb

-- Check ABP tenant
SELECT "Id", "Name", "IsActive" FROM "AbpTenants" WHERE "Name" = 'test-company';

-- Check ABP user
SELECT "Id", "Email", "TenantId" FROM "AbpUsers" WHERE "Email" = 'test@testcompany.com';

-- Check custom tenant
SELECT "Id", "TenantSlug", "OrganizationName", "IsTrial" FROM "Tenants" WHERE "TenantSlug" = 'test-company';
```

**Expected:** All queries return records

---

## ✅ Test Checklist

### Code Verification
- [x] ABP packages installed (21 packages)
- [x] TrialController exists
- [x] Trial view exists
- [ ] Application builds successfully
- [ ] Application runs without errors

### Functionality Tests
- [ ] Trial registration form loads
- [ ] Form validation works
- [ ] Registration creates ABP tenant
- [ ] Registration creates ABP user
- [ ] Registration creates custom tenant
- [ ] Auto-login works
- [ ] Redirect to onboarding works

### Database Tests
- [ ] AbpTenants table has records
- [ ] AbpUsers table has records
- [ ] Tenants table has records
- [ ] OnboardingWizards table has records
- [ ] TenantUsers table has linkages

---

## 🐛 Common Issues & Fixes

### Issue: Build fails
```bash
# Clear and restore
dotnet clean
dotnet restore
dotnet build
```

### Issue: Database connection fails
```bash
# Check PostgreSQL container
docker ps | grep postgres

# Start if not running
docker-compose up -d db
```

### Issue: Migration needed
```bash
# Create migration
dotnet ef migrations add AddAbpFrameworkTables

# Apply migration
dotnet ef database update
```

### Issue: ITenantAppService not found
- Verify `GrcMvcWebModule.cs` exists
- Check `Program.cs` registers ABP module
- Ensure all ABP packages are restored

---

## 📊 Test Results

**Date:** ___________
**Tester:** ___________

**Build Status:** [ ] ✅ Pass [ ] ❌ Fail
**Registration Test:** [ ] ✅ Pass [ ] ❌ Fail
**Database Records:** [ ] ✅ Pass [ ] ❌ Fail
**Auto-Login:** [ ] ✅ Pass [ ] ❌ Fail
**Onboarding Redirect:** [ ] ✅ Pass [ ] ❌ Fail

**Notes:**
_________________________________________________
_________________________________________________

---

## 🎯 Next Steps After Testing

1. **If all tests pass:**
   - ✅ Ready for production
   - ✅ Document any findings
   - ✅ Create migration if needed

2. **If tests fail:**
   - Check error logs
   - Verify database connection
   - Check ABP module configuration
   - Review troubleshooting guide

---

## 🚀 Ready to Test!

Everything is set up. Just run:

```bash
cd /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc
dotnet restore && dotnet build && dotnet run
```

Then open: **http://localhost:5010/trial**

Good luck! 🎉
