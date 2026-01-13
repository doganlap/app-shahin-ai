# DNS Records to Add - Quick Reference

## 📋 Copy-Paste DNS Records

### 1. SPF Record (TXT)
```
Type: TXT
Name: @ (or leave blank for root domain)
Content: v=spf1 include:spf.protection.outlook.com include:_spf.shahin-ai.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
TTL: Auto
Proxy: DNS only
```

### 2. DMARC Record - Quarantine Policy (Recommended)
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
TTL: Auto
Proxy: DNS only
```

### 3. DMARC Record - Reject Policy (After monitoring)
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=reject; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=reject; aspf=r; adkim=r
TTL: Auto
Proxy: DNS only
```

### 4. DKIM Records (Get from Microsoft 365 Admin Center)

**Steps:**
1. Go to Microsoft 365 Admin Center
2. Exchange Admin Center → Protection → DKIM
3. Enable DKIM for `shahin-ai.com`
4. Copy the CNAME records provided

**Typical format:**
```
Type: CNAME
Name: selector1._domainkey
Content: selector1-shahin-ai-com._domainkey.outlook.com
TTL: Auto
Proxy: DNS only

Type: CNAME
Name: selector2._domainkey
Content: selector2-shahin-ai-com._domainkey.outlook.com
TTL: Auto
Proxy: DNS only
```

---

## ⚠️ Important: Update A Records

**Current:** All A records point to `46.224.68.73`  
**Should be:** `157.180.105.48` (if that's your production server)

**Update these A records:**
- `app` → 157.180.105.48
- `login` → 157.180.105.48
- `portal` → 157.180.105.48
- `shahin-ai.com` → 157.180.105.48
- `www` → 157.180.105.48

---

## 🎯 DMARC Policy Decision Guide

### What happens to failing emails?

**Option 1: Quarantine (Recommended)**
- ✅ Failing emails go to spam folder
- ✅ Recipients can still access them
- ✅ Less disruptive
- ✅ Good for initial rollout

**Option 2: Reject (Strict)**
- ✅ Failing emails are completely rejected
- ✅ Maximum protection
- ⚠️ Legitimate emails might be lost if misconfigured
- ⚠️ Use only after monitoring period

**Option 3: None (Current)**
- ✅ Monitoring only
- ✅ No action taken
- ⚠️ No protection
- ⚠️ Use only for initial setup/testing

---

## 📊 Recommended Timeline

**Week 1-2: Setup & Monitor**
- Add SPF record
- Enable DKIM
- Set DMARC to `p=none` (monitoring)

**Week 3-4: Review Reports**
- Check DMARC reports at `dmarc@shahin-ai.com`
- Verify all legitimate emails pass
- Fix any authentication issues

**Week 5-6: Quarantine**
- Update DMARC to `p=quarantine`
- Monitor spam folder complaints
- Continue reviewing reports

**Week 7-8: Final Review**
- If <1% failure rate: Move to reject
- If >1% failure rate: Investigate and fix issues

**Week 9+: Reject (Production)**
- Update DMARC to `p=reject`
- Maximum email security
- Continue monitoring reports

---

## ✅ Verification Checklist

After adding records, verify:

- [ ] SPF record resolves correctly
- [ ] DKIM records resolve correctly
- [ ] DMARC record resolves correctly
- [ ] Test email sending works
- [ ] DMARC reports are being received
- [ ] All legitimate emails pass authentication

---

**Use this guide when adding DNS records in your DNS management interface.**
