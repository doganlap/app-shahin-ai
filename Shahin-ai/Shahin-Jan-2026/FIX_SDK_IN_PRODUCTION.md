# Fix: SDK Missing in Production Container

**Date:** 2026-01-22  
**Issue:** Production container doesn't have .NET SDK

---

## ✅ This is Normal (But Can Be Fixed If Needed)

**Your Dockerfile is correct** - production containers typically don't have SDK to keep them small and secure.

**However, if you need SDK for:**
- Running migrations (`dotnet ef database update`)
- Runtime compilation
- Other SDK commands

**Here are your options:**

---

## 🔧 Solution 1: Run Migrations Outside Container (Recommended)

**Run migrations from your local machine or CI/CD:**

```bash
# On your local machine (or CI/CD server)
cd /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc

# Connect to production database
export ConnectionStrings__DefaultConnection="Host=46.224.68.73;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432"

# Run migrations
dotnet ef database update
```

**Or use docker-compose exec to run in database container:**
```bash
# If you have a migration container
docker-compose run --rm migrations dotnet ef database update
```

---

## 🔧 Solution 2: Add SDK to Production Container (If Really Needed)

**If you absolutely need SDK in production, modify Dockerfile:**

```dockerfile
# Change final stage from runtime to SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef

# Rest of configuration...
```

**⚠️ Warning:** This makes the image ~1.3 GB larger!

---

## 🔧 Solution 3: Create Separate Migration Container

**Create a dedicated container just for migrations:**

**File:** `docker-compose.migrations.yml`
```yaml
services:
  migrations:
    image: mcr.microsoft.com/dotnet/sdk:8.0
    volumes:
      - ./src/GrcMvc:/app
      - ./src/GrcMvc/Migrations:/app/Migrations
    working_dir: /app
    environment:
      - ConnectionStrings__DefaultConnection=${CONNECTION_STRING}
    command: dotnet ef database update
```

**Run migrations:**
```bash
docker-compose -f docker-compose.migrations.yml run --rm migrations
```

---

## 🔧 Solution 4: Use EF Core Migrate() API (Already Implemented)

**Your application already has this!** Check `Program.cs:1394`

```csharp
dbContext.Database.Migrate(); // This doesn't require SDK!
```

**To enable auto-migration:**
```bash
# Set environment variable
export ENABLE_AUTO_MIGRATION=true

# Or in .env file
echo "ENABLE_AUTO_MIGRATION=true" >> .env

# Restart container
docker-compose restart grcmvc
```

**⚠️ Note:** Auto-migration is disabled in production by default (best practice).

---

## 📊 Current Container Status

**Runtime:** ✅ Present (.NET 8.0.22)  
**SDK:** ❌ Not present (by design)  
**Application:** ✅ Should run fine without SDK

**The application uses `Database.Migrate()` which doesn't require SDK!**

---

## 🧪 Verify What You Need SDK For

**If you're seeing an error about SDK, check:**

1. **Are you trying to run `dotnet ef`?**
   - Solution: Run migrations outside container

2. **Is the application failing to start?**
   - Check logs: `docker logs <container>`
   - Application should run fine with just runtime

3. **Do you need to compile something at runtime?**
   - Solution: Add SDK to container (Solution 2)

---

## ✅ Recommended Approach

**For Production:**
1. ✅ Keep production container without SDK (current setup)
2. ✅ Run migrations via CI/CD or separate migration container
3. ✅ Use `Database.Migrate()` API if needed (already in code)

**For Development:**
- Use SDK container for development/testing

---

## 🔍 Check Current Setup

```bash
# Check if container is running
docker-compose -f docker-compose.yml ps grcmvc

# Check runtime version
docker exec <container> dotnet --info

# Check application status
docker logs <container> --tail 50
```

---

## ✅ Summary

**Current Setup:** ✅ **CORRECT** (runtime only, no SDK)  
**If You Need SDK:** Choose one of the solutions above  
**For Migrations:** Use `Database.Migrate()` API or run outside container

**The application should work fine without SDK. If you're seeing a specific error, let me know what it is!**

---

**Last Updated:** 2026-01-22
