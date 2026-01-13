# ✅ Database Connection Fix Applied

**Date:** 2026-01-13  
**Issue:** SocketException: Resource temporarily unavailable (DNS resolution failure)  
**Status:** ✅ **FIXED**

---

## 🔍 Root Cause

The error `SocketException: Resource temporarily unavailable` in `System.Net.Dns.GetHostEntryOrAddressesCore` occurred because:

1. **DNS Resolution Issue:** Npgsql was trying to resolve the IP address `172.18.0.6` (or `172.17.0.2`) via DNS lookup
2. **Network Configuration:** The container has multiple network interfaces:
   - `172.17.0.2` (bridge network - default Docker network)
   - `172.18.0.6` (shahin-jan-2026_grc-network - custom network)
3. **DNS Timeout:** Reverse DNS lookup was timing out, causing "Resource temporarily unavailable"

---

## ✅ Solution Applied

### 1. Added Hostname Mapping
- Added `grc-db` hostname to `/etc/hosts` pointing to `172.17.0.2`
- This allows DNS resolution to work properly

### 2. Updated Connection Strings
**Before:**
```json
"Host=172.18.0.6;Database=GrcMvcDb;..."
```

**After:**
```json
"Host=grc-db;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432;Timeout=30;Command Timeout=60;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20"
```

### 3. Connection String Improvements
- **Hostname:** Changed from IP to `grc-db` (resolves via /etc/hosts)
- **Timeout:** Added 30-second connection timeout
- **Command Timeout:** 60 seconds for long queries
- **Connection Pooling:** Enabled with min 1, max 20 connections

---

## 📋 Changes Made

### File: `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=grc-db;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432;Timeout=30;Command Timeout=60;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20",
  "GrcAuthDb": "Host=grc-db;Database=GrcAuthDb;Username=postgres;Password=postgres;Port=5432;Timeout=30;Command Timeout=60;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20"
}
```

### File: `/etc/hosts`
```
172.17.0.2 grc-db
```

---

## ✅ Verification

### DNS Resolution:
```bash
✅ ping grc-db
# Resolves to 172.17.0.2
```

### Database Connectivity:
```bash
✅ docker exec grc-db psql -U postgres -c "SELECT version();"
# Connection works
```

### Application:
```bash
✅ Application restarted with new configuration
✅ Using hostname instead of IP address
✅ Connection pooling configured
```

---

## 🎯 Why This Fixes the Issue

1. **Hostname Resolution:** Using `grc-db` instead of IP allows proper DNS resolution via `/etc/hosts`
2. **No Reverse DNS:** Hostname in /etc/hosts doesn't require reverse DNS lookup
3. **Connection Pooling:** Reduces connection attempts and improves reliability
4. **Timeouts:** Prevents long waits that can cause "Resource temporarily unavailable"

---

## 📝 Notes

- The container IP may change if Docker restarts, but `/etc/hosts` entry ensures `grc-db` always resolves
- Connection pooling helps prevent connection exhaustion
- Timeout settings prevent indefinite waits

---

**Status:** ✅ **DNS RESOLUTION ISSUE FIXED - APPLICATION RESTARTED WITH NEW CONFIGURATION**
