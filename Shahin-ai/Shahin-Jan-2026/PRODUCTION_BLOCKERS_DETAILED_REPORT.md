# Production Blockers - Detailed Report

**Generated:** 2026-01-22  
**Status:** ❌ **NOT PRODUCTION READY**  
**Overall Completion:** ~40% (Implementation: 100%, Integration: 0%, Testing: 0%, Deployment: 0%)

---

## 🔴 CRITICAL BLOCKERS (Must Fix Before Production)

### 1. Policy Enforcement System Not Integrated

**Status:** ❌ **BLOCKING**  
**Severity:** CRITICAL (Security Risk)  
**Impact:** Policy engine exists but is NOT being used - security rules are not enforced  
**Estimated Fix Time:** 4-6 hours

**Details:**
- Policy engine infrastructure is built (PolicyEnforcer, PolicyStore, DotPathResolver, etc.)
- **BUT:** No controllers are calling policy enforcement
- All 81 controllers bypass policy checks
- Security rules defined in YAML are never evaluated

**Affected Files:**
- `EvidenceController.cs` - No policy check on create/update/submit/approve/delete
- `AssessmentController.cs` - No policy check on create/update/submit/approve
- `PolicyController.cs` - No policy check on create/update/approve/publish
- `RiskController.cs` - No policy check on create/update/accept
- `ControlController.cs` - No policy check
- `AuditController.cs` - No policy check
- All other domain controllers

**Current Code Pattern (WRONG):**
```csharp
[HttpPost]
[Authorize(GrcPermissions.Evidence.Upload)]
public async Task<IActionResult> Create(CreateEvidenceDto dto)
{
    var evidence = await _evidenceService.CreateAsync(dto); // ❌ No policy check!
    return Ok(evidence);
}
```

**Required Fix:**
```csharp
[HttpPost]
[Authorize(GrcPermissions.Evidence.Upload)]
public async Task<IActionResult> Create(CreateEvidenceDto dto)
{
    // ✅ Add policy enforcement
    await _policyEnforcer.EnforceAsync(new PolicyContext
    {
        Action = "create",
        Environment = _envNameProvider.Get(),
        ResourceType = "Evidence",
        Resource = dto,
        TenantId = CurrentTenant.Id,
        PrincipalId = CurrentUser.Id?.ToString(),
        PrincipalRoles = await _roleResolver.GetCurrentRolesAsync()
    });
    
    var evidence = await _evidenceService.CreateAsync(dto);
    return Ok(evidence);
}
```

---

### 2. IGovernedResource Interface Not Implemented

**Status:** ❌ **BLOCKING**  
**Severity:** CRITICAL (Policy Rules Will Always Fail)  
**Impact:** Policy rules will ALWAYS fail because entities don't have required metadata  
**Estimated Fix Time:** 1 hour

**Details:**
- `IGovernedResource` interface exists but is NOT implemented on entities
- `BaseEntity` does NOT have:
  - `Owner` property
  - `DataClassification` property
  - `Labels` dictionary
- All 80 entity classes are missing governance metadata

**Policy Rules That Will Fail:**
```yaml
# This rule will ALWAYS deny because metadata.labels.dataClassification doesn't exist
- id: REQUIRE_DATA_CLASSIFICATION
  when:
    - op: notMatches
      path: "metadata.labels.dataClassification"
      value: "^(public|internal|confidential|restricted)$"
  effect: deny
```

**Required Fix:**
1. Add `IGovernedResource` implementation to `BaseEntity`
2. Add properties:
   ```csharp
   public string? Owner { get; set; }
   public string? DataClassification { get; set; }
   public Dictionary<string, string> Labels { get; set; } = new();
   ```
3. Create database migration
4. Update all entity classes to populate these fields

**Files to Modify:**
- `Models/Entities/BaseEntity.cs`
- Create migration: `AddGovernanceMetadataToBaseEntity`

---

### 3. No Role Seeding - RBAC Non-Functional

**Status:** ❌ **BLOCKING**  
**Severity:** CRITICAL (Authorization Broken)  
**Impact:** Permissions are defined but no roles exist to grant them - RBAC is non-functional  
**Estimated Fix Time:** 2 hours

**Details:**
- ✅ `GrcPermissions.cs` - 40+ permissions defined
- ✅ `PermissionDefinitionProvider.cs` - Permissions registered
- ❌ **NO** `GrcRoleDataSeedContributor.cs` - No roles seeded
- ❌ **NO** permission grants to roles

**Impact:**
- Users cannot be assigned roles
- Menu items won't show (permission-based)
- Authorization will fail
- System is unusable for end users

**Required Fix:**
1. Create `GrcRoleDataSeedContributor.cs`
2. Define 8 default roles:
   - SuperAdmin (all permissions)
   - TenantAdmin (admin + subscriptions + integrations + users/roles)
   - ComplianceManager (Frameworks/Regulators/Assessments/Evidence/Policies/Calendar/Workflow/Reports)
   - RiskManager (Risks/ActionPlans/Reports)
   - Auditor (Audits + read-only on Evidence/Assessments)
   - EvidenceOfficer (Evidence upload/update + submit)
   - VendorManager (Vendors + Vendor Assessments)
   - Viewer (View-only on everything)
3. Grant permissions to roles
4. Register in `ApplicationInitializer`

**File to Create:**
- `Data/Seed/GrcRoleDataSeedContributor.cs`

---

### 4. Synchronous Blocking Call in AlertService

**Status:** ❌ **BLOCKING** (Performance/Deadlock Risk)  
**Severity:** CRITICAL  
**Impact:** Deadlock risk, thread pool starvation  
**Estimated Fix Time:** 5 minutes

**Location:** `Services/Implementations/AlertService.cs:89`

**Current Code (WRONG):**
```csharp
config ??= GetConfigurationAsync().GetAwaiter().GetResult(); // ❌ BLOCKING
```

**Problem:**
- `.GetAwaiter().GetResult()` blocks the thread
- Can cause deadlocks in ASP.NET Core
- Violates async/await best practices
- Thread pool starvation risk

**Required Fix:**
```csharp
config ??= await GetConfigurationAsync(); // ✅ ASYNC
```

---

## 🟡 HIGH PRIORITY BLOCKERS

### 5. Stub ClickHouse Service Returns Empty Data

**Status:** ⚠️ **HIGH PRIORITY**  
**Impact:** Analytics dashboards will show no data  
**Estimated Fix Time:** 4-6 hours

**Location:** `Services/Analytics/StubImplementations.cs`

**Current State:**
```csharp
public class StubClickHouseService : IClickHouseService
{
    public Task<DashboardSnapshotDto?> GetLatestSnapshotAsync(Guid tenantId) 
        => Task.FromResult<DashboardSnapshotDto?>(null); // Always returns null!
    
    public Task<List<ComplianceTrendDto>> GetComplianceTrendsAsync(...) 
        => Task.FromResult(new List<ComplianceTrendDto>()); // Always empty!
}
```

**Impact:**
- All analytics queries return empty/null
- Dashboards won't display data
- Reports will be empty
- System falls back to PostgreSQL queries (slower performance)

**Required Fix:**
- Either enable ClickHouse in production
- Or implement PostgreSQL-based analytics queries in stub

---

### 6. Missing Unit Tests for Policy Engine

**Status:** ⚠️ **HIGH PRIORITY**  
**Impact:** Policy engine is untested - bugs may exist  
**Estimated Fix Time:** 3-4 hours

**Location:** `tests/GrcMvc.Tests/Unit/`

**Missing Tests:**
- ❌ `DotPathResolverTests.cs` (8 tests needed)
- ❌ `MutationApplierTests.cs` (6 tests needed)
- ❌ `PolicyEnforcerTests.cs` (10 tests needed)
- ⚠️ `PolicyEngineTests.cs` exists but scope unknown

**Impact:**
- No confidence in policy engine correctness
- Bugs may go undetected
- Changes may break functionality

---

### 7. Missing Integration Tests for Policy Enforcement

**Status:** ⚠️ **HIGH PRIORITY**  
**Impact:** No end-to-end validation of policy enforcement  
**Estimated Fix Time:** 2 hours

**Location:** `tests/GrcMvc.Tests/Integration/`

**Missing Tests:**
- ❌ Evidence create denied if `dataClassification` missing
- ❌ Evidence restricted in prod requires `approvedForProd=true`
- ❌ Exception in dev allows restricted without approval
- ❌ Policy mutations applied correctly

---

### 8. Blazor Components Have TODO Placeholders

**Status:** ⚠️ **HIGH PRIORITY**  
**Impact:** UI features are incomplete  
**Estimated Fix Time:** 4-6 hours

**Files with TODOs:**
- `Components/Pages/Workflows/Edit.razor` - Line 132: "TODO: Load from service"
- `Components/Pages/Policies/Index.razor` - Line 83: "TODO: Load from service - for now, demo data"
- `Components/Pages/Audits/Create.razor` - Line 137: "TODO: Call service to create audit"
- `Components/Pages/Controls/Index.razor` - Multiple TODOs for filtering + demo data
- `Components/Pages/Assessments/Index.razor` - Line 82: "TODO: Load from service - for now, demo data"

**Impact:**
- UI shows demo data instead of real data
- Features don't work end-to-end
- User experience is broken

**Example (Controls/Index.razor):**
```csharp
// Demo data - in production, load from IControlService
allControls = new List<ControlListItemDto>
{
    new() { Id = Guid.NewGuid(), ControlNumber = "CTRL-001", ... }, // ❌ Hardcoded
    // ... more demo data
};
```

---

### 9. Workflow Services Have Incomplete Implementations

**Status:** ⚠️ **HIGH PRIORITY**  
**Impact:** Workflow notifications and stakeholder resolution incomplete  
**Estimated Fix Time:** 3-4 hours

**Files:**
- `RiskWorkflowService.cs:110` - "TODO: Get stakeholders from role/permission system"
- `RiskWorkflowService.cs:124` - "TODO: Notify the risk owner"
- `EvidenceWorkflowService.cs:142` - "TODO: Get reviewers from role/permission system"
- `EvidenceWorkflowService.cs:157` - "TODO: Notify the submitter"

**Impact:**
- Workflow notifications may not be sent
- Stakeholders not resolved correctly
- Workflow automation incomplete

---

## 🟢 MEDIUM PRIORITY BLOCKERS

### 10. No Blazor UI Policy Guards

**Status:** ⚠️ **MEDIUM PRIORITY**  
**Impact:** Poor UX - users don't see policy violation reasons  
**Estimated Fix Time:** 2 hours

**Missing:**
- ❌ `PolicyViolationDialog.razor` component
- ❌ Global error handler for policy violations
- ❌ Structured error responses from controllers

**Impact:**
- Users see generic errors
- No remediation hints displayed
- Poor user experience

---

### 11. Missing Redis Cache Package

**Status:** ⚠️ **MEDIUM PRIORITY**  
**Impact:** Caching disabled, performance impact  
**Estimated Fix Time:** 30 minutes

**Location:** `Program.cs:662`

**Code:**
```csharp
// TODO: Add Microsoft.Extensions.Caching.StackExchangeRedis package to enable
```

**Impact:**
- No distributed caching
- Slower performance
- Higher database load

---

### 12. Missing SignalR Redis Backplane

**Status:** ⚠️ **MEDIUM PRIORITY**  
**Impact:** SignalR won't work in multi-instance deployments  
**Estimated Fix Time:** 30 minutes

**Location:** `Program.cs:691`

**Code:**
```csharp
// TODO: Add Microsoft.AspNetCore.SignalR.StackExchangeRedis package to enable
```

**Impact:**
- SignalR only works in single-instance deployments
- Real-time features broken in load-balanced scenarios

---

### 13. Help Contact Form Not Implemented

**Status:** ⚠️ **MEDIUM PRIORITY**  
**Impact:** Contact form doesn't submit  
**Estimated Fix Time:** 1 hour

**Location:** `Views/Help/Contact.cshtml:193`

**Code:**
```javascript
// TODO: Implement actual form submission to server
```

**Impact:**
- Contact form is non-functional
- Users cannot contact support via form

---

### 14. Subscription Management TODOs

**Status:** ⚠️ **MEDIUM PRIORITY**  
**Impact:** Subscription features incomplete  
**Estimated Fix Time:** 2 hours

**Location:** `Views/Subscription/List.cshtml`

**TODOs:**
- Line 147: "TODO: Load available plans and show change plan modal"
- Line 156: "TODO: Call API to cancel subscription"

**Impact:**
- Users cannot change plans
- Users cannot cancel subscriptions

---

### 15. Plan Phase Details Not Loaded

**Status:** ⚠️ **MEDIUM PRIORITY**  
**Impact:** Plan phase details modal is empty  
**Estimated Fix Time:** 1 hour

**Location:** `Views/Plans/Phases.cshtml:198`

**Code:**
```javascript
// TODO: Load phase details and populate modal
```

**Impact:**
- Plan phase details not displayed
- Incomplete functionality

---

## 📊 Summary Statistics

| Priority | Count | Total Estimated Fix Time |
|----------|-------|-------------------------|
| 🔴 Critical | 4 | ~7-9 hours |
| 🟡 High | 5 | ~15-20 hours |
| 🟢 Medium | 6 | ~7-8 hours |
| **TOTAL** | **15** | **~29-37 hours** |

---

## 🎯 Recommended Fix Order

### Phase 1: Critical Blockers (Must Fix First) - 7-9 hours

1. ✅ Add `IGovernedResource` to `BaseEntity` (1 hour)
2. ✅ Integrate policy enforcement into 6 core controllers (4-6 hours)
3. ✅ Create `GrcRoleDataSeedContributor` (2 hours)
4. ✅ Fix synchronous blocking in `AlertService` (5 minutes)

**Phase 1 Total:** ~7-9 hours  
**Status After Phase 1:** ✅ **MINIMUM PRODUCTION READY** (Security & RBAC functional)

### Phase 2: High Priority (Fix Next) - 16-22 hours

5. ✅ Complete Blazor component TODOs (4-6 hours)
6. ✅ Complete workflow service TODOs (3-4 hours)
7. ✅ Add unit tests for policy engine (3-4 hours)
8. ✅ Add integration tests (2 hours)
9. ✅ Fix ClickHouse stub or enable ClickHouse (4-6 hours)

**Phase 2 Total:** ~16-22 hours  
**Status After Phase 2:** ✅ **FEATURE COMPLETE** (All features functional)

### Phase 3: Medium Priority (Polish) - 7 hours

10. ✅ Add Blazor UI policy guards (2 hours)
11. ✅ Add Redis cache package (30 minutes)
12. ✅ Add SignalR Redis backplane (30 minutes)
13. ✅ Implement contact form (1 hour)
14. ✅ Complete subscription management (2 hours)
15. ✅ Load plan phase details (1 hour)

**Phase 3 Total:** ~7 hours  
**Status After Phase 3:** ✅ **PRODUCTION POLISHED** (All features + UX improvements)

---

## 🚨 Production Readiness Assessment

### ❌ NOT Production Ready - Critical Blockers Present

**Blocking Issues:**
1. ❌ Policy enforcement not active (security risk)
2. ❌ Entities missing required metadata (policy rules will fail)
3. ❌ No roles seeded (RBAC non-functional)
4. ❌ Synchronous blocking call (deadlock risk)

**Recommendation:** 
- **DO NOT deploy to production** until Phase 1 critical blockers are fixed
- Estimated time to production-ready: **7-9 hours** (Phase 1 only)
- For full feature completeness: **29-37 hours** (all phases)

---

## 📝 Technical Debt Summary

1. **Policy Engine Built But Not Used** - 35% complete, core infrastructure ready but not integrated
2. **Incomplete UI Components** - Blazor components use demo data
3. **Missing Test Coverage** - Policy engine untested
4. **Stub Implementations** - ClickHouse analytics disabled
5. **TODOs Throughout Codebase** - 6,294 matches found (includes library files, but ~120+ actual TODOs in source code)

---

## 🔧 Quick Wins (Can Fix Immediately)

1. **Fix AlertService blocking call** (5 minutes)
2. **Add Redis cache package** (30 minutes)
3. **Add SignalR Redis backplane** (30 minutes)
4. **Implement contact form** (1 hour)

**Total Quick Wins:** ~2 hours

---

## 📋 Action Items Checklist

### Critical (Do First)
- [ ] Add `IGovernedResource` to `BaseEntity`
- [ ] Integrate policy enforcement into `EvidenceController`
- [ ] Integrate policy enforcement into `AssessmentController`
- [ ] Integrate policy enforcement into `PolicyController`
- [ ] Integrate policy enforcement into `RiskController`
- [ ] Integrate policy enforcement into `ControlController`
- [ ] Integrate policy enforcement into `AuditController`
- [ ] Create `GrcRoleDataSeedContributor`
- [ ] Fix `AlertService` blocking call

### High Priority (Do Next)
- [ ] Complete Blazor component TODOs
- [ ] Complete workflow service TODOs
- [ ] Add policy engine unit tests
- [ ] Add policy enforcement integration tests
- [ ] Fix/enable ClickHouse analytics

### Medium Priority (Polish)
- [ ] Add Blazor UI policy guards
- [ ] Add Redis cache package
- [ ] Add SignalR Redis backplane
- [ ] Implement contact form
- [ ] Complete subscription management
- [ ] Load plan phase details

---

## 📈 Progress Tracking

| Phase | Status | Completion | Estimated Time |
|-------|--------|------------|----------------|
| Phase 1: Critical | ⏳ Pending | 0% | 7-9 hours |
| Phase 2: High Priority | ⏳ Pending | 0% | 16-22 hours |
| Phase 3: Medium Priority | ⏳ Pending | 0% | 7 hours |
| **TOTAL** | **⏳ Pending** | **0%** | **29-37 hours** |

---

**Report Generated:** 2026-01-22  
**Next Review:** After Phase 1 fixes are complete
