# 🔒 SSL Certificate Fix - ERR_CERT_AUTHORITY_INVALID

**Error**: `net::ERR_CERT_AUTHORITY_INVALID` on `www.shahin-ai.com`  
**Cause**: Self-signed certificate (not trusted by browsers)  
**Fix Time**: 5 minutes

---

## ⚡ Quick Fix: Cloudflare Origin Certificate

### Step 1: Get Certificate from Cloudflare (2 min)

1. Go to: https://dash.cloudflare.com
2. Select domain: **shahin-ai.com**
3. Navigate: **SSL/TLS** → **Origin Server**
4. Click: **"Create Certificate"**
5. Configure:
   - **Hostnames**: 
     ```
     shahin-ai.com
     *.shahin-ai.com
     ```
   - **Validity**: 15 years
   - **Private key type**: RSA (2048)
6. Click: **"Create"**
7. **Copy both**:
   - Origin Certificate (starts with `-----BEGIN CERTIFICATE-----`)
   - Private Key (starts with `-----BEGIN PRIVATE KEY-----`)

### Step 2: Update Certificate Files (2 min)

```bash
# Backup current certificates
sudo cp /etc/nginx/ssl/fullchain.pem /etc/nginx/ssl/fullchain.pem.backup
sudo cp /etc/nginx/ssl/privkey.pem /etc/nginx/ssl/privkey.pem.backup

# Edit certificate file
sudo nano /etc/nginx/ssl/fullchain.pem
# Delete all, paste Origin Certificate from Cloudflare, save (Ctrl+X, Y, Enter)

# Edit private key file
sudo nano /etc/nginx/ssl/privkey.pem
# Delete all, paste Private Key from Cloudflare, save (Ctrl+X, Y, Enter)

# Set permissions
sudo chmod 600 /etc/nginx/ssl/privkey.pem
sudo chmod 644 /etc/nginx/ssl/fullchain.pem

# Test nginx config
sudo nginx -t

# Reload nginx
sudo systemctl reload nginx
```

### Step 3: Verify (1 min)

```bash
# Test from command line
curl -I https://www.shahin-ai.com
# Should return: HTTP/2 200 (no errors)

# Test in browser
# Visit: https://www.shahin-ai.com
# Should show: ✅ Green padlock (no warnings)
```

---

## 🎯 Alternative: Cloudflare SSL Mode (30 seconds)

**Even faster temporary fix**:

1. **Cloudflare Dashboard** → **SSL/TLS** → **Overview**
2. Change from **"Flexible"** to **"Full"**
3. **Save**

**What this does**:
- Cloudflare accepts your self-signed certificate
- Cloudflare presents its trusted certificate to visitors
- No file changes needed

**⚠️ Note**: This is temporary. Use Origin Certificate for production.

---

## ✅ Verification Checklist

- [ ] Certificate files updated
- [ ] Nginx config test passes (`sudo nginx -t`)
- [ ] Nginx reloaded (`sudo systemctl reload nginx`)
- [ ] Browser shows green padlock ✅
- [ ] No certificate warnings

---

## 📋 Current Certificate Status

**Location**: `/etc/nginx/ssl/`
- `fullchain.pem` - Certificate (currently self-signed)
- `privkey.pem` - Private key

**Nginx Config**: `/etc/nginx/nginx.conf`
- Lines 123-124: Certificate paths configured

---

## 🚀 Next Steps

1. **Get Cloudflare Origin Certificate** (Step 1 above)
2. **Update certificate files** (Step 2 above)
3. **Reload nginx** (Step 2 above)
4. **Test in browser** (Step 3 above)

**Total Time**: ~5 minutes  
**Result**: ✅ Trusted SSL certificate, no browser warnings

---

**Created**: 2026-01-11  
**Status**: Ready to implement
