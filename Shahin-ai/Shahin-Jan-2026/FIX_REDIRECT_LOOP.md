# 🔄 Fix: ERR_TOO_MANY_REDIRECTS

**Date:** 2026-01-22  
**Issue:** Redirect loop causing ERR_TOO_MANY_REDIRECTS

---

## 🚨 Problem

You're seeing redirect loops on:
- app.shahin-ai.com
- portal.shahin-ai.com
- shahin-ai.com

**Root Cause:** Cloudflare proxy (orange cloud) + nginx redirects creating a loop

---

## 🔍 Why This Happens

### Current Setup:
1. **Cloudflare Proxy (ON):** 
   - Receives HTTPS request
   - Forwards to your server (might be HTTP or HTTPS)
   - May also redirect HTTP → HTTPS

2. **Nginx Configuration:**
   - Redirects HTTP → HTTPS (line 110)
   - Listens on HTTPS (port 443)

3. **The Loop:**
   ```
   Browser → Cloudflare (HTTPS) → Server (HTTP) → Nginx redirects to HTTPS → Cloudflare → Loop!
   ```

---

## ✅ Solutions

### Option 1: Turn Off Cloudflare Proxy (Recommended)

**Change all A records from "Proxied" to "DNS only" (gray cloud)**

This eliminates the proxy layer and lets nginx handle everything directly.

**Steps:**
1. Go to Cloudflare DNS settings
2. For each A record (app, login, portal, shahin-ai.com, www):
   - Click "Edit"
   - Toggle proxy OFF (gray cloud)
   - Save
3. Wait 2-5 minutes for propagation

**After this, nginx will handle all redirects directly.**

---

### Option 2: Configure Cloudflare SSL Mode

If you want to keep Cloudflare proxy ON:

1. **Go to Cloudflare Dashboard**
2. **SSL/TLS → Overview**
3. **Set SSL/TLS encryption mode to: "Full" or "Full (strict)"**

**NOT "Flexible"** - Flexible mode causes redirect loops!

**Modes:**
- ❌ **Flexible:** Cloudflare → Server (HTTP) → Causes loop
- ✅ **Full:** Cloudflare → Server (HTTPS) → Works
- ✅ **Full (strict):** Cloudflare → Server (HTTPS, valid cert) → Best

---

### Option 3: Update Nginx for Cloudflare Proxy

If keeping proxy ON, update nginx to detect Cloudflare:

**Add to nginx.conf (in server block):**
```nginx
# Trust Cloudflare IPs
set_real_ip_from 173.245.48.0/20;
set_real_ip_from 103.21.244.0/22;
set_real_ip_from 103.22.200.0/22;
set_real_ip_from 103.31.4.0/22;
set_real_ip_from 141.101.64.0/18;
set_real_ip_from 108.162.192.0/18;
set_real_ip_from 190.93.240.0/20;
set_real_ip_from 188.114.96.0/20;
set_real_ip_from 197.234.240.0/22;
set_real_ip_from 198.41.128.0/17;
set_real_ip_from 162.158.0.0/15;
set_real_ip_from 104.16.0.0/13;
set_real_ip_from 104.24.0.0/14;
set_real_ip_from 172.64.0.0/13;
set_real_ip_from 131.0.72.0/22;
real_ip_header CF-Connecting-IP;

# Only redirect HTTP to HTTPS if not from Cloudflare
if ($http_cf_visitor ~ '"scheme":"http"') {
    return 301 https://$host$request_uri;
}
```

**But this is complex. Option 1 (DNS only) is simpler.**

---

## 🎯 Recommended Fix

### Step 1: Turn Off Cloudflare Proxy

**For all 5 A records:**
- app
- login
- portal
- shahin-ai.com
- www

**Change from "Proxied" (orange) to "DNS only" (gray)**

### Step 2: Wait for DNS Propagation

Wait 2-5 minutes, then verify:
```bash
nslookup shahin-ai.com
# Should return: 46.224.68.73 (not Cloudflare IPs)
```

### Step 3: Test Access

```bash
curl -I https://shahin-ai.com
# Should return: 200 OK (no redirect loop)
```

---

## 🔧 Quick Test

### Test if it's Cloudflare causing the loop:

```bash
# Direct to server (bypass Cloudflare)
curl -I -H "Host: shahin-ai.com" http://46.224.68.73/
# Should work without loop
```

If this works, the issue is definitely Cloudflare proxy.

---

## 📋 Current Nginx Configuration

Your nginx is configured to:
- Redirect HTTP → HTTPS (line 110)
- Listen on HTTPS (port 443)
- Handle SSL certificates

**This is correct for direct connections (DNS only).**

**Problem:** Cloudflare proxy interferes with this.

---

## ✅ Summary

**Issue:** Redirect loop between Cloudflare proxy and nginx

**Fix:** 
1. **Turn off Cloudflare proxy** (change to DNS only) - **RECOMMENDED**
   OR
2. **Set Cloudflare SSL mode to "Full"** (if keeping proxy)

**After fix:**
- Wait 2-5 minutes
- Test access
- Should work without redirect loop

---

**Last Updated:** 2026-01-22
