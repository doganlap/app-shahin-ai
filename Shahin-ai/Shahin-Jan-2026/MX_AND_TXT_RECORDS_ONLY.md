# MX and TXT Records - Exact Configuration

**Domain:** shahin-ai.com  
**Total Records Needed:** 1 MX + 2 TXT

---

## ✅ MX Record (1 record)

### Keep This One (Already Correct):

```
Type: MX
Name: shahin-ai.com (or @)
Mail server: shahin-ai-com.mail.protection.outlook.com
Priority: 0
Proxy: DNS only
TTL: Auto
```

**Status:** ✅ Already configured correctly - **DO NOT CHANGE**

---

## ✅ TXT Records (2 records only)

### TXT Record 1: SPF (Add This)

```
Type: TXT
Name: @ (or shahin-ai.com - use @ for root)
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
Proxy: DNS only
TTL: Auto
```

**Action:** Add this record (delete any old SPF records first)

---

### TXT Record 2: DMARC (Keep This One)

```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
Proxy: DNS only
TTL: Auto
```

**Status:** ✅ Already configured correctly - **DO NOT CHANGE**

**Action:** Delete any other `_dmarc` records (you should only have this one)

---

## 📋 Summary

### What You Should Have:

**MX Records: 1**
- ✅ `shahin-ai.com` → `shahin-ai-com.mail.protection.outlook.com` (Priority 0)

**TXT Records: 2**
- ✅ `@` → `v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all`
- ✅ `_dmarc` → `v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r`

**Total: 3 records (1 MX + 2 TXT)**

---

## 🚨 Delete These (If They Exist)

### Delete These TXT Records:
- ❌ Any `_dmarc` record with `p=reject`
- ❌ Any `_dmarc` record with `p=none`
- ❌ Any `shahin-ai.com` TXT with `include:_spf.google.com` (wrong provider)
- ❌ Any `shahin-ai.com` TXT with `include:spf.protection.outlook.com` without `~all` (incomplete)

---

## ✅ Final Checklist

After cleanup, verify you have:

- [ ] **1 MX record** for `shahin-ai.com` → Microsoft 365
- [ ] **1 TXT record** for `@` → SPF (Microsoft 365 + server IPs)
- [ ] **1 TXT record** for `_dmarc` → DMARC quarantine policy
- [ ] **No duplicate** MX or TXT records
- [ ] **All email records** have Proxy = DNS only (gray cloud)

---

## 📝 Quick Copy-Paste Values

### MX Record (Keep As-Is):
```
Name: shahin-ai.com
Mail server: shahin-ai-com.mail.protection.outlook.com
Priority: 0
```

### TXT Record 1 - SPF (Add):
```
Name: @
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
```

### TXT Record 2 - DMARC (Keep As-Is):
```
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
```

---

**That's it! Just 1 MX + 2 TXT records for email authentication.**
