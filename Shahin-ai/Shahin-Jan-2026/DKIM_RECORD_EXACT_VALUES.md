# DKIM CNAME Record - Exact Values

**For:** Microsoft 365 DKIM Setup  
**Domain:** shahin-ai.com

---

## 📋 DKIM Record 1: selector1

**Fill in your DNS form exactly like this:**

```
Type: CNAME
Name: selector1._domainkey
Target: selector1-shahin-ai-com._domainkey.outlook.com
Proxy status: DNS only (gray cloud - OFF)
TTL: Auto
Comment: Microsoft 365 DKIM selector1 (optional - for your reference only)
```

**Important:**
- **Name:** `selector1._domainkey` (include the underscore and dot)
- **Target:** `selector1-shahin-ai-com._domainkey.outlook.com` (this is what goes in the "Target" field)
- **Comment:** Optional - just for your reference, doesn't affect DNS

---

## 📋 DKIM Record 2: selector2

**After adding the first one, add a second record:**

```
Type: CNAME
Name: selector2._domainkey
Target: selector2-shahin-ai-com._domainkey.outlook.com
Proxy status: DNS only (gray cloud - OFF)
TTL: Auto
Comment: Microsoft 365 DKIM selector2 (optional - for your reference only)
```

---

## ⚠️ Important Notes

### Field Mapping:
- **"Name" field** = `selector1._domainkey`
- **"Target" field** = `selector1-shahin-ai-com._domainkey.outlook.com` ← **This is the important one!**
- **"Comment" field** = Optional documentation (doesn't affect DNS resolution)

### Common Mistakes:
- ❌ Don't put the target value in the "Comment" field
- ❌ Don't forget the underscore in `_domainkey`
- ❌ Don't enable proxy (must be "DNS only")
- ✅ The "Target" field is what actually points the CNAME

---

## ✅ Verification

After adding both records, verify with:

```bash
dig CNAME selector1._domainkey.shahin-ai.com +short
# Should return: selector1-shahin-ai-com._domainkey.outlook.com

dig CNAME selector2._domainkey.shahin-ai.com +short
# Should return: selector2-shahin-ai-com._domainkey.outlook.com
```

---

## 📝 Quick Copy-Paste

**Record 1:**
- Name: `selector1._domainkey`
- Target: `selector1-shahin-ai-com._domainkey.outlook.com`

**Record 2:**
- Name: `selector2._domainkey`
- Target: `selector2-shahin-ai-com._domainkey.outlook.com`

---

**Note:** If Microsoft 365 shows different selector names or targets, use those instead. These are the typical values, but always verify in your Microsoft 365 Admin Center.
