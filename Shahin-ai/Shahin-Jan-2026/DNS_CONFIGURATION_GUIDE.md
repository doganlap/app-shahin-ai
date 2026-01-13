# DNS Configuration Guide for shahin-ai.com

**Date:** 2026-01-22  
**Purpose:** Complete DNS setup including DMARC, SPF, DKIM for email authentication

---

## 📋 Current DNS Records Status

### ✅ Existing Records
- **A Records:** All pointing to `46.224.68.73` ✅
  - `app.shahin-ai.com` → 46.224.68.73
  - `login.shahin-ai.com` → 46.224.68.73
  - `portal.shahin-ai.com` → 46.224.68.73
  - `shahin-ai.com` → 46.224.68.73
  - `www.shahin-ai.com` → 46.224.68.73

- **MX Record:** ✅ Configured
  - `shahin-ai.com` → `shahin-ai-com.mail.protection.outlook.com` (Priority 0)

- **DMARC Record:** ⚠️ Monitoring Only
  - `_dmarc.shahin-ai.com` → `"DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100"`

### ⚠️ Missing Records
- **SPF Record:** Not visible (needs verification)
- **DKIM Record:** Not visible (needs verification)
- **TXT Record for shahin-ai.com:** Incomplete

---

## 🔧 Required DNS Records

### 1. SPF Record (Sender Policy Framework)

**Type:** TXT  
**Name:** `@` or `shahin-ai.com`  
**Content:**
```
v=spf1 include:spf.protection.outlook.com include:_spf.shahin-ai.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
```

**Explanation:**
- `include:spf.protection.outlook.com` - Allows Microsoft 365 to send emails
- `include:_spf.shahin-ai.com` - Allows custom SPF subdomain (if needed)
- `ip4:46.224.68.73` - Your current server IP
- `ip4:157.180.105.48` - Alternative server IP (if applicable)
- `~all` - Soft fail for other sources (use `-all` for hard fail in production)

---

### 2. DKIM Record (DomainKeys Identified Mail)

**For Microsoft 365:**
Microsoft 365 automatically generates DKIM keys. You need to:
1. Go to Microsoft 365 Admin Center
2. Navigate to Exchange Admin Center → Protection → DKIM
3. Enable DKIM for `shahin-ai.com`
4. Copy the CNAME records provided by Microsoft

**Typical DKIM Records (from Microsoft 365):**
```
Type: CNAME
Name: selector1._domainkey
Content: selector1-shahin-ai-com._domainkey.outlook.com

Type: CNAME
Name: selector2._domainkey
Content: selector2-shahin-ai-com._domainkey.outlook.com
```

---

### 3. DMARC Record (Domain-based Message Authentication)

**Current (Monitoring Only):**
```
Type: TXT
Name: _dmarc
Content: "DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100"
```

**Recommended for Production:**

#### Option 1: Quarantine (Recommended for gradual rollout)
```
Type: TXT
Name: _dmarc
Content: "DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r"
```

#### Option 2: Reject (Strict - use after monitoring)
```
Type: TXT
Name: _dmarc
Content: "DMARC1; p=reject; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=reject; aspf=r; adkim=r"
```

**DMARC Policy Parameters:**
- `p=quarantine` - Quarantine emails that fail (move to spam)
- `p=reject` - Reject emails that fail (hard reject)
- `p=none` - Monitor only (current setting)
- `rua=mailto:dmarc@shahin-ai.com` - Aggregate reports email
- `ruf=mailto:dmarc@shahin-ai.com` - Forensic reports email
- `pct=100` - Apply to 100% of emails
- `sp=quarantine` - Subdomain policy (same as domain)
- `aspf=r` - SPF alignment (relaxed)
- `adkim=r` - DKIM alignment (relaxed)

---

### 4. Complete TXT Record for shahin-ai.com

**Type:** TXT  
**Name:** `@` or `shahin-ai.com`  
**Content:** (This should be your SPF record - see above)

---

## 🎯 Step-by-Step DMARC Setup Wizard

### Step 1: Choose DMARC Policy

**For Initial Setup (Recommended):**
1. Start with **Quarantine** (`p=quarantine`)
   - Failing emails go to spam folder
   - Less disruptive than reject
   - Allows monitoring of issues

**After 30 days of monitoring:**
2. Move to **Reject** (`p=reject`)
   - Failing emails are completely rejected
   - Maximum protection
   - Only use after confirming all legitimate emails pass

### Step 2: Configure Reporting

**Aggregate Reports (rua):**
- Email: `dmarc@shahin-ai.com`
- Frequency: Daily
- Purpose: Summary of authentication results

**Forensic Reports (ruf):**
- Email: `dmarc@shahin-ai.com`
- Frequency: Real-time
- Purpose: Detailed reports on failures

### Step 3: Set Coverage Percentage

- **100%** - Apply to all emails (recommended)
- **Lower %** - Gradual rollout (e.g., 25%, 50%, 75%)

### Step 4: Subdomain Policy

- **Same as domain** (`sp=quarantine` or `sp=reject`)
- **Different policy** - Set separately if needed

---

## 📝 Complete DNS Records to Add/Update

### Record 1: SPF (TXT)
```
Type: TXT
Name: @ (or shahin-ai.com)
Content: v=spf1 include:spf.protection.outlook.com include:_spf.shahin-ai.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
TTL: Auto (or 3600)
Proxy: DNS only
```

### Record 2: DMARC - Quarantine (Recommended)
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
TTL: Auto (or 3600)
Proxy: DNS only
```

### Record 3: DMARC - Reject (After monitoring period)
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=reject; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=reject; aspf=r; adkim=r
TTL: Auto (or 3600)
Proxy: DNS only
```

### Record 4: DKIM (From Microsoft 365)
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

## ✅ Verification Steps

### 1. Verify SPF Record
```bash
dig TXT shahin-ai.com +short
# Should show: "v=spf1 include:spf.protection.outlook.com ..."
```

### 2. Verify DKIM Records
```bash
dig CNAME selector1._domainkey.shahin-ai.com +short
dig CNAME selector2._domainkey.shahin-ai.com +short
```

### 3. Verify DMARC Record
```bash
dig TXT _dmarc.shahin-ai.com +short
# Should show: "v=DMARC1; p=quarantine; ..."
```

### 4. Test Email Authentication
- Use tools like:
  - https://mxtoolbox.com/dmarc.aspx
  - https://www.dmarcanalyzer.com/
  - https://dmarcian.com/dmarc-xml/

---

## 🚨 Important Notes

### Server IP Update
**Current A Records:** Pointing to `46.224.68.73`  
**Application Server:** `157.180.105.48` (from deployment)

**Action Required:**
- Update A records to point to `157.180.105.48` if that's your production server
- Or verify which IP is correct for your deployment

### Email Sending
- **Microsoft 365:** Primary email service (configured via MX record)
- **Application:** May send emails via SMTP (needs SPF record)
- **Both:** Must be included in SPF record

### DMARC Policy Recommendation

**Week 1-4: Monitoring**
```
p=none; rua=mailto:dmarc@shahin-ai.com; pct=100
```

**Week 5-8: Quarantine**
```
p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine
```

**Week 9+: Reject (if all looks good)**
```
p=reject; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=reject
```

---

## 📧 DMARC Report Email Setup

Create email account: `dmarc@shahin-ai.com`

**Purpose:**
- Receive daily aggregate reports
- Receive real-time forensic reports
- Monitor authentication failures

**Setup:**
1. Create mailbox in Microsoft 365
2. Set up forwarding/aliasing if needed
3. Monitor reports for 2-4 weeks before moving to quarantine/reject

---

## 🔍 Quick Reference

### Current Status
- ✅ A Records: Configured (but check IP)
- ✅ MX Record: Configured
- ⚠️ SPF: Needs verification/addition
- ⚠️ DKIM: Needs Microsoft 365 setup
- ⚠️ DMARC: Monitoring only (needs upgrade)

### Recommended Actions
1. ✅ Verify/Add SPF record
2. ✅ Enable DKIM in Microsoft 365
3. ✅ Update DMARC to quarantine policy
4. ✅ Monitor for 2-4 weeks
5. ✅ Upgrade to reject policy

---

**Last Updated:** 2026-01-22
