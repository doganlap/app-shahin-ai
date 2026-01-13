# Exact DNS Records to Add in Cloudflare - Step by Step

**Domain**: shahin-ai.com  
**Server IP**: 46.224.68.73  
**Date**: 2025-01-22

---

## 📋 Step-by-Step: Add DNS Records in Cloudflare

### Step 1: Add SPF Record (Required - Prevents Email Spoofing)

**In Cloudflare Dashboard:**

1. Click **"Add record"** button
2. Fill in exactly:
   - **Type**: Select `TXT`
   - **Name**: `@` (or leave blank - means root domain)
   - **Content**: `v=spf1 include:_spf.google.com ~all`
   - **Proxy status**: Click gray cloud to turn **OFF** (must be gray, not orange!)
   - **TTL**: `Auto`
3. Click **"Save"**

**Expected Result:**
- Shows in DNS list as: `TXT @ "v=spf1 include:_spf.google.com ~all"`
- Proxy icon is **gray** (OFF)

---

### Step 2: Add DMARC Record (Required - Email Authentication Policy)

**In Cloudflare Dashboard:**

1. Click **"Add record"** button
2. Fill in exactly:
   - **Type**: Select `TXT`
   - **Name**: `_dmarc` (include the underscore!)
   - **Content**: `v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100`
   - **Proxy status**: Click gray cloud to turn **OFF** (must be gray!)
   - **TTL**: `Auto`
3. Click **"Save"**

**Expected Result:**
- Shows in DNS list as: `TXT _dmarc "v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100"`
- Proxy icon is **gray** (OFF)
- Name must have underscore: `_dmarc`

---

### Step 3: Add MX Record (Required if you want to receive emails)

**Choose ONE option based on your email service:**

#### Option A: If using Google Workspace

Add **5 MX records** (Google requires multiple for redundancy):

**MX Record 1:**
- **Type**: `MX`
- **Name**: `@` (or leave blank)
- **Priority**: `1`
- **Mail server**: `aspmx.l.google.com`
- **Proxy**: **OFF** (gray cloud)
- **TTL**: `Auto`

**MX Record 2:**
- **Type**: `MX`
- **Name**: `@`
- **Priority**: `5`
- **Mail server**: `alt1.aspmx.l.google.com`
- **Proxy**: **OFF**
- **TTL**: `Auto`

**MX Record 3:**
- **Type**: `MX`
- **Name**: `@`
- **Priority**: `5`
- **Mail server**: `alt2.aspmx.l.google.com`
- **Proxy**: **OFF**
- **TTL**: `Auto`

**MX Record 4:**
- **Type**: `MX`
- **Name**: `@`
- **Priority**: `10`
- **Mail server**: `alt3.aspmx.l.google.com`
- **Proxy**: **OFF**
- **TTL**: `Auto`

**MX Record 5:**
- **Type**: `MX`
- **Name**: `@`
- **Priority**: `10`
- **Mail server**: `alt4.aspmx.l.google.com`
- **Proxy**: **OFF**
- **TTL**: `Auto`

---

#### Option B: If using Microsoft Office 365

Add **1 MX record**:

- **Type**: `MX`
- **Name**: `@` (or leave blank)
- **Priority**: `0`
- **Mail server**: `shahin-ai-com.mail.protection.outlook.com`
- **Proxy**: **OFF** (gray cloud)
- **TTL**: `Auto`

**Also update SPF to:**
- **Type**: `TXT`
- **Name**: `@`
- **Content**: `v=spf1 include:spf.protection.outlook.com ~all`
- **Proxy**: **OFF**

---

#### Option C: If hosting your own mail server

Add **2 records**:

**1. MX Record:**
- **Type**: `MX`
- **Name**: `@` (or leave blank)
- **Priority**: `10`
- **Mail server**: `mail.shahin-ai.com`
- **Proxy**: **OFF** (gray cloud)
- **TTL**: `Auto`

**2. A Record for mail subdomain:**
- **Type**: `A`
- **Name**: `mail`
- **Content**: `46.224.68.73`
- **Proxy**: **OFF** (gray cloud)
- **TTL**: `Auto`

---

#### Option D: If NO email service yet (Prevent spoofing only - no receiving emails)

Add **2 records**:

**1. MX Record (will fail until you set up mail server):**
- **Type**: `MX`
- **Name**: `@` (or leave blank)
- **Priority**: `10`
- **Mail server**: `mail.shahin-ai.com`
- **Proxy**: **OFF** (gray cloud)
- **TTL**: `Auto`

**2. A Record for mail subdomain:**
- **Type**: `A`
- **Name**: `mail`
- **Content**: `46.224.68.73`
- **Proxy**: **OFF** (gray cloud)
- **TTL**: `Auto`

**Also use restrictive SPF:**
- **Type**: `TXT`
- **Name**: `@`
- **Content**: `v=spf1 -all` (prevents all email sending until you configure)
- **Proxy**: **OFF**

---

### Step 4: Add DKIM Record (Optional - Get from your email provider)

**Only if using Google Workspace or Office 365:**

#### For Google Workspace:

1. Go to **Google Admin Console** → **Apps** → **Google Workspace** → **Gmail**
2. Click **"Authenticate email"**
3. Select domain: **shahin-ai.com**
4. Click **"Generate new record"** (or view existing)
5. Copy the **TXT record value** (looks like: `v=DKIM1; k=rsa; p=MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC...`)
6. Add in Cloudflare:
   - **Type**: `TXT`
   - **Name**: `google._domainkey` (selector from Google)
   - **Content**: `[Paste the value from Google Admin Console]`
   - **Proxy**: **OFF** (gray cloud)
   - **TTL**: `Auto`

#### For Office 365:

1. Go to **Office 365 Admin Center** → **Exchange** → **Mail Flow** → **DKIM**
2. Enable DKIM for **shahin-ai.com**
3. Copy the **CNAME records** provided (usually `selector1._domainkey` and `selector2._domainkey`)
4. Add as **CNAME** records (not TXT) in Cloudflare with Proxy **OFF**

---

## 📊 Summary Table: Minimum Required Records

| Type | Name | Content/Value | Priority | Proxy | Purpose |
|------|------|---------------|----------|-------|---------|
| **TXT** | `@` | `v=spf1 include:_spf.google.com ~all` | - | **OFF** | Prevents email spoofing |
| **TXT** | `_dmarc` | `v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100` | - | **OFF** | Email authentication policy |
| **MX** | `@` | `aspmx.l.google.com` (or your mail server) | 1-10 | **OFF** | Receives emails (if using email service) |

---

## ⚠️ Critical Reminders

### Proxy Status (VERY IMPORTANT!)

- ✅ **ALL email records MUST have Proxy = OFF** (gray cloud)
- ❌ **NEVER enable proxy** (orange cloud) for MX, SPF, DKIM, or DMARC records
- ✅ Web records (A records) can have Proxy = ON (orange cloud)
- ❌ Email records (MX, TXT) MUST have Proxy = OFF (gray cloud)

### Record Names

- **Root domain**: Use `@` or leave blank (both mean shahin-ai.com)
- **DMARC**: Must be exactly `_dmarc` (with underscore, no quotes)
- **DKIM**: Usually `google._domainkey` or `selector1._domainkey` (get from email provider)

### Content Format

- **SPF**: Must start with `v=spf1` and end with `~all` or `-all`
- **DMARC**: Must start with `v=DMARC1` and include policy (`p=none`, `p=quarantine`, or `p=reject`)
- **MX**: Must be a valid mail server hostname (not an IP address)

---

## ✅ Verification Checklist

After adding all records, verify:

- [ ] All records show **"DNS only"** (gray cloud) in Cloudflare
- [ ] SPF record exists: `v=spf1 ...`
- [ ] DMARC record exists: `v=DMARC1 ...`
- [ ] MX records added (if using email service)
- [ ] DKIM added (if configured with email provider)
- [ ] No typos in record names or content
- [ ] All records saved (not in edit mode)

---

## 🧪 Quick Test Commands

After adding records, test with:

```bash
# Check SPF
dig shahin-ai.com TXT +short | grep spf

# Check DMARC
dig _dmarc.shahin-ai.com TXT +short

# Check MX
dig shahin-ai.com MX +short

# Check all TXT records
dig shahin-ai.com TXT +short
```

---

## 📝 Recommended Order of Adding Records

1. ✅ **First**: Add SPF record (prevents spoofing immediately)
2. ✅ **Second**: Add DMARC with `p=none` (start monitoring mode)
3. ✅ **Third**: Add MX records (if you have email service)
4. ✅ **Fourth**: Add DKIM (after configuring with email provider)

---

## 🆘 Common Mistakes to Avoid

❌ **DON'T** enable proxy (orange cloud) for email records  
❌ **DON'T** use IP addresses in MX records (must use hostname)  
❌ **DON'T** forget the underscore in `_dmarc`  
❌ **DON'T** use quotes in Content field (Cloudflare adds them automatically)  
❌ **DON'T** use `www._dmarc` (use `_dmarc` only)  
❌ **DON'T** set TTL too high (use Auto or 300-3600 seconds)

---

## 📞 Quick Reference

**Current Server IP**: 46.224.68.73  
**Domain**: shahin-ai.com  
**Cloudflare Dashboard**: https://dash.cloudflare.com  
**DNS Zone**: shahin-ai.com

**Minimum Setup (Prevent Spoofing Only):**
- 1 TXT record for SPF
- 1 TXT record for DMARC

**Full Email Setup (Receive & Send Emails):**
- 1-5 MX records (depending on provider)
- 1 TXT record for SPF
- 1 TXT record for DMARC
- 1-2 TXT/CNAME records for DKIM (from email provider)

---

**Last Updated**: 2025-01-22  
**Status**: Ready to add  
**Next Step**: Add SPF and DMARC records first, then MX if needed
