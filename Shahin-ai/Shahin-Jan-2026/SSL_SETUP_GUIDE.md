# 🔐 SSL Certificate Setup Guide

**Date:** 2026-01-11
**Status:** Ready to configure

---

## Overview

This guide will help you set up automatic SSL certificates for all your shahin-ai.com domains using Caddy with Let's Encrypt.

**What will be configured:**
- ✅ https://portal.shahin-ai.com
- ✅ https://app.shahin-ai.com
- ✅ https://shahin-ai.com
- ✅ https://www.shahin-ai.com
- ✅ https://login.shahin-ai.com

---

## Prerequisites

### 1. DNS Configuration (REQUIRED)

**Before running the setup, ensure ALL domains point to your server IP:**

```bash
# Check DNS resolution for each domain
dig +short portal.shahin-ai.com
dig +short app.shahin-ai.com
dig +short shahin-ai.com
dig +short www.shahin-ai.com
dig +short login.shahin-ai.com
```

All should return your server's public IP address.

**If DNS is not configured:**
1. Log into your DNS provider (Cloudflare, Route53, etc.)
2. Create A records:
   - `portal.shahin-ai.com` → Your Server IP
   - `app.shahin-ai.com` → Your Server IP
   - `shahin-ai.com` → Your Server IP
   - `www.shahin-ai.com` → Your Server IP
   - `login.shahin-ai.com` → Your Server IP
3. Wait 5-10 minutes for DNS propagation

### 2. Firewall Configuration (REQUIRED)

**Ensure ports 80 and 443 are open:**

```bash
# Check if ports are open
netstat -tlnp | grep -E ':80|:443'

# If using ufw
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# If using iptables
sudo iptables -A INPUT -p tcp --dport 80 -j ACCEPT
sudo iptables -A INPUT -p tcp --dport 443 -j ACCEPT

# If using cloud provider firewall (AWS, Azure, etc.)
# Add rules in your cloud console for ports 80 and 443
```

### 3. Caddy Installation

✅ Already installed (Caddy 2.10.2)

---

## Quick Setup (Automated)

### Run the automated setup script:

```bash
cd /home/Shahin-ai/Shahin-Jan-2026
sudo ./scripts/setup-ssl-caddy.sh
```

This script will:
1. Create Caddyfile with SSL configuration
2. Validate configuration
3. Start Caddy service
4. Request Let's Encrypt certificates
5. Test HTTPS endpoints

**Expected output:**
```
🔐 SSL Certificate Setup with Caddy
====================================

✅ Caddyfile created
✅ Log directory created
✅ Caddyfile is valid
✅ Caddy is running
✅ SSL certificates are being provisioned
✅ https://portal.shahin-ai.com is accessible
✅ https://app.shahin-ai.com is accessible

========================================
✅ SSL SETUP COMPLETE
========================================
```

---

## Manual Setup (Step-by-Step)

If you prefer to run steps manually:

### Step 1: Create Caddyfile

```bash
sudo nano /etc/caddy/Caddyfile
```

Paste the following configuration:

```
# Main application domains
portal.shahin-ai.com, app.shahin-ai.com {
    reverse_proxy localhost:8888 {
        header_up Host {host}
        header_up X-Real-IP {remote}
        header_up X-Forwarded-For {remote}
        header_up X-Forwarded-Proto {scheme}

        health_uri /health/ready
        health_interval 30s
        health_timeout 5s
    }

    header {
        Strict-Transport-Security "max-age=31536000; includeSubDomains; preload"
        X-Frame-Options "SAMEORIGIN"
        X-Content-Type-Options "nosniff"
        X-XSS-Protection "1; mode=block"
    }
}

# Landing page
shahin-ai.com, www.shahin-ai.com {
    reverse_proxy localhost:8888 {
        header_up Host {host}
        header_up X-Real-IP {remote}
        header_up X-Forwarded-For {remote}
        header_up X-Forwarded-Proto {scheme}
    }

    header {
        Strict-Transport-Security "max-age=31536000; includeSubDomains; preload"
        X-Frame-Options "SAMEORIGIN"
        X-Content-Type-Options "nosniff"
    }
}

# Login subdomain
login.shahin-ai.com {
    reverse_proxy localhost:8888 {
        header_up Host {host}
        header_up X-Real-IP {remote}
        header_up X-Forwarded-For {remote}
        header_up X-Forwarded-Proto {scheme}
    }
}

# Global options
{
    email admin@shahin-ai.com
    auto_https on
}
```

### Step 2: Validate Configuration

```bash
sudo caddy validate --config /etc/caddy/Caddyfile
```

Should return: `Valid configuration`

### Step 3: Restart Caddy

```bash
sudo systemctl restart caddy
sudo systemctl status caddy
```

### Step 4: Monitor Certificate Issuance

```bash
# Watch Caddy logs
sudo journalctl -u caddy -f

# Check for successful certificate issuance
# Look for: "certificate obtained successfully"
```

### Step 5: Test HTTPS

```bash
# Test each domain
curl -I https://portal.shahin-ai.com/health/ready
curl -I https://app.shahin-ai.com/health/ready
curl -I https://shahin-ai.com
curl -I https://www.shahin-ai.com
curl -I https://login.shahin-ai.com
```

All should return HTTP 200 or 301/302.

---

## Verification

### Check SSL Certificate

```bash
# View certificate details
openssl s_client -connect portal.shahin-ai.com:443 -servername portal.shahin-ai.com < /dev/null 2>/dev/null | openssl x509 -text -noout | grep -A 2 "Validity"

# Should show Let's Encrypt certificate with 90-day validity
```

### Test in Browser

1. Open: https://portal.shahin-ai.com
2. Check for green padlock
3. Click padlock → Certificate should show "Let's Encrypt"

### Check Caddy Logs

```bash
# View recent logs
sudo journalctl -u caddy --no-pager -n 100

# Follow logs in real-time
sudo journalctl -u caddy -f
```

---

## Troubleshooting

### Issue 1: DNS Not Resolving

**Error:** "dial tcp: lookup portal.shahin-ai.com: no such host"

**Solution:**
```bash
# Check DNS
dig +short portal.shahin-ai.com

# If empty, update your DNS records and wait 5-10 minutes
# Then restart Caddy
sudo systemctl restart caddy
```

### Issue 2: Port 80/443 Blocked

**Error:** "bind: address already in use" or "permission denied"

**Solution:**
```bash
# Check what's using the ports
sudo netstat -tlnp | grep -E ':80|:443'

# If something else is using them, stop it
# For example, if Apache is running:
sudo systemctl stop apache2

# Then restart Caddy
sudo systemctl restart caddy
```

### Issue 3: Let's Encrypt Rate Limit

**Error:** "too many certificates already issued"

**Solution:**
- Let's Encrypt has rate limits (50 certs/week per domain)
- Wait 7 days if limit reached
- Or use staging environment for testing:

```bash
# Edit Caddyfile and add under global options:
{
    acme_ca https://acme-staging-v02.api.letsencrypt.org/directory
}

# Restart Caddy
sudo systemctl restart caddy
```

### Issue 4: Firewall Blocking

**Error:** Connection timeout when accessing HTTPS

**Solution:**
```bash
# Check firewall status
sudo ufw status

# Allow ports
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# If using cloud provider, check security groups/firewall rules
```

### Issue 5: Certificate Not Renewing

**Error:** Certificate expired

**Solution:**
```bash
# Caddy auto-renews certificates
# Force renewal:
sudo systemctl restart caddy

# Check renewal logs
sudo journalctl -u caddy | grep renew
```

---

## Certificate Management

### Auto-Renewal

Caddy automatically renews certificates 30 days before expiration. No action required.

### Manual Renewal (if needed)

```bash
# Restart Caddy to trigger renewal check
sudo systemctl restart caddy

# Check certificate expiry
echo | openssl s_client -connect portal.shahin-ai.com:443 -servername portal.shahin-ai.com 2>/dev/null | openssl x509 -noout -dates
```

### View Certificates

```bash
# List all certificates
sudo ls -la /var/lib/caddy/certificates/acme-v02.api.letsencrypt.org-directory/

# Each domain will have:
# - *.crt (certificate)
# - *.key (private key)
# - *.json (metadata)
```

---

## Security Best Practices

### 1. HSTS Preloading

After SSL is working for 24 hours, submit to HSTS preload list:
- Visit: https://hstspreload.org/
- Submit: shahin-ai.com

### 2. Monitor Certificate Expiry

Set up monitoring:
```bash
# Add to cron (weekly check)
0 0 * * 0 curl -s https://portal.shahin-ai.com 2>&1 | grep -q "subject" || echo "SSL certificate issue" | mail -s "SSL Alert" admin@shahin-ai.com
```

### 3. Security Headers

Already configured in Caddyfile:
- ✅ HSTS (HTTP Strict Transport Security)
- ✅ X-Frame-Options (prevent clickjacking)
- ✅ X-Content-Type-Options (prevent MIME sniffing)
- ✅ X-XSS-Protection (XSS protection)

---

## Useful Commands

```bash
# Reload Caddy configuration (no downtime)
sudo systemctl reload caddy

# Restart Caddy
sudo systemctl restart caddy

# Check Caddy status
sudo systemctl status caddy

# View logs
sudo journalctl -u caddy -f

# Validate Caddyfile
sudo caddy validate --config /etc/caddy/Caddyfile

# Test configuration without applying
sudo caddy adapt --config /etc/caddy/Caddyfile

# Stop Caddy
sudo systemctl stop caddy

# Disable Caddy auto-start
sudo systemctl disable caddy
```

---

## Integration with Application

### Update .env file (if needed)

The application already uses the correct URLs:
```bash
App__BaseUrl=https://portal.shahin-ai.com
App__LandingUrl=https://shahin-ai.com
```

No changes needed in application configuration.

### Update Allowed Hosts

Already configured in .env:
```bash
ALLOWED_HOSTS=localhost;127.0.0.1;portal.shahin-ai.com;app.shahin-ai.com;shahin-ai.com;www.shahin-ai.com;login.shahin-ai.com
```

---

## Expected Timeline

1. **DNS Configuration:** 5-10 minutes (propagation time)
2. **Caddy Installation:** ✅ Already complete
3. **SSL Setup:** 2-5 minutes
4. **Certificate Issuance:** 1-2 minutes (Let's Encrypt)
5. **Testing:** 1-2 minutes

**Total:** ~10-20 minutes

---

## Success Criteria

✅ All domains resolve to correct IP
✅ Ports 80 and 443 are open
✅ Caddy service is running
✅ Let's Encrypt certificates issued
✅ HTTPS endpoints return 200 OK
✅ No certificate errors in browser
✅ Health check passes: https://portal.shahin-ai.com/health/ready

---

## Support

### View Caddy Documentation
- Official docs: https://caddyserver.com/docs/
- Automatic HTTPS: https://caddyserver.com/docs/automatic-https

### Check Certificate Status
- SSL Labs test: https://www.ssllabs.com/ssltest/analyze.html?d=portal.shahin-ai.com

### Get Help
```bash
# Caddy forum: https://caddy.community/
# GitHub issues: https://github.com/caddyserver/caddy/issues
```

---

## Summary

**Current Status:** Caddy installed, ready to configure

**Next Step:** Run the automated setup script:
```bash
sudo ./scripts/setup-ssl-caddy.sh
```

**Prerequisites:**
- ✅ Caddy installed
- ⚠️  DNS records (verify before running)
- ⚠️  Firewall ports 80/443 (verify before running)

**After setup:**
- All domains will have valid SSL certificates
- Automatic renewal every 60 days
- HTTPS enforced with security headers
- Application accessible via HTTPS

---

**Created:** 2026-01-11
**Status:** Ready to deploy
**Script:** `/home/Shahin-ai/Shahin-Jan-2026/scripts/setup-ssl-caddy.sh`
