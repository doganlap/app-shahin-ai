# DNS Deployment Checklist

**Domain:** shahin-ai.com  
**Server IP:** 46.224.68.73  
**Date:** 2026-01-22

---

## ✅ Required DNS Records

### A Records (5 records) - Point to Server IP

| Type | Name | Value | Proxy | TTL |
|------|------|-------|-------|-----|
| A | `shahin-ai.com` (or `@`) | `46.224.68.73` | DNS only (OFF) | Auto |
| A | `www` | `46.224.68.73` | DNS only (OFF) | Auto |
| A | `app` | `46.224.68.73` | DNS only (OFF) | Auto |
| A | `portal` | `46.224.68.73` | DNS only (OFF) | Auto |
| A | `login` | `46.224.68.73` | DNS only (OFF) | Auto |

**Status:** ⚠️ **VERIFY IN CLOUDFLARE**

---

### CNAME Records (2 records) - DKIM for Email

| Type | Name | Target | Proxy | TTL |
|------|------|--------|-------|-----|
| CNAME | `selector1._domainkey` | `selector1-shahin-ai-com._domainkey.outlook.com` | DNS only (OFF) | Auto |
| CNAME | `selector2._domainkey` | `selector2-shahin-ai-com._domainkey.outlook.com` | DNS only (OFF) | Auto |

**Status:** ⚠️ **VERIFY IN CLOUDFLARE**

---

### MX Record (1 record) - Email Delivery

| Type | Name | Mail Server | Priority | Proxy | TTL |
|------|------|-------------|----------|-------|-----|
| MX | `shahin-ai.com` (or `@`) | `shahin-ai-com.mail.protection.outlook.com` | 0 | DNS only (OFF) | Auto |

**Status:** ✅ **ALREADY CONFIGURED** (from previous setup)

---

### TXT Records (2 records) - Email Authentication

#### SPF Record:
| Type | Name | Value | Proxy | TTL |
|------|------|-------|-------|-----|
| TXT | `shahin-ai.com` (or `@`) | `v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all` | DNS only (OFF) | Auto |

#### DMARC Record:
| Type | Name | Value | Proxy | TTL |
|------|------|-------|-------|-----|
| TXT | `_dmarc` | `v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; pct=100` | DNS only (OFF) | Auto |

**Status:** ⚠️ **VERIFY IN CLOUDFLARE**

---

## 🔍 Verification Commands

### Check DNS Resolution:
```bash
# Check A records
nslookup shahin-ai.com
nslookup www.shahin-ai.com
nslookup app.shahin-ai.com
nslookup portal.shahin-ai.com
nslookup login.shahin-ai.com

# All should return: 46.224.68.73

# Check MX record
nslookup -type=MX shahin-ai.com
# Should return: shahin-ai-com.mail.protection.outlook.com

# Check TXT records
nslookup -type=TXT shahin-ai.com
nslookup -type=TXT _dmarc.shahin-ai.com
```

### Online DNS Checkers:
- https://dnschecker.org/
- https://www.whatsmydns.net/
- https://mxtoolbox.com/

---

## ⚠️ Important Notes

1. **DNS Propagation:** Changes can take 24-48 hours to propagate globally
2. **Proxy Status:** All records should have **Proxy OFF** (DNS only - gray cloud)
3. **TTL:** Use Auto or 3600 seconds (1 hour) for faster updates
4. **Verification:** Wait 5-10 minutes after changes, then verify with nslookup

---

## 📋 Deployment Steps

1. **Log in to Cloudflare** (or your DNS provider)
2. **Navigate to DNS settings** for shahin-ai.com
3. **Add/Update A records** for all 5 subdomains
4. **Add/Update CNAME records** for DKIM
5. **Verify MX record** is correct
6. **Add/Update TXT records** for SPF and DMARC
7. **Wait 5-10 minutes** for initial propagation
8. **Verify with nslookup** commands above
9. **Test public access** once DNS propagates

---

## ✅ Quick Copy-Paste Values

### A Records:
```
shahin-ai.com → 46.224.68.73
www → 46.224.68.73
app → 46.224.68.73
portal → 46.224.68.73
login → 46.224.68.73
```

### CNAME Records:
```
selector1._domainkey → selector1-shahin-ai-com._domainkey.outlook.com
selector2._domainkey → selector2-shahin-ai-com._domainkey.outlook.com
```

### TXT Records:
```
shahin-ai.com → v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
_dmarc → v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; pct=100
```

---

**Status:** ⚠️ **ACTION REQUIRED - Configure DNS Records**

**Last Updated:** 2026-01-22
