# Cloudflare Email DNS Configuration for shahin-ai.com

## 📧 Required DNS Records for Email

### Current Status
- **Domain**: shahin-ai.com
- **Current IP**: 46.224.68.73
- **Nameservers**: elle.ns.cloudflare.com, grant.ns.cloudflare.com
- **Missing**: MX, SPF, DKIM, DMARC records

---

## 🔧 DNS Records to Add in Cloudflare

### 1. MX Record (Mail Exchange) - **REQUIRED**

**If using external email service (Office 365, Google Workspace, etc.):**

| Type | Name | Priority | Content | Proxy | TTL |
|------|------|----------|---------|-------|-----|
| MX | @ (root) | 10 | `mail.shahin-ai.com` | 🟡 DNS only | Auto |

**OR if using Office 365:**
```
MX    @    10    shahin-ai-com.mail.protection.outlook.com    DNS only    Auto
```

**OR if using Google Workspace:**
```
MX    @    1     aspmx.l.google.com                           DNS only    Auto
MX    @    5     alt1.aspmx.l.google.com                      DNS only    Auto
MX    @    5     alt2.aspmx.l.google.com                      DNS only    Auto
MX    @    10    alt3.aspmx.l.google.com                      DNS only    Auto
MX    @    10    alt4.aspmx.l.google.com                      DNS only    Auto
```

**OR if hosting your own mail server (requires mail server on 46.224.68.73):**
```
MX    @    10    mail.shahin-ai.com                           DNS only    Auto
A     mail      46.224.68.73                                  DNS only    Auto
```

---

### 2. SPF Record (Sender Policy Framework) - **REQUIRED**

| Type | Name | Content | Proxy | TTL |
|------|------|---------|-------|-----|
| TXT | @ (root) | `v=spf1 include:_spf.google.com ~all` | 🟡 DNS only | Auto |

**Options:**

**For Google Workspace:**
```
TXT   @    v=spf1 include:_spf.google.com ~all               DNS only    Auto
```

**For Office 365:**
```
TXT   @    v=spf1 include:spf.protection.outlook.com ~all    DNS only    Auto
```

**For custom SMTP server (your own mail server):**
```
TXT   @    v=spf1 ip4:46.224.68.73 include:_spf.google.com ~all    DNS only    Auto
```

**Restrictive (only allow specified servers):**
```
TXT   @    v=spf1 ip4:46.224.68.73 -all                      DNS only    Auto
```

**Explanation:**
- `v=spf1` = SPF version 1
- `ip4:46.224.68.73` = Allow this IP to send emails
- `include:_spf.google.com` = Include Google's SPF records
- `~all` = Soft fail for others (recommended)
- `-all` = Hard fail for others (strict)

---

### 3. DKIM Record (DomainKeys Identified Mail) - **RECOMMENDED**

DKIM requires generating a public/private key pair. **Get this from your email provider.**

**For Google Workspace:**
1. Go to Google Admin Console → Apps → Google Workspace → Gmail
2. Find "Authenticate email" section
3. Generate DKIM key (format: `google._domainkey`)

| Type | Name | Content | Proxy | TTL |
|------|------|---------|-------|-----|
| TXT | google._domainkey | `v=DKIM1; k=rsa; p=MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC...` | 🟡 DNS only | Auto |

**For Office 365:**
1. Office 365 Admin Center → Exchange → Mail Flow → DKIM
2. Enable DKIM and copy the CNAME records provided

**Example (Google Workspace format):**
```
TXT   google._domainkey    v=DKIM1; k=rsa; p=MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC...    DNS only    Auto
```

**For custom mail server**, you need to:
1. Generate DKIM keys: `openssl genrsa -out private.key 2048`
2. Extract public key: `openssl rsa -in private.key -pubout -out public.key`
3. Create TXT record with selector (e.g., `default._domainkey`)

---

### 4. DMARC Record (Domain-based Message Authentication) - **REQUIRED**

| Type | Name | Content | Proxy | TTL |
|------|------|---------|-------|-----|
| TXT | _dmarc | `v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100` | 🟡 DNS only | Auto |

**Recommended DMARC policies:**

**Monitoring Mode (start here):**
```
TXT   _dmarc    v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100    DNS only    Auto
```

**Quarantine Mode (after testing):**
```
TXT   _dmarc    v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r    DNS only    Auto
```

**Reject Mode (strict - use after monitoring):**
```
TXT   _dmarc    v=DMARC1; p=reject; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=reject; aspf=r    DNS only    Auto
```

**Explanation:**
- `v=DMARC1` = DMARC version 1
- `p=none` = No action (monitoring only)
- `p=quarantine` = Send to spam folder
- `p=reject` = Reject email entirely
- `rua=mailto:dmarc@shahin-ai.com` = Aggregate reports email
- `ruf=mailto:dmarc@shahin-ai.com` = Forensic reports email
- `pct=100` = Apply to 100% of emails
- `sp=quarantine` = Subdomain policy
- `aspf=r` = SPF alignment (strict)

---

## 📋 Complete DNS Records Checklist

### Current Records (Keep These)
- ✅ A record: app → 46.224.68.73
- ✅ A record: login → 46.224.68.73
- ✅ A record: portal → 46.224.68.73
- ✅ A record: @ (shahin-ai.com) → 46.224.68.73
- ✅ A record: www → 46.224.68.73

### New Records to Add

**For Basic Email (Recommended Minimum):**

1. ✅ **MX Record** - Choose ONE based on your email provider:
   - Option A: Google Workspace (see above)
   - Option B: Office 365 (see above)
   - Option C: Custom mail server on your server

2. ✅ **SPF (TXT record)** - Prevent email spoofing:
   ```
   TXT   @    v=spf1 include:_spf.google.com ~all
   ```

3. ✅ **DMARC (TXT record)** - Start in monitoring mode:
   ```
   TXT   _dmarc    v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100
   ```

**For Full Email Security (Complete Setup):**

4. ✅ **DKIM (TXT record)** - Get from your email provider:
   ```
   TXT   google._domainkey    [your-public-key-from-email-provider]
   ```

---

## 🚀 Step-by-Step: Add Records in Cloudflare

### Step 1: Add MX Record

1. Go to Cloudflare Dashboard → DNS → shahin-ai.com
2. Click **"Add record"**
3. Select **MX** from Type dropdown
4. Fill in:
   - **Name**: `@` (root domain) or leave blank
   - **Priority**: `10`
   - **Mail server**: `mail.shahin-ai.com` OR your email provider's MX hostname
   - **Proxy status**: ⚪ **DNS only** (gray cloud - DISABLED proxy)
   - **TTL**: Auto
5. Click **"Save"**

### Step 2: Add SPF Record

1. Click **"Add record"**
2. Select **TXT** from Type dropdown
3. Fill in:
   - **Name**: `@` (root domain) or leave blank
   - **Content**: `v=spf1 include:_spf.google.com ~all`
   - **Proxy status**: ⚪ **DNS only** (gray cloud)
   - **TTL**: Auto
4. Click **"Save"**

### Step 3: Add DMARC Record

1. Click **"Add record"**
2. Select **TXT** from Type dropdown
3. Fill in:
   - **Name**: `_dmarc`
   - **Content**: `v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100`
   - **Proxy status**: ⚪ **DNS only** (gray cloud)
   - **TTL**: Auto
4. Click **"Save"**

### Step 4: Add DKIM Record (if available)

1. Get DKIM public key from your email provider
2. Click **"Add record"**
3. Select **TXT** from Type dropdown
4. Fill in:
   - **Name**: `google._domainkey` (or your provider's selector)
   - **Content**: `v=DKIM1; k=rsa; p=YOUR_PUBLIC_KEY_HERE`
   - **Proxy status**: ⚪ **DNS only** (gray cloud)
   - **TTL**: Auto
5. Click **"Save"**

---

## ⚠️ Important Notes

### Proxy Status (Critical!)
- **ALL email-related records (MX, TXT for SPF/DKIM/DMARC) MUST be "DNS only" (gray cloud = OFF)**
- **DO NOT enable proxy (orange cloud) for email records** - it will break email delivery
- **A records for web can be proxied (orange cloud), but email records cannot**

### DNS Propagation
- Changes typically take effect within **5-15 minutes** on Cloudflare
- Can take up to **48 hours** globally (usually much faster)
- Test with: `dig shahin-ai.com MX` or `nslookup -type=MX shahin-ai.com`

### Testing Your Records

**Test MX:**
```bash
dig shahin-ai.com MX +short
# Should return: 10 mail.shahin-ai.com
```

**Test SPF:**
```bash
dig shahin-ai.com TXT +short | grep spf
# Should return: "v=spf1 include:_spf.google.com ~all"
```

**Test DMARC:**
```bash
dig _dmarc.shahin-ai.com TXT +short
# Should return: "v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100"
```

**Test DKIM:**
```bash
dig google._domainkey.shahin-ai.com TXT +short
# Should return your DKIM public key
```

---

## 🔐 Security Recommendations

### Phase 1: Setup (Week 1)
- ✅ Add MX record
- ✅ Add SPF with `~all` (soft fail)
- ✅ Add DMARC with `p=none` (monitoring mode)
- ✅ Monitor DMARC reports

### Phase 2: Hardening (Week 2-4)
- ✅ Review DMARC aggregate reports
- ✅ Add DKIM record
- ✅ Change DMARC to `p=quarantine`
- ✅ Update SPF to `-all` (hard fail) if not using third-party services

### Phase 3: Strict (After 1 Month)
- ✅ Change DMARC to `p=reject`
- ✅ Set `pct=100` and `aspf=r` (strict alignment)
- ✅ Set `sp=reject` for subdomains

---

## 📧 Email Service Options

### Option 1: Google Workspace (Recommended)
- **Cost**: $6/user/month
- **MX**: `aspmx.l.google.com` (priority 1)
- **SPF**: `v=spf1 include:_spf.google.com ~all`
- **DKIM**: Provided by Google (in Admin Console)

### Option 2: Microsoft Office 365
- **Cost**: $6/user/month
- **MX**: `shahin-ai-com.mail.protection.outlook.com` (priority 0)
- **SPF**: `v=spf1 include:spf.protection.outlook.com ~all`
- **DKIM**: Enabled in Exchange Admin Center

### Option 3: Self-Hosted (Mail Server on 46.224.68.73)
- **Cost**: Free (but requires mail server setup)
- **MX**: `mail.shahin-ai.com` (priority 10)
- **SPF**: `v=spf1 ip4:46.224.68.73 -all`
- **DKIM**: Generate your own keys

### Option 4: Zoho Mail (Free Tier Available)
- **Cost**: Free for 5 users
- **MX**: `mx.zoho.com` (priority 10)
- **SPF**: `v=spf1 include:zoho.com ~all`
- **DKIM**: Provided by Zoho

---

## ✅ Verification Checklist

After adding all records, verify:

- [ ] MX record resolves: `dig shahin-ai.com MX`
- [ ] SPF record exists: `dig shahin-ai.com TXT | grep spf`
- [ ] DMARC record exists: `dig _dmarc.shahin-ai.com TXT`
- [ ] DKIM record exists (if configured): `dig google._domainkey.shahin-ai.com TXT`
- [ ] All records show **"DNS only"** (gray cloud) in Cloudflare
- [ ] Can receive test email at `test@shahin-ai.com`
- [ ] DMARC reports are being sent to `dmarc@shahin-ai.com`
- [ ] Email authentication score: 10/10 on [mail-tester.com](https://www.mail-tester.com)

---

## 🆘 Troubleshooting

### MX Record Not Working
- Verify proxy is OFF (gray cloud)
- Check MX priority is correct (10 is standard)
- Wait 15-30 minutes for DNS propagation
- Test with: `dig shahin-ai.com MX`

### SPF Validation Failing
- Ensure SPF record doesn't exceed 255 characters
- Check for multiple SPF records (only one allowed)
- Use [SPF Record Checker](https://mxtoolbox.com/spf.aspx)

### DMARC Not Working
- Verify record name is exactly `_dmarc` (with underscore)
- Check email address in rua/ruf is valid
- Start with `p=none` and monitor first

### DKIM Validation Failing
- Verify public key is correctly formatted
- Check selector name matches your email provider
- Ensure private key matches on mail server

---

## 📞 Quick Reference

**Current Server IP**: 46.224.68.73
**Domain**: shahin-ai.com
**Cloudflare Dashboard**: https://dash.cloudflare.com
**DNS Zone**: shahin-ai.com

**Minimum Required Records:**
```
MX    @    10    mail.shahin-ai.com
TXT   @    v=spf1 include:_spf.google.com ~all
TXT   _dmarc    v=DMARC1; p=none; rua=mailto:dmarc@shahin-ai.com; pct=100
```

---

**Last Updated**: 2025-01-22
**Status**: Ready to configure
**Next Step**: Add records in Cloudflare dashboard following Step-by-Step guide above
