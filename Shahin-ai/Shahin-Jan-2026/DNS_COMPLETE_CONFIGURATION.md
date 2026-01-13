# Complete DNS Configuration for shahin-ai.com

**Domain:** shahin-ai.com  
**Email Service:** Microsoft 365 (Office 365)  
**Date:** 2026-01-22

---

## 📋 Complete DNS Records (10 Total)

### Email Authentication (4 records)
- 1 MX record
- 2 CNAME records (DKIM)
- 2 TXT records (SPF + DMARC)

### Web Records (5 records)
- 5 A records

---

## ✅ 1. MX Record (1 record)

**Purpose:** Email delivery (Microsoft 365)

```
Type: MX
Name: shahin-ai.com (or @)
Mail server: shahin-ai-com.mail.protection.outlook.com
Priority: 0
Proxy: DNS only (gray cloud - OFF)
TTL: Auto
```

**Status:** ✅ Already configured correctly

---

## ✅ 2. CNAME Records - DKIM (2 records)

**Purpose:** Email authentication (Microsoft 365 DKIM)

### Record 1:
```
Type: CNAME
Name: selector1._domainkey
Target: selector1-shahin-ai-com._domainkey.outlook.com
Proxy: DNS only (gray cloud - OFF)
TTL: Auto
```

### Record 2:
```
Type: CNAME
Name: selector2._domainkey
Target: selector2-shahin-ai-com._domainkey.outlook.com
Proxy: DNS only (gray cloud - OFF)
TTL: Auto
```

**Status:** ✅ Already configured correctly

---

## ✅ 3. TXT Records (2 records)

### TXT Record 1: SPF
**Purpose:** Prevent email spoofing

```
Type: TXT
Name: @ (or shahin-ai.com - use @ for root)
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
Proxy: DNS only (gray cloud - OFF)
TTL: Auto
```

**Action:** Add this record (delete any old SPF records first)

---

### TXT Record 2: DMARC
**Purpose:** Email authentication policy

```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
Proxy: DNS only (gray cloud - OFF)
TTL: Auto
```

**Action:** Keep this one (delete any other `_dmarc` records)

---

## ✅ 4. A Records (5 records)

**Purpose:** Web traffic routing

### Record 1: Root Domain
```
Type: A
Name: @ (or shahin-ai.com)
Content: 46.224.68.73
Proxy: Proxied (orange cloud - ON)
TTL: Auto
```

### Record 2: App Subdomain
```
Type: A
Name: app
Content: 46.224.68.73
Proxy: Proxied (orange cloud - ON)
TTL: Auto
```

### Record 3: Login Subdomain
```
Type: A
Name: login
Content: 46.224.68.73
Proxy: Proxied (orange cloud - ON)
TTL: Auto
```

### Record 4: Portal Subdomain
```
Type: A
Name: portal
Content: 46.224.68.73
Proxy: Proxied (orange cloud - ON)
TTL: Auto
```

### Record 5: WWW Subdomain
```
Type: A
Name: www
Content: 46.224.68.73
Proxy: Proxied (orange cloud - ON)
TTL: Auto
```

**Status:** ✅ Already configured correctly

---

## 📊 Summary Table

| Type | Name | Content/Target | Proxy | Count |
|------|------|---------------|-------|-------|
| **MX** | shahin-ai.com | shahin-ai-com.mail.protection.outlook.com (Priority 0) | DNS only | 1 |
| **CNAME** | selector1._domainkey | selector1-shahin-ai-com._domainkey.outlook.com | DNS only | 1 |
| **CNAME** | selector2._domainkey | selector2-shahin-ai-com._domainkey.outlook.com | DNS only | 1 |
| **TXT** | @ | v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all | DNS only | 1 |
| **TXT** | _dmarc | v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r | DNS only | 1 |
| **A** | @ | 46.224.68.73 | Proxied | 1 |
| **A** | app | 46.224.68.73 | Proxied | 1 |
| **A** | login | 46.224.68.73 | Proxied | 1 |
| **A** | portal | 46.224.68.73 | Proxied | 1 |
| **A** | www | 46.224.68.73 | Proxied | 1 |

**Total: 10 records**

---

## 🚨 Important Rules

### Proxy Settings:
- **Email records (MX, CNAME, TXT):** MUST be **DNS only** (gray cloud - OFF)
- **Web records (A):** Can be **Proxied** (orange cloud - ON)

### Record Limits:
- **Only 1 MX record** per domain
- **Only 1 SPF record** per domain (TXT for root)
- **Only 1 DMARC record** per domain (TXT for `_dmarc`)
- **2 DKIM records** (CNAME for selector1 and selector2)
- **5 A records** (root + 4 subdomains)

---

## ✅ Verification Checklist

After configuration, verify you have:

- [ ] **1 MX record** → Microsoft 365
- [ ] **2 CNAME records** → DKIM (selector1, selector2)
- [ ] **1 TXT record** → SPF (root domain)
- [ ] **1 TXT record** → DMARC (`_dmarc`)
- [ ] **5 A records** → All pointing to 46.224.68.73
- [ ] **No duplicate** records
- [ ] **All email records** have Proxy = DNS only
- [ ] **All web records** have Proxy = Proxied

---

## 🔧 Quick Copy-Paste Values

### Email Records (DNS only - gray cloud):

**MX:**
```
Name: shahin-ai.com
Mail server: shahin-ai-com.mail.protection.outlook.com
Priority: 0
```

**CNAME 1:**
```
Name: selector1._domainkey
Target: selector1-shahin-ai-com._domainkey.outlook.com
```

**CNAME 2:**
```
Name: selector2._domainkey
Target: selector2-shahin-ai-com._domainkey.outlook.com
```

**TXT - SPF:**
```
Name: @
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
```

**TXT - DMARC:**
```
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
```

### Web Records (Proxied - orange cloud):

**A Records (all same IP):**
```
@ → 46.224.68.73
app → 46.224.68.73
login → 46.224.68.73
portal → 46.224.68.73
www → 46.224.68.73
```

---

## 🎯 Final Configuration

**Email Records (4):**
- 1 MX
- 2 CNAME (DKIM)
- 2 TXT (SPF + DMARC)

**Web Records (5):**
- 5 A (all same IP)

**Total: 10 records**

---

**This is your complete, clean DNS configuration. No duplicates, no confusion.**
