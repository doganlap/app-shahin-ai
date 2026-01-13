# Public Domain Deployment Guide

**Domain:** shahin-ai.com  
**Server IP:** 46.224.68.73  
**Date:** 2026-01-22

---

## 🎯 Deployment Steps

### 1. ✅ Update Nginx Configuration

**File:** `nginx/nginx.conf`

**Changes Made:**
- Updated upstream backend from port `5137` to `8888` (correct production port)
- Configuration already includes:
  - HTTP → HTTPS redirect
  - SSL/TLS configuration
  - Security headers
  - Rate limiting
  - WebSocket support (SignalR)

**Status:** ✅ **UPDATED**

---

### 2. 📋 DNS Records Required

Your DNS records should point to: **46.224.68.73**

#### A Records (5 records):
```
shahin-ai.com          → 46.224.68.73
www.shahin-ai.com      → 46.224.68.73
app.shahin-ai.com      → 46.224.68.73
portal.shahin-ai.com   → 46.224.68.73
login.shahin-ai.com    → 46.224.68.73
```

#### CNAME Records (2 records - DKIM):
```
selector1._domainkey   → selector1-shahin-ai-com._domainkey.outlook.com
selector2._domainkey   → selector2-shahin-ai-com._domainkey.outlook.com
```

#### MX Record (1 record):
```
shahin-ai.com          → shahin-ai-com.mail.protection.outlook.com (Priority: 0)
```

#### TXT Records (2 records):
```
shahin-ai.com          → v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
_dmarc                 → v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; pct=100
```

**Action Required:** Verify all DNS records are configured in Cloudflare/DNS provider.

---

### 3. 🔒 SSL Certificates

#### Option A: Use Existing Certificates
If you have SSL certificates:
```bash
# Copy certificates to nginx
sudo cp /path/to/fullchain.pem /etc/nginx/ssl/fullchain.pem
sudo cp /path/to/privkey.pem /etc/nginx/ssl/privkey.pem
sudo chmod 600 /etc/nginx/ssl/privkey.pem
```

#### Option B: Let's Encrypt (Recommended)
```bash
# Install certbot
sudo apt-get update
sudo apt-get install certbot python3-certbot-nginx -y

# Get certificates
sudo certbot --nginx -d shahin-ai.com -d www.shahin-ai.com -d app.shahin-ai.com -d portal.shahin-ai.com -d login.shahin-ai.com

# Auto-renewal
sudo certbot renew --dry-run
```

**Current Status:** Check if certificates exist in `nginx/ssl/` directory.

---

### 4. 🚀 Install and Configure Nginx

#### Install Nginx:
```bash
sudo apt-get update
sudo apt-get install nginx -y
```

#### Copy Configuration:
```bash
# Copy nginx config
sudo cp nginx/nginx.conf /etc/nginx/nginx.conf

# Test configuration
sudo nginx -t

# If test passes, reload nginx
sudo systemctl reload nginx
```

#### Start Nginx:
```bash
sudo systemctl enable nginx
sudo systemctl start nginx
sudo systemctl status nginx
```

---

### 5. 🔥 Firewall Configuration

#### Open Required Ports:
```bash
# HTTP (80) - for Let's Encrypt and redirects
sudo ufw allow 80/tcp

# HTTPS (443) - for production traffic
sudo ufw allow 443/tcp

# Application port (8888) - should only be accessible from localhost
# Already restricted by Docker, but verify:
sudo ufw status
```

---

### 6. ⚙️ Application Configuration

**File:** `.env`

**Updated:**
- `ALLOWED_HOSTS` now includes all public domains
- Application already configured for production

**File:** `appsettings.json`

**Current URLs:**
- BaseUrl: `https://app.shahin-ai.com`
- LandingUrl: `https://shahin-ai.com`

**Status:** ✅ **CONFIGURED**

---

### 7. 🐳 Docker Services

**Current Status:**
- ✅ Application running on port 8888
- ✅ Database running
- ✅ Redis running

**Verify:**
```bash
docker-compose -f docker-compose.yml ps
```

---

## 📊 Deployment Checklist

- [x] Update nginx.conf (port 8888)
- [x] Update .env (ALLOWED_HOSTS)
- [ ] Install nginx on server
- [ ] Copy nginx configuration
- [ ] Set up SSL certificates
- [ ] Configure DNS records
- [ ] Open firewall ports (80, 443)
- [ ] Start nginx service
- [ ] Test public domain access

---

## 🧪 Testing

### 1. Test DNS Resolution:
```bash
# Check if DNS is resolving
nslookup shahin-ai.com
dig shahin-ai.com

# Should return: 46.224.68.73
```

### 2. Test HTTP Redirect:
```bash
curl -I http://shahin-ai.com
# Should return: 301 redirect to HTTPS
```

### 3. Test HTTPS:
```bash
curl -I https://shahin-ai.com
# Should return: 200 OK
```

### 4. Test Application:
```bash
curl https://shahin-ai.com/
# Should return: HTML content
```

---

## 🌐 Public URLs

Once deployed, your application will be accessible at:

- **Main Site:** https://shahin-ai.com
- **Application:** https://app.shahin-ai.com
- **Portal:** https://portal.shahin-ai.com
- **Login:** https://login.shahin-ai.com
- **Trial:** https://shahin-ai.com/trial

---

## ⚠️ Important Notes

1. **DNS Propagation:** DNS changes can take 24-48 hours to propagate globally
2. **SSL Certificates:** Ensure certificates are valid and not expired
3. **Firewall:** Only ports 80 and 443 should be publicly accessible
4. **Application Port:** Port 8888 should only be accessible from localhost (via nginx)
5. **Monitoring:** Set up monitoring for nginx and application health

---

## 🔧 Troubleshooting

### Nginx Not Starting:
```bash
# Check configuration
sudo nginx -t

# Check logs
sudo tail -f /var/log/nginx/error.log
```

### SSL Certificate Issues:
```bash
# Check certificate validity
openssl x509 -in /etc/nginx/ssl/fullchain.pem -text -noout

# Test SSL connection
openssl s_client -connect shahin-ai.com:443
```

### Application Not Accessible:
```bash
# Check if application is running
docker-compose -f docker-compose.yml ps

# Check application logs
docker logs shahin-jan-2026_grcmvc_1 --tail 50

# Test local access
curl http://localhost:8888/
```

---

## 📝 Next Steps

1. **Install Nginx** on the server
2. **Copy configuration** from `nginx/nginx.conf`
3. **Set up SSL certificates** (Let's Encrypt recommended)
4. **Verify DNS records** are pointing to 46.224.68.73
5. **Start nginx service**
6. **Test public access**

---

**Status:** 🔄 **READY FOR DEPLOYMENT**

**Server IP:** 46.224.68.73  
**Application Port:** 8888 (internal, proxied by nginx)  
**Public Ports:** 80 (HTTP), 443 (HTTPS)

---

**Last Updated:** 2026-01-22
