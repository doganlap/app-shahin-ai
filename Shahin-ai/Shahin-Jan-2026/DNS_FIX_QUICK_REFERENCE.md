# DNS Fix - Quick Reference

## 🚨 Delete These Records (Duplicates/Wrong)

### Delete DMARC Records:
1. ❌ `_dmarc` → `p=reject` (delete)
2. ❌ `_dmarc` → `p=none` (delete)
3. ✅ `_dmarc` → `p=quarantine` (KEEP - but verify it's complete)

### Delete SPF Records:
1. ❌ `shahin-ai.com` → `v=spf1 include:spf.protection.outlook.com` (delete - incomplete)
2. ❌ `shahin-ai.com` → `v=spf1 include:_spf.google.com ~all` (delete - wrong provider)

---

## ✅ Add This Record (Correct SPF)

```
Type: TXT
Name: @ (or shahin-ai.com)
Content: v=spf1 include:spf.protection.outlook.com ip4:46.224.68.73 ip4:157.180.105.48 ~all
Proxy: DNS only
TTL: Auto
```

---

## ✅ Verify DMARC Record (Keep This One)

Make sure your remaining DMARC record has:

```
Type: TXT
Name: _dmarc
Content: v=DMARC1; p=quarantine; rua=mailto:dmarc@shahin-ai.com; ruf=mailto:dmarc@shahin-ai.com; pct=100; sp=quarantine; aspf=r; adkim=r
Proxy: DNS only
TTL: Auto
```

If it's missing any part, edit it to match exactly.

---

## 📊 Final Count

After cleanup:
- ✅ 1 SPF record
- ✅ 2 DKIM records (already correct)
- ✅ 1 DMARC record
- ✅ 1 MX record (already correct)
- ✅ 5 A records (already correct)

**Total: 10 records**
