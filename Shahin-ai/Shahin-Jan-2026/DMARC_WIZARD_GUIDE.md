# DMARC Policy Wizard Guide

**Domain:** shahin-ai.com  
**Email Service:** Microsoft 365 (Office 365)  
**Current DMARC:** `p=none` (Monitoring only)

---

## 🎯 DMARC Policy Decision

### What happens to emails that fail authentication?

Choose one of these policies:

---

### Option 1: **Quarantine** (Recommended - Start Here) ✅

**What it does:**
- ✅ Emails that fail authentication go to the **spam/junk folder**
- ✅ Recipients can still access them if needed
- ✅ Less disruptive than rejecting
- ✅ Good for initial rollout

**When to use:**
- Starting DMARC enforcement
- After 2-4 weeks of monitoring
- When you want protection but need flexibility

**DNS Record to Update:**
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
TTL: Auto
Proxy: DNS only
```

---

### Option 2: **Reject** (Strict - Use After Monitoring) 🔒

**What it does:**
- ✅ Emails that fail authentication are **completely rejected**
- ✅ Maximum protection against spoofing
- ⚠️ Legitimate emails might be lost if misconfigured
- ⚠️ Use only after confirming all emails pass

**When to use:**
- After 4-8 weeks of successful quarantine
- When <1% of legitimate emails fail
- When maximum security is required

**DNS Record to Update:**
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=reject; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=reject; aspf=r; adkim=r
TTL: Auto
Proxy: DNS only
```

---

### Option 3: **None** (Current - Monitoring Only) 👀

**What it does:**
- ✅ No action taken on failing emails
- ✅ Only collects reports
- ⚠️ No protection against spoofing
- ⚠️ Use only for testing/initial setup

**When to use:**
- Initial setup phase
- Testing email authentication
- First 2-4 weeks of monitoring

**Current Record (Keep as-is for now):**
```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100
TTL: Auto
Proxy: DNS only
```

---

## 📋 Step-by-Step: Update DMARC Record

### In Your DNS Management Interface:

1. **Find the existing DMARC record:**
   - Look for: `Type: TXT`, `Name: _dmarc`
   - Click **"Edit"** button

2. **Update the Content field:**

   **For Quarantine (Recommended):**
   ```
   v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
   ```

   **For Reject (Strict):**
   ```
   v=DMARC1; p=reject; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=reject; aspf=r; adkim=r
   ```

3. **Keep these settings:**
   - **Type:** TXT
   - **Name:** _dmarc
   - **TTL:** Auto
   - **Proxy:** DNS only (gray cloud - OFF)

4. **Click "Save"**

---

## 🔍 What Each Parameter Means

| Parameter | Value | Meaning |
|-----------|-------|---------|
| `v=DMARC1` | Version | DMARC version 1 |
| `p=quarantine` | Policy | What to do with failing emails |
| `rua=mailto:dmarc@shahin-ai.com` | Reports | Where to send daily summary reports |
| `ruf=mailto:dmarc@shahin-ai.com` | Forensic | Where to send failure reports |
| `pct=100` | Percentage | Apply to 100% of emails |
| `sp=quarantine` | Subdomain | Policy for subdomains (same as domain) |
| `aspf=r` | SPF Alignment | Relaxed SPF alignment |
| `adkim=r` | DKIM Alignment | Relaxed DKIM alignment |

---

## ✅ Required: Complete Email Authentication Setup

Before enabling DMARC enforcement, ensure you have:

### 1. SPF Record (Required)

**Check if exists:** Look for a TXT record with name `@` or `shahin-ai.com`

**If missing, add:**
```
Type: TXT
Name: @ (or shahin-ai.com)
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
TTL: Auto
Proxy: DNS only
```

### 2. DKIM Records (Required for Microsoft 365)

**Get from Microsoft 365 Admin Center:**

1. Go to **Microsoft 365 Admin Center**
2. Navigate to **Exchange Admin Center** → **Protection** → **DKIM**
3. Enable DKIM for `shahin-ai.com`
4. Copy the **CNAME records** provided

**Typical records to add:**
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

### 3. DMARC Record (You're updating this)

Already covered above.

---

## 📊 Recommended Timeline

### Week 1-2: Setup & Monitor
- ✅ Add SPF record
- ✅ Enable DKIM in Microsoft 365
- ✅ Keep DMARC at `p=none` (monitoring)
- ✅ Check reports at `dmarc@shahin-ai.com`

### Week 3-4: Review Reports
- ✅ Verify all legitimate emails pass authentication
- ✅ Fix any authentication issues
- ✅ Ensure <5% failure rate

### Week 5-6: Quarantine
- ✅ Update DMARC to `p=quarantine`
- ✅ Monitor spam folder complaints
- ✅ Continue reviewing reports

### Week 7-8: Final Review
- ✅ If <1% failure rate: Move to reject
- ✅ If >1% failure rate: Investigate and fix

### Week 9+: Reject (Production)
- ✅ Update DMARC to `p=reject`
- ✅ Maximum email security
- ✅ Continue monitoring reports

---

## 🚨 Important Notes

### Before Enabling Enforcement:

1. **Verify SPF is working:**
   - Test: `dig TXT shahin-ai.com +short`
   - Should show: `"v=spf1 include:spf.protection.outlook.com ..."`

2. **Verify DKIM is enabled:**
   - Check Microsoft 365 Admin Center
   - Ensure DKIM shows "Enabled" for shahin-ai.com

3. **Monitor reports:**
   - Create email: `dmarc@shahin-ai.com`
   - Check for daily aggregate reports
   - Review for any authentication failures

4. **Test email sending:**
   - Send test emails from your application
   - Verify they pass SPF and DKIM checks
   - Use tools like: https://mxtoolbox.com/dmarc.aspx

---

## 🎯 Quick Decision Guide

**Choose Quarantine if:**
- ✅ You're just starting DMARC enforcement
- ✅ You want protection but need flexibility
- ✅ You want to monitor before going strict
- ✅ **RECOMMENDED for most cases**

**Choose Reject if:**
- ✅ You've been monitoring for 4+ weeks
- ✅ All legitimate emails pass authentication
- ✅ You need maximum security
- ✅ You're confident in your email setup

**Keep None if:**
- ✅ You're still setting up SPF/DKIM
- ✅ You're in the first 2 weeks of monitoring
- ✅ You want to collect data first

---

## 📧 DMARC Report Email Setup

**Create:** `dmarc@shahin-ai.com`

**Purpose:**
- Receive daily aggregate reports (summary)
- Receive real-time forensic reports (detailed failures)

**Setup:**
1. Create mailbox in Microsoft 365
2. Or set up forwarding to your admin email
3. Monitor reports for authentication issues

---

## ✅ Verification Checklist

After updating DMARC:

- [ ] SPF record exists and resolves
- [ ] DKIM is enabled in Microsoft 365
- [ ] DMARC record updated with new policy
- [ ] DMARC reports email (`dmarc@shahin-ai.com`) is set up
- [ ] Test email sending works
- [ ] All legitimate emails pass authentication
- [ ] Monitoring reports for 2-4 weeks before moving to reject

---

**Recommendation:** Start with **Quarantine** policy, monitor for 2-4 weeks, then move to **Reject** if all looks good.
