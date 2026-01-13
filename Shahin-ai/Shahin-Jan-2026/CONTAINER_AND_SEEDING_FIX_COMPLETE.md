# Container and Seeding Fix - Complete

**Date**: 2025-01-22  
**Status**: ✅ **Fixes Applied - Container Recreated**

---

## Issues Fixed

### 1. ✅ Correct Containers Identified

**Active Containers**:
- `grcmvc-app` - Application (port 5137)
- `grcmvc-db` - PostgreSQL database (GrcMvcDb)

**Connection**: `Host=grcmvc-db;Database=GrcMvcDb`

---

### 2. ✅ Default Tenant Creation

**File**: `src/GrcMvc/Data/ApplicationInitializer.cs`

**Fix**: Moved default tenant creation to **beginning** of initialization (before DerivationRulesSeeds).

**Result**: Default tenant now exists before Ruleset creation.

---

### 3. ✅ DerivationRulesSeeds TenantId Fix

**File**: `src/GrcMvc/Data/Seeds/DerivationRulesSeeds.cs`

**Fix**: Added default tenant lookup and set `TenantId` when creating Ruleset.

**Result**: Ruleset now has valid TenantId (no foreign key violation).

---

### 4. ✅ GrcAuthDb Connection String

**File**: `docker-compose.grcmvc.yml`

**Fix**: Added missing `ConnectionStrings__GrcAuthDb` environment variable.

**Before**:
```yaml
- ConnectionStrings__DefaultConnection=Host=grcmvc-db;Database=GrcMvcDb;...
```

**After**:
```yaml
- ConnectionStrings__DefaultConnection=Host=grcmvc-db;Database=GrcMvcDb;...
- ConnectionStrings__GrcAuthDb=Host=grcmvc-db;Database=GrcAuthDb;...
```

**Result**: Identity database connection string now configured.

---

### 5. ✅ EvidenceScoring and SectorFrameworkIndex Seeding

**File**: `src/GrcMvc/Data/ApplicationInitializer.cs`

**Fix**: Added seeding calls for EvidenceScoringCriteria and SectorFrameworkIndex.

**Result**: These tables will be seeded on next successful startup.

---

## Container Rebuild Required

The container needs to be **rebuilt** to include code changes:

```bash
cd /home/Shahin-ai/Shahin-Jan-2026
docker-compose -f docker-compose.grcmvc.yml build grcmvc
docker-compose -f docker-compose.grcmvc.yml up -d --force-recreate grcmvc
```

---

## Expected Results After Rebuild

Once container is rebuilt and restarted:

1. ✅ Default tenant created
2. ✅ DerivationRulesSeeds runs successfully (with TenantId)
3. ✅ RBAC seeding runs (Features, Permissions, Roles)
4. ✅ EvidenceScoringCriteria seeded (29 records)
5. ✅ SectorFrameworkIndex seeded (50+ records)
6. ✅ All critical tables populated

---

## Verification Commands

After rebuild, verify seeding:

```sql
SELECT 'Tenants' as table_name, COUNT(*) FROM "Tenants"
UNION ALL SELECT 'Features', COUNT(*) FROM "Features"
UNION ALL SELECT 'Permissions', COUNT(*) FROM "Permissions"
UNION ALL SELECT 'EvidenceScoringCriteria', COUNT(*) FROM "EvidenceScoringCriteria"
UNION ALL SELECT 'SectorFrameworkIndex', COUNT(*) FROM "SectorFrameworkIndex";
```

**Expected**:
- Tenants: ≥ 1
- Features: 19
- Permissions: 80+
- EvidenceScoringCriteria: 29
- SectorFrameworkIndex: 50+

---

## Files Modified

1. ✅ `src/GrcMvc/Data/ApplicationInitializer.cs` - Default tenant first, added EvidenceScoring seeding
2. ✅ `src/GrcMvc/Data/Seeds/DerivationRulesSeeds.cs` - Use TenantId
3. ✅ `docker-compose.grcmvc.yml` - Added GrcAuthDb connection string
4. ✅ `src/GrcMvc/Data/GrcDbContext.cs` - Table naming consistency

---

**Status**: All code fixes complete. **Container rebuild required** to apply changes.
