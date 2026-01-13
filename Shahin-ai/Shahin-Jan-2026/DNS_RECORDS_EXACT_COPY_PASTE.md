# DNS Records - Exact Copy-Paste Values

**Domain:** shahin-ai.com  
**For:** DNS Management Interface

---

## 🔧 Records to Add/Update

### 1. SPF Record (TXT) - Add if Missing

**In your DNS interface, click "Add record" or "Edit" the existing TXT record for root domain:**

```
Type: TXT
Name: @ (or leave blank for root domain)
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
TTL: Auto
Proxy: DNS only (gray cloud - OFF)
```

**Note:** If you already have a TXT record for `@` or `shahin-ai.com`, edit it to include this SPF content.

---

### 2. DMARC Record - Quarantine Policy (Recommended)

**Edit the existing `_dmarc` record:**

```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
TTL: Auto
Proxy: DNS only (gray cloud - OFF)
```

**What this does:** Emails that fail authentication go to spam folder (quarantine).

---

### 3. DMARC Record - Reject Policy (Strict - Use Later)

**After monitoring for 4+ weeks, update to:**

```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=reject; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=reject; aspf=r; adkim=r
TTL: Auto
Proxy: DNS only (gray cloud - OFF)
```

**What this does:** Emails that fail authentication are completely rejected.

---

### 4. DKIM Records (Get from Microsoft 365)

**Steps:**
1. Go to Microsoft 365 Admin Center
2. Exchange Admin Center → Protection → DKIM
3. Enable DKIM for `shahin-ai.com`
4. Copy the CNAME records shown

**Typical format (example - use actual values from Microsoft 365):**

```
Type: CNAME
Name: selector1._domainkey
Content: selector1-shahin-ai-com._domainkey.outlook.com
TTL: Auto
Proxy: DNS only (gray cloud - OFF)

Type: CNAME
Name: selector2._domainkey
Content: selector2-shahin-ai-com._domainkey.outlook.com
TTL: Auto
Proxy: DNS only (gray cloud - OFF)
```

---

## ⚠️ Important: Update A Records (If Needed)

**Current:** All A records point to `46.224.68.73`  
**Check:** If your production server is `157.180.105.48`, update these:

```
app → 157.180.105.48
login → 157.180.105.48
portal → 157.180.105.48
shahin-ai.com → 157.180.105.48
www → 157.180.105.48
```

---

## 📋 Complete Checklist

### Email Authentication Records

- [ ] **SPF Record:** Added/Updated (TXT record for `@`)
- [ ] **DKIM Records:** Added (CNAME records from Microsoft 365)
- [ ] **DMARC Record:** Updated to quarantine or reject
- [ ] **MX Record:** Already exists ✅ (Microsoft 365)

### Web Records

- [ ] **A Records:** Verify IP addresses are correct
- [ ] **All records:** Proxy set to "DNS only" (gray cloud) for email records

---

## 🎯 Quick Action Items

1. **Edit DMARC record:** Change `p=none` to `p=quarantine`
2. **Add/Update SPF record:** Ensure it includes Microsoft 365 and your server IPs
3. **Get DKIM records:** Enable DKIM in Microsoft 365 and add CNAME records
4. **Create email:** Set up `dmarc@shahin-ai.com` to receive reports
5. **Monitor:** Check reports for 2-4 weeks before moving to reject

---

**All email-related records MUST have Proxy = OFF (gray cloud, DNS only)**
