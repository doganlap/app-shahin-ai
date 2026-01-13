# DNS Records Cleanup - Action Required

**Issue:** Duplicate records detected  
**Date:** 2026-01-22

---

## 🚨 Critical Issues Found

### 1. **Multiple DMARC Records** (3 found - only 1 allowed!)

**Current Records:**
- ❌ `_dmarc` → `p=reject` (too strict for now)
- ❌ `_dmarc` → `p=quarantine` (good, but duplicate)
- ❌ `_dmarc` → `p=none` (old monitoring record)

**Action:** Delete 2, keep 1

**Keep This One:**
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
Proxy: DNS only
TTL: Auto
```

**Delete These:**
- ❌ Delete: `_dmarc` with `p=reject` (too strict for initial setup)
- ❌ Delete: `_dmarc` with `p=none` (old monitoring record)

---

### 2. **Multiple SPF Records** (2 found - only 1 allowed!)

**Current Records:**
- ❌ `shahin-ai.com` → `v=spf1 include:spf.protection.outlook.com` (incomplete - missing ~all)
- ❌ `shahin-ai.com` → `v=spf1 include:_spf.google.com ~all` (wrong - you're using Microsoft 365, not Google)

**Action:** Delete both, add 1 correct one

**Delete Both:**
- ❌ Delete: `shahin-ai.com` with `include:spf.protection.outlook.com` (incomplete)
- ❌ Delete: `shahin-ai.com` with `include:_spf.google.com` (wrong provider)

**Add This Correct One:**
```
Type: TXT
Name: @ (or shahin-ai.com)
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
Proxy: DNS only
TTL: Auto
```

---

## ✅ Correct DNS Records (Final State)

### Email Authentication Records

1. **SPF (TXT) - ONE record only:**
   ```
   Type: TXT
   Name: @ (or shahin-ai.com)
   Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
   Proxy: DNS only
   TTL: Auto
   ```

2. **DKIM (CNAME) - Both correct ✅:**
   ```
   ✅ selector1._domainkey → selector1-shahin-ai-com._domainkey.outlook.com
   ✅ selector2._domainkey → selector2-shahin-ai-com._domainkey.outlook.com
   ```

3. **DMARC (TXT) - ONE record only:**
   ```
   Type: TXT
   Name: _dmarc
   Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
   Proxy: DNS only
   TTL: Auto
   ```

4. **MX Record - Correct ✅:**
   ```
   ✅ shahin-ai.com → shahin-ai-com.mail.protection.outlook.com (Priority 0)
   ```

### Web Records (A Records)

All A records are correct ✅:
- ✅ app → 46.224.68.73 (Proxied)
- ✅ login → 46.224.68.73 (Proxied)
- ✅ portal → 46.224.68.73 (Proxied)
- ✅ shahin-ai.com → 46.224.68.73 (Proxied)
- ✅ www → 46.224.68.73 (Proxied)

---

## 📋 Step-by-Step Cleanup

### Step 1: Delete Duplicate DMARC Records

1. Find the DMARC record with `p=reject`
   - Click **"Edit"** or delete button
   - **Delete** this record

2. Find the DMARC record with `p=none`
   - Click **"Edit"** or delete button
   - **Delete** this record

3. Keep the DMARC record with `p=quarantine`
   - Verify it has the full content:
     ```
     v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
     ```
   - If it's missing any part, edit it to match exactly

### Step 2: Delete Duplicate/Incorrect SPF Records

1. Find the SPF record with `include:spf.protection.outlook.com` (incomplete)
   - Click **"Edit"** or delete button
   - **Delete** this record

2. Find the SPF record with `include:_spf.google.com` (wrong provider)
   - Click **"Edit"** or delete button
   - **Delete** this record

3. Add the correct SPF record:
   - Click **"Add record"**
   - Type: `TXT`
   - Name: `@` (or leave blank for root domain)
   - Content: `v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all`
   - Proxy: **DNS only** (gray cloud - OFF)
   - TTL: Auto
   - Click **"Save"**

---

## ✅ Verification Checklist

After cleanup, you should have:

- [ ] **1 SPF record** (TXT for `@` or `shahin-ai.com`)
- [ ] **2 DKIM records** (CNAME for selector1 and selector2) ✅ Already correct
- [ ] **1 DMARC record** (TXT for `_dmarc`)
- [ ] **1 MX record** ✅ Already correct
- [ ] **5 A records** ✅ Already correct

**Total:** 10 records (5 A + 1 MX + 1 SPF + 2 DKIM + 1 DMARC)

---

## 🎯 Why This Matters

### Duplicate Records Cause Problems:

1. **Multiple DMARC records:**
   - Email servers don't know which policy to follow
   - Can cause authentication failures
   - May result in emails being rejected incorrectly

2. **Multiple SPF records:**
   - SPF validation may fail
   - Email servers can't determine authorized senders
   - Can cause legitimate emails to be marked as spam

3. **Wrong SPF provider:**
   - Using Google SPF when you use Microsoft 365
   - Microsoft 365 emails will fail SPF checks
   - All emails from Microsoft 365 will be rejected/quarantined

---

## 📊 Current vs. Correct State

### Current (Wrong):
- ❌ 3 DMARC records (should be 1)
- ❌ 2 SPF records (should be 1)
- ❌ SPF points to Google (should be Microsoft 365)
- ✅ 2 DKIM records (correct)
- ✅ 1 MX record (correct)
- ✅ 5 A records (correct)

### After Cleanup (Correct):
- ✅ 1 DMARC record (`p=quarantine`)
- ✅ 1 SPF record (Microsoft 365 + server IPs)
- ✅ 2 DKIM records (Microsoft 365)
- ✅ 1 MX record (Microsoft 365)
- ✅ 5 A records

---

## 🚀 Quick Action Summary

**Delete:**
1. DMARC with `p=reject`
2. DMARC with `p=none`
3. SPF with `include:spf.protection.outlook.com` (incomplete)
4. SPF with `include:_spf.google.com` (wrong)

**Keep:**
1. DMARC with `p=quarantine` (verify it's complete)
2. Both DKIM records ✅
3. MX record ✅
4. All A records ✅

**Add:**
1. Correct SPF record (see Step 2 above)

---

**After cleanup, wait 15-30 minutes for DNS propagation, then verify with:**
```bash
dig TXT shahin-ai.com +short
dig TXT _dmarc.shahin-ai.com +short
```
