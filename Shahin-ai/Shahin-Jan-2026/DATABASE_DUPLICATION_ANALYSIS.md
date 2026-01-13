# 🗄️ Database Count & Duplication Analysis

**Date:** 2026-01-22  
**Status:** ⚠️ **DUPLICATION ISSUE FOUND**

---

## 📊 Database Count

### Actual Databases in PostgreSQL Container

| Database Name | Purpose | Status | Size |
|--------------|---------|--------|------|
| **GrcMvcDb** | Main application database | ✅ Active | 19 MB |
| **postgres** | Default PostgreSQL database | ✅ System | 7.5 MB |
| **template0** | PostgreSQL template | System | - |
| **template1** | PostgreSQL template | System | - |

**Total User Databases:** **1** (GrcMvcDb)  
**Total System Databases:** 3 (postgres, template0, template1)

---

## ⚠️ DUPLICATION ISSUE FOUND

### Problem: GrcAuthDb Connection String Points to Wrong Database

**Current Configuration** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=172.18.0.6;Database=GrcMvcDb;...",
    "GrcAuthDb": "Host=172.18.0.6;Database=GrcMvcDb;..."  // ❌ WRONG!
  }
}
```

**Issue:**
- `GrcAuthDb` connection string points to `GrcMvcDb` database
- Both `GrcDbContext` and `GrcAuthDbContext` use the **same database**
- This defeats the purpose of having separate auth database

**Expected Configuration:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=172.18.0.6;Database=GrcMvcDb;...",
    "GrcAuthDb": "Host=172.18.0.6;Database=GrcAuthDb;..."  // ✅ CORRECT
  }
}
```

---

## 🔍 Architecture Analysis

### Intended Architecture (From Code Comments)

**Two Separate Databases:**
1. **`GrcMvcDb`** - Main application data
   - Entities, workflows, GRC data
   - Uses `GrcDbContext`
   
2. **`GrcAuthDb`** - Authentication & Identity
   - ASP.NET Identity tables (AspNetUsers, AspNetRoles, etc.)
   - Uses `GrcAuthDbContext`
   - **Security isolation** from application data

### Current Reality

**One Database (`GrcMvcDb`):**
- Both contexts point to the same database
- All tables (application + auth) are in one database
- No security isolation

---

## 📋 Code Evidence

### 1. GrcAuthDbContext.cs
```csharp
/// <summary>
/// Separate authentication database context for security isolation.
/// This database contains ONLY authentication/identity data.
/// </summary>
public class GrcAuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    // Purpose: Security isolation
    // Location: Separate database (GrcAuthDb)
}
```

### 2. Program.cs (Lines 142-166)
```csharp
// Get GrcAuthDb connection string
string? authConnectionString = builder.Configuration.GetConnectionString("GrcAuthDb");

// If not set, fallback to default connection (WRONG!)
if (string.IsNullOrWhiteSpace(authConnectionString))
{
    authConnectionString = connectionString; // ❌ Falls back to GrcMvcDb
}

// Register GrcAuthDbContext
builder.Services.AddDbContext<GrcAuthDbContext>(options =>
    options.UseNpgsql(finalAuthConnectionString, ...));
```

**Problem:** Falls back to `DefaultConnection` if `GrcAuthDb` not configured, which points to `GrcMvcDb`.

---

## 🔧 Duplication in Setup

### 1. Connection String Duplication

**Location 1:** `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "...",
  "GrcAuthDb": "..."  // Points to GrcMvcDb (wrong)
}
```

**Location 2:** `docker-compose.yml`
```yaml
environment:
  - ConnectionStrings__DefaultConnection=${CONNECTION_STRING}
  - ConnectionStrings__GrcAuthDb=${CONNECTION_STRING_GrcAuthDb}
```

**Location 3:** `.env` file (if exists)
```bash
CONNECTION_STRING=Host=grc-db;Database=GrcMvcDb;...
CONNECTION_STRING_GrcAuthDb=Host=grc-db;Database=GrcMvcDb;...  # ❌ Wrong
```

### 2. Database Context Registration Duplication

**In Program.cs:**
- `GrcDbContext` registered (line ~406)
- `GrcAuthDbContext` registered (line ~420)
- Both use same connection string (duplication)

### 3. Migration Duplication

**Two Migration Folders:**
- `src/GrcMvc/Migrations/` - GrcDbContext migrations
- `src/GrcMvc/Data/Migrations/Auth/` - GrcAuthDbContext migrations

**But both point to same database!**

---

## ✅ Solution: Fix Database Separation

### Step 1: Create GrcAuthDb Database

```sql
-- Connect to PostgreSQL
docker exec -it grc-db psql -U postgres

-- Create GrcAuthDb database
CREATE DATABASE "GrcAuthDb" 
    WITH OWNER = postgres 
    ENCODING = 'UTF8' 
    LC_COLLATE = 'en_US.utf8' 
    LC_CTYPE = 'en_US.utf8';
```

### Step 2: Update Connection Strings

**Update `appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=grc-db;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432",
    "GrcAuthDb": "Host=grc-db;Database=GrcAuthDb;Username=postgres;Password=postgres;Port=5432"
  }
}
```

**Update `.env` (if exists):**
```bash
CONNECTION_STRING=Host=grc-db;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432
CONNECTION_STRING_GrcAuthDb=Host=grc-db;Database=GrcAuthDb;Username=postgres;Password=postgres;Port=5432
```

### Step 3: Run Auth Migrations

```bash
cd src/GrcMvc
dotnet ef database update --context GrcAuthDbContext
```

### Step 4: Verify Separation

```sql
-- Check GrcMvcDb tables (should NOT have AspNetUsers)
\c GrcMvcDb
\dt | grep -i aspnet  # Should return nothing

-- Check GrcAuthDb tables (should have AspNetUsers)
\c GrcAuthDb
\dt | grep -i aspnet  # Should return AspNetUsers, AspNetRoles, etc.
```

---

## 📊 Summary

| Question | Answer |
|----------|--------|
| **How many databases?** | **1 user database** (GrcMvcDb) - Should be 2 |
| **Duplication in setup?** | **YES** - GrcAuthDb connection points to GrcMvcDb |
| **Intended architecture?** | 2 separate databases (GrcMvcDb + GrcAuthDb) |
| **Current reality?** | 1 database (both contexts use GrcMvcDb) |
| **Security impact?** | ⚠️ No isolation between auth and app data |

---

## 🎯 Recommendations

1. ✅ **Create GrcAuthDb database** (if security isolation is required)
2. ✅ **Update connection strings** to point to correct databases
3. ✅ **Run auth migrations** to populate GrcAuthDb
4. ✅ **Verify separation** by checking table locations
5. ⚠️ **OR** - If security isolation not needed, remove GrcAuthDbContext and use single database

---

**Status:** ⚠️ **Action Required** - Fix database separation or consolidate to single database
