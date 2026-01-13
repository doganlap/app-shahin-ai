# 🔄 Final Fix for ERR_TOO_MANY_REDIRECTS

**Date:** 2026-01-22  
**Issue:** Redirect loop between Cloudflare and nginx

---

## 🚨 Root Cause

**The redirect loop is caused by Cloudflare SSL mode being "Flexible"**

When Cloudflare proxy is ON with "Flexible" SSL:
- Browser → Cloudflare (HTTPS)
- Cloudflare → Your Server (HTTP) ← **This is the problem!**
- Nginx redirects HTTP → HTTPS
- **LOOP!**

---

## ✅ Solution: Change Cloudflare SSL Mode

### Step 1: Go to Cloudflare Dashboard
1. Log in: https://dash.cloudflare.com
2. Select domain: **shahin-ai.com**
3. Click: **SSL/TLS** (left sidebar)
4. Click: **Overview** tab

### Step 2: Change SSL/TLS Encryption Mode

**Current:** Likely "Flexible" ❌  
**Change to:** **"Full"** ✅

**Click the dropdown and select "Full"**

### Step 3: Save
- The change takes effect immediately
- Wait 1-2 minutes for propagation

---

## 🎯 Why "Full" Mode Fixes It

**Flexible Mode (Current - Causes Loop):**
```
Browser (HTTPS) → Cloudflare (HTTPS) → Server (HTTP) → Nginx redirects → LOOP!
```

**Full Mode (Fixed):**
```
Browser (HTTPS) → Cloudflare (HTTPS) → Server (HTTPS) → Works! ✅
```

---

## 🧪 Test After Fix

```bash
# Should work without redirect loop
curl -I https://shahin-ai.com
# Should return: 200 OK

# Test in browser
# Visit: https://shahin-ai.com
# Should load without ERR_TOO_MANY_REDIRECTS
```

---

## 📋 Alternative: Turn Off Cloudflare Proxy

**If you prefer direct connection:**

1. Go to **DNS** → **Records**
2. For all 5 A records (app, login, portal, shahin-ai.com, www):
   - Click "Edit"
   - Toggle proxy OFF (gray cloud)
   - Save
3. Wait 2-5 minutes
4. Test

**This eliminates Cloudflare proxy entirely.**

---

## ✅ Summary

**The Fix:**
1. **Cloudflare Dashboard** → SSL/TLS → Overview
2. **Change SSL mode** from "Flexible" to **"Full"**
3. **Save and wait** 1-2 minutes
4. **Test access**

**OR**

1. **Turn off Cloudflare proxy** (DNS only)
2. **Wait** 2-5 minutes
3. **Test access**

**After either fix, the redirect loop will be resolved.**

---

**Last Updated:** 2026-01-22
