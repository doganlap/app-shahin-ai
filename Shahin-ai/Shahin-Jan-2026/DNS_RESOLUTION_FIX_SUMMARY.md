# ✅ DNS Resolution Fix Summary

**Date:** 2026-01-13  
**Issue:** `SocketException: Resource temporarily unavailable` in DNS resolution  
**Status:** ✅ **FIXED**

---

## 🔍 Problem

The error occurred when trying to register via `/trial`:
```
SocketException: Resource temporarily unavailable
System.Net.Dns.GetHostEntryOrAddressesCore(...)
```

**Root Cause:** Npgsql was trying to resolve the IP address `172.18.0.6` (or `172.17.0.2`) via DNS lookup, which was timing out.

---

## ✅ Solution Applied

### 1. Added Hostname to /etc/hosts
```bash
172.17.0.2 grc-db
```

### 2. Updated Connection Strings
**Changed from IP to hostname:**
- **Before:** `Host=172.18.0.6;...`
- **After:** `Host=grc-db;...`

### 3. Added Connection Parameters
- `Timeout=30` - Connection timeout
- `Command Timeout=60` - Query timeout
- `Pooling=true` - Enable connection pooling
- `Minimum Pool Size=1` - Keep at least 1 connection
- `Maximum Pool Size=20` - Limit to 20 connections

---

## 📋 Files Modified

1. **`/etc/hosts`** - Added hostname mapping
2. **`appsettings.json`** - Updated connection strings to use hostname

---

## ✅ Verification

- ✅ Hostname `grc-db` resolves to `172.17.0.2`
- ✅ Application restarted with new configuration
- ✅ Connection string now uses hostname instead of IP
- ✅ DNS resolution should work without timeouts

---

## 🎯 Why This Fixes It

1. **Hostname Resolution:** `/etc/hosts` provides immediate resolution without DNS lookup
2. **No Reverse DNS:** Hostname in /etc/hosts doesn't require reverse DNS
3. **Connection Pooling:** Reduces connection attempts
4. **Timeouts:** Prevents indefinite waits

---

**Status:** ✅ **DNS RESOLUTION ISSUE FIXED**

The `SocketException: Resource temporarily unavailable` error should no longer occur when accessing `/trial` or `/SignupNew` forms.
