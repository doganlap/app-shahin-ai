# Safe Database Update Guide - Zero Downtime Deployment

**Issue:** Updating database while old form/application is still deployed  
**Risk:** Schema changes can break running application  
**Solution:** Backward-compatible migrations + deployment strategy

---

## ⚠️ The Problem

**Scenario:**
- Old application is running and using the database
- You want to update the database schema
- New application code is ready but not deployed yet

**Risk:**
- If you add **required** columns → Old app will fail (can't insert/update)
- If you **remove** columns → Old app will fail (queries will break)
- If you **rename** columns → Old app will fail (queries will break)

---

## ✅ Safe Migration Strategy

### Rule 1: Backward-Compatible Migrations Only

**DO:**
- ✅ Add **optional** columns (nullable)
- ✅ Add **new tables** (doesn't affect old code)
- ✅ Add **indexes** (improves performance, doesn't break queries)
- ✅ Add **new columns with defaults** (old code can ignore them)

**DON'T:**
- ❌ Add **required** columns (old code can't insert)
- ❌ **Remove** columns (old code queries will fail)
- ❌ **Rename** columns (old code queries will fail)
- ❌ **Change** column types (old code will fail)

---

## 🔄 Deployment Strategy: Blue-Green Deployment

### Phase 1: Database Update (Backward Compatible)

**Step 1: Apply Safe Migrations**
```bash
# Only apply backward-compatible migrations
dotnet ef database update --context GrcDbContext --no-build
```

**What's Safe:**
- Adding nullable columns
- Adding new tables
- Adding indexes
- Adding columns with default values

**What's NOT Safe:**
- Removing columns
- Renaming columns
- Making columns required (non-nullable)

---

### Phase 2: Deploy New Application

**Step 2: Deploy New Code**
```bash
# Build new application
dotnet build -c Release

# Deploy (without stopping old app)
docker-compose up -d --no-deps --build grcmvc
```

**Strategy:**
- Old app continues running
- New app starts alongside
- Both can use the database (backward compatible)

---

### Phase 3: Switch Traffic

**Step 3: Update Load Balancer/Proxy**
```bash
# Update nginx to point to new app
# Old app can be stopped after traffic switches
```

---

## 📋 Current Migration Status

**Your Application:**
- Migrations run **automatically on startup** (Program.cs line 1340)
- This is **RISKY** for production deployments

**Current Code:**
```csharp
// Program.cs - Line 1340
dbContext.Database.Migrate();  // ⚠️ Runs automatically
authContext.Database.Migrate(); // ⚠️ Runs automatically
```

---

## 🔧 Recommended Fix: Manual Migration Control

### Option 1: Disable Auto-Migration (Recommended)

**Update Program.cs:**
```csharp
// REMOVE or COMMENT OUT automatic migrations
// dbContext.Database.Migrate();  // ❌ Remove this
// authContext.Database.Migrate(); // ❌ Remove this

// Instead, run migrations manually before deployment
```

**Deploy Script:**
```bash
#!/bin/bash
# 1. Backup database
pg_dump -U postgres GrcMvcDb > backup_$(date +%Y%m%d_%H%M%S).sql

# 2. Apply migrations (manually, before deployment)
dotnet ef database update --context GrcDbContext

# 3. Deploy application (migrations already applied)
docker-compose up -d --build grcmvc
```

---

### Option 2: Environment-Based Migration Control

**Update Program.cs:**
```csharp
// Only run migrations in Development
if (builder.Environment.IsDevelopment())
{
    try
    {
        dbContext.Database.Migrate();
        authContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log but don't fail startup
        logger.LogWarning(ex, "Migration failed, continuing...");
    }
}
// Production: Run migrations manually before deployment
```

---

## ✅ Safe Migration Checklist

Before applying any migration:

- [ ] **Migration is backward compatible?**
  - [ ] Only adds nullable columns
  - [ ] Only adds new tables
  - [ ] Only adds indexes
  - [ ] No column removals
  - [ ] No column renames
  - [ ] No required column additions

- [ ] **Database backup created?**
  ```bash
  pg_dump -U postgres GrcMvcDb > backup_$(date +%Y%m%d_%H%M%S).sql
  ```

- [ ] **Migration tested in development?**
  - [ ] Tested with old application code
  - [ ] Tested with new application code
  - [ ] Both work correctly

- [ ] **Rollback plan ready?**
  ```bash
  # If migration fails, restore backup
  psql -U postgres GrcMvcDb < backup_YYYYMMDD_HHMMSS.sql
  ```

---

## 🚨 Breaking Change Migration Strategy

**If you MUST make breaking changes:**

### Step 1: Add New Column (Keep Old)
```sql
-- Migration 1: Add new column (nullable)
ALTER TABLE "Users" ADD COLUMN "NewEmail" VARCHAR(255) NULL;
```

### Step 2: Migrate Data
```sql
-- Migration 2: Copy data from old to new
UPDATE "Users" SET "NewEmail" = "OldEmail";
```

### Step 3: Deploy New Code
- New code uses `NewEmail`
- Old code still uses `OldEmail`
- Both work during transition

### Step 4: Make New Column Required (After All Deployments)
```sql
-- Migration 3: Make required (only after all apps updated)
ALTER TABLE "Users" ALTER COLUMN "NewEmail" SET NOT NULL;
```

### Step 5: Remove Old Column (After All Deployments)
```sql
-- Migration 4: Remove old (only after all apps updated)
ALTER TABLE "Users" DROP COLUMN "OldEmail";
```

**This takes 5 migrations over multiple deployments!**

---

## 📊 Current Deployment Status

**Your Setup:**
- ✅ Application deployed in Docker
- ✅ Database in separate container
- ⚠️ Migrations run automatically on startup

**Recommendation:**
1. **Disable auto-migration** in production
2. **Run migrations manually** before deployment
3. **Test migrations** in development first
4. **Always backup** before migrations

---

## 🔄 Recommended Deployment Workflow

### Development:
```bash
# Auto-migrations OK in dev
dotnet run  # Migrations run automatically
```

### Production:
```bash
# 1. Backup
pg_dump -U postgres GrcMvcDb > backup.sql

# 2. Apply migrations (manually)
dotnet ef database update --context GrcDbContext

# 3. Deploy application
docker-compose up -d --build grcmvc

# 4. Verify
curl http://localhost:8888/health
```

---

## ✅ Quick Fix for Your Current Setup

**To safely update database while old form is deployed:**

1. **Check pending migrations:**
   ```bash
   dotnet ef migrations list --context GrcDbContext
   ```

2. **Review migration SQL:**
   ```bash
   dotnet ef migrations script --context GrcDbContext
   ```

3. **Verify backward compatibility:**
   - Are new columns nullable? ✅
   - Are any columns removed? ❌
   - Are any columns renamed? ❌

4. **If safe, apply:**
   ```bash
   # Backup first!
   docker exec grc-db pg_dump -U postgres GrcMvcDb > backup.sql
   
   # Apply migration
   dotnet ef database update --context GrcDbContext
   ```

5. **Old application will continue working** (if migration is backward compatible)

---

## 🎯 Summary

**Safe Approach:**
- ✅ Disable auto-migrations in production
- ✅ Run migrations manually before deployment
- ✅ Only use backward-compatible migrations
- ✅ Always backup before migrations
- ✅ Test in development first

**Your Current Risk:**
- ⚠️ Auto-migrations run on startup
- ⚠️ Could break if migration is not backward compatible
- ✅ Can be fixed by disabling auto-migration

---

**Next Steps:**
1. Review pending migrations for backward compatibility
2. Disable auto-migration in production code
3. Create deployment script with manual migration step
