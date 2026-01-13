# Seeding Fix Summary

**Date**: 2025-01-22  
**Issue**: Database seeding failing due to foreign key constraint violation

---

## Problem Identified

### Root Cause
`DerivationRulesSeeds.SeedAsync()` was trying to create a `Ruleset` without a `TenantId`, but the `Rulesets` table has a foreign key constraint requiring a valid `TenantId`.

**Error**:
```
23503: insert or update on table "Rulesets" violates foreign key constraint "FK_Rulesets_Tenants_TenantId"
```

---

## Fixes Applied

### 1. Create Default Tenant First ✅

**File**: `src/GrcMvc/Data/ApplicationInitializer.cs`

**Change**: Moved default tenant creation to the **beginning** of initialization, before `DerivationRulesSeeds` runs.

```csharp
// CRITICAL: Create default tenant FIRST (required for Rulesets and RBAC)
var defaultTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.TenantSlug == "default" && !t.IsDeleted);
if (defaultTenant == null)
{
    // Create default tenant...
}
```

### 2. Fix DerivationRulesSeeds to Use TenantId ✅

**File**: `src/GrcMvc/Data/Seeds/DerivationRulesSeeds.cs`

**Change**: Added default tenant lookup and set `TenantId` when creating Ruleset.

```csharp
// Get default tenant (required for Ruleset foreign key)
var defaultTenant = await context.Tenants.FirstOrDefaultAsync(t => t.TenantSlug == "default" && !t.IsDeleted);
if (defaultTenant == null)
{
    throw new InvalidOperationException("Default tenant must exist before seeding derivation rules.");
}

ruleset = new Ruleset
{
    TenantId = defaultTenant.Id, // Required foreign key
    // ... other properties
};
```

---

## Container Status

**Correct Containers**:
- ✅ `grcmvc-app` - Application container (port 5137)
- ✅ `grcmvc-db` - PostgreSQL database (GrcMvcDb)

**Connection String**: `Host=grcmvc-db;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432`

---

## Next Steps

1. **Rebuild Docker Image** (if code changes not reflected):
   ```bash
   docker-compose -f docker-compose.grcmvc.yml build grcmvc
   ```

2. **Restart Container**:
   ```bash
   docker-compose -f docker-compose.grcmvc.yml up -d --force-recreate grcmvc
   ```

3. **Verify Seeding**:
   ```sql
   SELECT 'Tenants' as table_name, COUNT(*) FROM "Tenants"
   UNION ALL SELECT 'Features', COUNT(*) FROM "Features"
   UNION ALL SELECT 'Permissions', COUNT(*) FROM "Permissions"
   UNION ALL SELECT 'EvidenceScoringCriteria', COUNT(*) FROM "EvidenceScoringCriteria"
   UNION ALL SELECT 'SectorFrameworkIndex', COUNT(*) FROM "SectorFrameworkIndex";
   ```

**Expected Results**:
- Tenants: ≥ 1
- Features: 19
- Permissions: 80+
- EvidenceScoringCriteria: 29
- SectorFrameworkIndex: 50+

---

## Files Modified

1. ✅ `src/GrcMvc/Data/ApplicationInitializer.cs` - Create default tenant first
2. ✅ `src/GrcMvc/Data/Seeds/DerivationRulesSeeds.cs` - Use TenantId when creating Ruleset
3. ✅ `src/GrcMvc/Data/ApplicationInitializer.cs` - Added EvidenceScoring and SectorFrameworkIndex seeding

---

**Status**: Code fixes complete. Container needs rebuild/restart to apply changes.
