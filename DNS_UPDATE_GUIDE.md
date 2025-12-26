# 🌐 DNS Configuration Update Required

## Problem
The error `DEPLOYMENT_NOT_FOUND` indicates your domains are pointing to the wrong IP address (old Cloudflare/Railway deployment).

## Current Status
✅ **Local Applications**: Running perfectly on server  
❌ **Public Domains**: DNS pointing to wrong IP

## Your Server Details

**IPv4 Address:** `37.27.139.173`  
**IPv6 Address:** `2a01:4f9:3100:1391::2`

## DNS Configuration Steps

### 1. Login to Cloudflare (or your DNS provider)
- Go to https://dash.cloudflare.com
- Select your domain: `shahin-ai.com`

### 2. Update DNS Records

Navigate to **DNS > Records** and update these:

#### A Records (IPv4)
```
Type: A
Name: grc
Content: 37.27.139.173
Proxy: OFF (DNS only) ⚠️ Important!
TTL: Auto
```

```
Type: A
Name: api-grc
Content: 37.27.139.173
Proxy: OFF (DNS only) ⚠️ Important!
TTL: Auto
```

#### AAAA Records (IPv6) - Optional but recommended
```
Type: AAAA
Name: grc
Content: 2a01:4f9:3100:1391::2
Proxy: OFF (DNS only)
TTL: Auto
```

```
Type: AAAA
Name: api-grc
Content: 2a01:4f9:3100:1391::2
Proxy: OFF (DNS only)
TTL: Auto
```

### 3. SSL/TLS Configuration

In Cloudflare SSL/TLS settings:
- **SSL/TLS encryption mode**: Full (not Full Strict yet)
- After SSL certificates are configured on server, change to "Full (strict)"

### 4. Wait for DNS Propagation
- Typically: 5-60 minutes
- Can take up to 24 hours in rare cases

### 5. Verify DNS Update

```bash
# Check if DNS has updated
dig grc.shahin-ai.com
dig api-grc.shahin-ai.com

# Should show: 37.27.139.173
```

## Testing After Update

### Once DNS propagates:

```bash
# Test Web Application
curl https://grc.shahin-ai.com

# Test API
curl https://api-grc.shahin-ai.com/api

# Test in browser
# https://grc.shahin-ai.com
# https://api-grc.shahin-ai.com/swagger
```

## Temporary Access (Until DNS Updates)

Use direct IP access:

**Web Application:**
```
http://37.27.139.173:5001
```

**API Application:**
```
http://37.27.139.173:5000
```

## SSL Certificate Setup (After DNS Update)

Once DNS is pointing correctly, configure SSL:

```bash
# Install certbot
sudo apt install certbot python3-certbot-nginx

# Get certificates
sudo certbot certonly --standalone -d grc.shahin-ai.com -d api-grc.shahin-ai.com

# Certificates will be at:
# /etc/letsencrypt/live/grc.shahin-ai.com/
```

## Current Application Status

✅ **Web Service**: Active (HTTP 200)  
✅ **API Service**: Active (HTTP 302)  
✅ **Database**: 42 tables created  
✅ **All Modules**: Evidence, Framework Library, Risks - Working  

**The applications are fully functional - only DNS routing needs update!**

## Troubleshooting

### If domains still don't work after 1 hour:

1. **Clear DNS Cache**
   ```bash
   # Windows
   ipconfig /flushdns
   
   # Mac
   sudo dscacheutil -flushcache
   
   # Linux
   sudo systemd-resolve --flush-caches
   ```

2. **Check DNS propagation globally**
   - Visit: https://dnschecker.org
   - Enter: grc.shahin-ai.com
   - Should show: 37.27.139.173

3. **Verify Cloudflare proxy is OFF**
   - Orange cloud = Proxied (wrong)
   - Grey cloud = DNS only (correct) ✅

---

**Need Help?**
- Cloudflare Docs: https://developers.cloudflare.com/dns/
- Contact: Check your domain registrar support

**Last Updated:** December 21, 2024
