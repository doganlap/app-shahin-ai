# DNS Records Verification Checklist

**Domain:** shahin-ai.com  
**Date:** 2026-01-22

---

## ✅ What Should Be There (Correct State)

### Email Authentication (4 records)
1. ✅ **1 SPF Record** (TXT for `@` or `shahin-ai.com`)
2. ✅ **2 DKIM Records** (CNAME for selector1 and selector2)
3. ✅ **1 DMARC Record** (TXT for `_dmarc`)
4. ✅ **1 MX Record** (MX for `shahin-ai.com`)

### Web Records (5 records)
5. ✅ **5 A Records** (app, login, portal, shahin-ai.com, www)

**Total: 10 records**

---

## 🔍 Current Status Check

### ✅ CORRECT Records (Keep These)

**DKIM Records (2) - ✅ Correct:**
- ✅ `selector1._domainkey` → `selector1-shahin-ai-com._domainkey.outlook.com`
- ✅ `selector2._domainkey` → `selector2-shahin-ai-com._domainkey.outlook.com`

**MX Record (1) - ✅ Correct:**
- ✅ `shahin-ai.com` → `shahin-ai-com.mail.protection.outlook.com` (Priority 0)

**A Records (5) - ✅ Correct:**
- ✅ `app` → `46.224.68.73` (Proxied)
- ✅ `login` → `46.224.68.73` (Proxied)
- ✅ `portal` → `46.224.68.73` (Proxied)
- ✅ `shahin-ai.com` → `46.224.68.73` (Proxied)
- ✅ `www` → `46.224.68.73` (Proxied)

---

## ❌ ISSUES Found (Need Action)

### Issue 1: Multiple DMARC Records (2-3 found, need only 1)

**Current DMARC Records:**
- ❌ `_dmarc` → `p=reject` (DELETE - too strict)
- ✅ `_dmarc` → `p=quarantine` (KEEP - this is the correct one)
- ❌ `_dmarc` → `p=none` (DELETE - old monitoring record)

**Action:** Delete all except the `p=quarantine` one.

**Keep This:**
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
```

---

### Issue 2: Multiple/Wrong SPF Records (2 found, need only 1)

**Current SPF Records:**
- ❌ `shahin-ai.com` → `v=spf1 include:spf.protection.outlook.com` (DELETE - incomplete, missing ~all)
- ❌ `shahin-ai.com` → `v=spf1 include:_spf.google.com ~all` (DELETE - wrong provider, you use Microsoft 365)

**Action:** Delete both, add 1 correct one.

**Add This:**
```
Type: TXT
Name: @ (or shahin-ai.com)
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
Proxy: DNS only
TTL: Auto
```

---

## 📋 Action Items Checklist

### Step 1: Clean Up DMARC Records
- [ ] Delete: `_dmarc` with `p=reject`
- [ ] Delete: `_dmarc` with `p=none`
- [ ] Verify: `_dmarc` with `p=quarantine` is complete (has all parameters)

### Step 2: Fix SPF Records
- [ ] Delete: `shahin-ai.com` with `include:spf.protection.outlook.com` (incomplete)
- [ ] Delete: `shahin-ai.com` with `include:_spf.google.com` (wrong provider)
- [ ] Add: New SPF record with correct content (see above)

### Step 3: Verify Final State
- [ ] Count DMARC records: Should be **1**
- [ ] Count SPF records: Should be **1**
- [ ] Count DKIM records: Should be **2** ✅
- [ ] Count MX records: Should be **1** ✅
- [ ] Count A records: Should be **5** ✅

---

## ✅ Final Correct Configuration

After cleanup, you should have exactly:

### Email Records (4 total)
1. **SPF (TXT):**
   ```
   Name: @
   Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
   ```

2. **DKIM (CNAME) - 2 records:**
   ```
   selector1._domainkey → selector1-shahin-ai-com._domainkey.outlook.com
   selector2._domainkey → selector2-shahin-ai-com._domainkey.outlook.com
   ```

3. **DMARC (TXT):**
   ```
   Name: _dmarc
   Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
   ```

4. **MX:**
   ```
   shahin-ai.com → shahin-ai-com.mail.protection.outlook.com (Priority 0)
   ```

### Web Records (5 total)
- app → 46.224.68.73
- login → 46.224.68.73
- portal → 46.224.68.73
- shahin-ai.com → 46.224.68.73
- www → 46.224.68.73

---

## 🚨 Why This Matters

**Multiple DMARC records:**
- Email servers will be confused about which policy to use
- May cause authentication failures
- Can result in legitimate emails being rejected

**Multiple/Wrong SPF records:**
- SPF validation will fail
- Microsoft 365 emails will be rejected
- All emails from your domain will fail authentication

---

## ✅ Verification Commands

After cleanup, verify with:

```bash
# Check SPF
dig TXT shahin-ai.com +short
# Should show: "v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all"

# Check DMARC
dig TXT _dmarc.shahin-ai.com +short
# Should show: "v=DMARC1; p=quarantine; ..."

# Check DKIM
dig CNAME selector1._domainkey.shahin-ai.com +short
dig CNAME selector2._domainkey.shahin-ai.com +short
```

---

**Status:** ⚠️ **Needs Cleanup** - Multiple duplicate records detected
