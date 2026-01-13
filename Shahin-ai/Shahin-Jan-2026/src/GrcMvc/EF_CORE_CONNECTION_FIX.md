# EF Core Connection Issue - Root Cause & Solution

## 🔴 Problem Identified

**Error:** `System.Net.Sockets.SocketException: Resource temporarily unavailable`
```
at System.Net.Dns.GetHostAddresses(String hostNameOrAddress)
at Npgsql.Internal.NpgsqlConnector.Connect(...)
```

**Root Cause:**
- Connection string uses: `Host=grc-db`
- `grc-db` is a **Docker container name**, only resolvable **inside Docker network**
- When running `dotnet ef database update` from **host machine**, it cannot resolve `grc-db`
- Container is on network: `shahin-jan-2026_grc-network`
- Container IP: `172.18.0.6` (internal Docker network)

## ✅ Solutions

### Solution 1: Use Environment Variable (Recommended)
Override connection string when running migrations:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432"
dotnet ef database update --context GrcMvcDbContext
```

### Solution 2: Use Container IP
```bash
export ConnectionStrings__DefaultConnection="Host=172.18.0.6;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432"
dotnet ef database update --context GrcMvcDbContext
```

### Solution 3: Run EF Core from Inside Docker Container
```bash
docker exec -it grc-mvc-container dotnet ef database update --context GrcMvcDbContext
```

### Solution 4: Expose Database Port to Host
Modify `docker-compose.yml` to expose port:
```yaml
services:
  grc-db:
    ports:
      - "5432:5432"  # Expose to host
```

Then use: `Host=localhost;Port=5432`

## 📋 Current Status

- ✅ **Migration Applied:** Directly via SQL (bypassed EF Core connection issue)
- ✅ **Indexes Created:** 8 performance indexes
- ✅ **Migration Recorded:** In `__EFMigrationsHistory`
- ⚠️ **EF Core Tool:** Cannot connect from host (needs connection string override)

## 🔧 Quick Fix for Future Migrations

Create a script: `scripts/apply-migration.sh`
```bash
#!/bin/bash
# Override connection string for EF Core migrations
export ConnectionStrings__DefaultConnection="Host=localhost;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432"
dotnet ef database update --context GrcMvcDbContext --project src/GrcMvc
```

Or use Docker exec:
```bash
docker exec -it $(docker ps -q -f name=grc-mvc) dotnet ef database update --context GrcMvcDbContext
```
