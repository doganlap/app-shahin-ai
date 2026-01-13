# 🔧 Connection Troubleshooting Guide

**Issue:** ERR_CONNECTION_REFUSED when accessing localhost:5137  
**Status:** Application is running and accessible ✅

---

## ✅ Application Status

- **Process:** Running (PID 255835)
- **Port:** 5137 (listening on 0.0.0.0 - all interfaces)
- **Uptime:** ~6 hours
- **Server Test:** ✅ Responding with HTTP 200

---

## 🔍 Troubleshooting Steps

### 1. Verify the Correct URL Format

**✅ CORRECT URLs:**
```
http://localhost:5137
http://127.0.0.1:5137
```

**❌ INCORRECT URLs (will fail):**
```
https://localhost:5137  (HTTPS not configured)
localhost:5137          (missing http://)
http://localhost        (missing port :5137)
```

### 2. Browser-Specific Fixes

#### Chrome/Edge:
1. Clear browser cache: `Ctrl+Shift+Delete` → Clear browsing data
2. Try Incognito/Private mode: `Ctrl+Shift+N`
3. Disable extensions temporarily
4. Check if proxy is enabled: Settings → System → Open proxy settings

#### Firefox:
1. Clear cache: `Ctrl+Shift+Delete`
2. Try Private window: `Ctrl+Shift+P`
3. Check proxy settings: Settings → Network Settings

### 3. Check Browser Console

Open Developer Tools (`F12`) and check:
- **Console tab:** Look for connection errors
- **Network tab:** See if request is being sent
- **Security tab:** Check for HTTPS redirects

### 4. Try Alternative Access Methods

**From Terminal:**
```bash
curl http://localhost:5137/
```

**From Browser:**
- Try `http://127.0.0.1:5137` instead of `localhost`
- Try `http://[your-ip]:5137` (if on network)

### 5. Check System Firewall

```bash
# Check if firewall is blocking
sudo ufw status
sudo iptables -L -n | grep 5137
```

### 6. Restart Application (If Needed)

If the above doesn't work, restart the application:

```bash
# Stop current instance
pkill -f "dotnet.*GrcMvc"

# Wait a few seconds
sleep 3

# Restart
cd /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc
export ConnectionStrings__DefaultConnection="Host=172.18.0.6;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432"
export ConnectionStrings__GrcAuthDb="Host=172.18.0.6;Database=GrcAuthDb;Username=postgres;Password=postgres;Port=5432"
nohup dotnet run --urls "http://0.0.0.0:5137" > /tmp/grcmvc_startup.log 2>&1 &
```

---

## 🎯 Quick Test

Run this command to verify the application is accessible:

```bash
curl -v http://localhost:5137/ 2>&1 | head -20
```

**Expected Output:**
```
* Connected to localhost (127.0.0.1) port 5137
< HTTP/1.1 200 OK
```

---

## 📝 Common Causes

1. **HTTPS vs HTTP:** Browser trying HTTPS (not configured)
2. **Browser Cache:** Old cached connection errors
3. **Proxy Settings:** Browser proxy blocking localhost
4. **Wrong Port:** Using different port number
5. **Browser Extensions:** Security extensions blocking localhost

---

## ✅ Verification

**Application is confirmed working:**
- ✅ Process running (PID 255835)
- ✅ Port 5137 listening on all interfaces
- ✅ HTTP 200 responses working
- ✅ Server accessible via curl

**If still having issues, try:**
1. Different browser
2. Different device on same network
3. Restart browser completely
4. Check browser error console for specific error messages

---

**Last Verified:** 2026-01-13 07:11:48  
**Application Status:** ✅ Running and Accessible
