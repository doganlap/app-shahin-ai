# 🔧 Fix "Claude Code process exited with code 1"

**Error:** "Claude Code process exited with code 1"

---

## 🔍 Root Cause

The error is **NOT actually a Claude API issue**. The application is failing to start because it **cannot connect to the database**.

**Actual Error from Logs:**
```
Npgsql.NpgsqlException: Failed to connect to 172.18.0.6:5432
Connection refused
```

---

## ✅ Solution

### Problem 1: Database Connection String Uses IP Address

The `appsettings.json` was using a hardcoded IP address (`172.18.0.6`) instead of the Docker service name.

**Fixed:**
- Changed `Host=172.18.0.6` → `Host=grc-db` (Docker service name)
- This allows proper Docker network resolution

### Problem 2: Database Container May Not Be Running

**Check Database Status:**
```bash
docker ps | grep grc-db
```

**Start Database if Stopped:**
```bash
docker start 84e53b2922a6_grc-db
# Or
docker-compose up -d db
```

---

## 🔧 Fixes Applied

1. ✅ **Updated Connection Strings:**
   - `appsettings.json`: Changed IP to Docker service name
   - `DefaultConnection`: `Host=grc-db` (was `Host=172.18.0.6`)
   - `GrcAuthDb`: `Host=grc-db` (was `Host=172.18.0.6`)

2. ✅ **Verified Database:**
   - Database container is running
   - Connection test successful

---

## 🚀 Next Steps

### 1. Restart Application

**If using Docker:**
```bash
docker-compose restart grcmvc
# Or
docker restart shahin-jan-2026_grcmvc_1
```

**If running locally:**
```bash
cd src/GrcMvc
dotnet run
```

### 2. Verify Application Starts

**Check Logs:**
```bash
docker logs shahin-jan-2026_grcmvc_1 --tail 50
```

**Should see:**
- ✅ Database connection successful
- ✅ Application started
- ✅ No connection errors

### 3. Test Claude Features

Once application is running:
- Navigate to AI features
- Test Claude chat/agents
- Should work without errors

---

## 📋 Configuration Summary

**Before (Broken):**
```json
"DefaultConnection": "Host=172.18.0.6;Database=GrcMvcDb;..."
```

**After (Fixed):**
```json
"DefaultConnection": "Host=grc-db;Database=GrcMvcDb;..."
```

**Why This Works:**
- Docker Compose creates a network where services can resolve by name
- `grc-db` is the service name in `docker-compose.yml`
- IP addresses can change, service names are stable

---

## ✅ Verification Checklist

- [ ] Database container running: `docker ps | grep grc-db`
- [ ] Connection string uses `grc-db` (not IP)
- [ ] Application starts without database errors
- [ ] Claude API key configured in `.env`
- [ ] Application logs show successful startup

---

## 🐛 If Still Failing

### Check Database Connection:
```bash
# Test from application container
docker exec shahin-jan-2026_grcmvc_1 ping -c 2 grc-db

# Test database directly
docker exec 84e53b2922a6_grc-db psql -U postgres -c "SELECT 1;"
```

### Check Network:
```bash
# Verify both containers on same network
docker network inspect shahin-jan-2026_grc-network | grep -A 5 grc
```

### Check Environment Variables:
```bash
# Verify connection string in container
docker exec shahin-jan-2026_grcmvc_1 env | grep CONNECTION
```

---

**Status:** ✅ **Fixed** - Connection string updated, ready to restart
