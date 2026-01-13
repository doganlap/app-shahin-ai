# i18n Quick Reference Card

## 🎯 View Your Work Now

**Development Server:** http://localhost:3000

**How to Test:**
1. Click the **Globe icon** (🌐) in the top-right header
2. Select "English" or "العربية"
3. Watch the search box text change languages instantly

## 📁 File Locations

| What | Where |
|------|-------|
| **English translations** | `/public/locales/en/*.json` |
| **Arabic translations** | `/public/locales/ar/*.json` |
| **i18n config** | `/src/lib/i18n.ts` |
| **Language selector** | `/src/components/LanguageSelector.tsx` |
| **Documentation** | `I18N_SETUP.md` (full guide) |

## 🔧 Basic Usage

### Convert a Component (5 Steps)

```tsx
// 1. Add directive at top
'use client';

// 2. Import hook
import { useTranslation } from 'react-i18next';

// 3. Use hook in component
export default function MyPage() {
  const { t } = useTranslation('dashboard'); // ← namespace name

  // 4. Replace hardcoded strings
  return (
    <div>
      <h1>{t('title')}</h1>
      <p>{t('subtitle')}</p>
    </div>
  );
}
```

### Translation Namespaces

| Namespace | Use For | File |
|-----------|---------|------|
| `common` | Navigation, buttons, status | `common.json` |
| `auth` | Login page | `auth.json` |
| `dashboard` | Dashboard page | `dashboard.json` |
| `controls` | Controls page | `controls.json` |
| `evidence` | Evidence page | `evidence.json` |
| `reports` | Reports page | `reports.json` |
| `remediation` | Remediation page | `remediation.json` |

### Common Translation Keys

```tsx
const { t } = useTranslation('common');

// Navigation
t('navigation.dashboard')    // "Dashboard" / "لوحة التحكم"
t('navigation.controls')     // "Controls" / "الضوابط"
t('navigation.evidence')     // "Evidence" / "الأدلة"

// Actions
t('actions.export')          // "Export" / "تصدير"
t('actions.add')             // "Add" / "إضافة"
t('actions.delete')          // "Delete" / "حذف"

// Status
t('status.compliant')        // "Compliant" / "متوافق"
t('status.pending')          // "Pending Review" / "قيد المراجعة"
t('status.approved')         // "Approved" / "معتمد"

// Common UI
t('common.loading')          // "Loading..." / "جاري التحميل..."
t('common.noData')           // "No data available" / "لا توجد بيانات"
```

## 📝 What to Migrate (Priority Order)

| # | Component | File | Priority | Time |
|---|-----------|------|----------|------|
| 1 | Login | `/src/app/(auth)/login/page.tsx` | 🔴 Critical | 30m |
| 2 | Sidebar | `/src/components/layout/Sidebar.tsx` | 🔴 High | 45m |
| 3 | Dashboard | `/src/app/(app)/dashboard/page.tsx` | 🟡 High | 1h |
| 4 | Controls | `/src/app/(app)/controls/page.tsx` | 🟡 Medium | 1.5h |
| 5 | Evidence | `/src/app/(app)/evidence/page.tsx` | 🟢 Medium | 1h |
| 6 | Reports | `/src/app/(app)/reports/page.tsx` | 🟢 Low | 1h |
| 7 | Remediation | `/src/app/(app)/remediation/page.tsx` | 🟢 Low | 1.5h |

**Total Estimated Time:** ~10 hours

## 🚀 Quick Migration Example

### Before (Hardcoded)

```tsx
// src/app/(auth)/login/page.tsx
export default function LoginPage() {
  return (
    <div>
      <h1>Sign in to your account</h1>
      <input placeholder="Email" />
      <input placeholder="Password" />
      <button>Sign in</button>
    </div>
  );
}
```

### After (Translated)

```tsx
'use client';

import { useTranslation } from 'react-i18next';

export default function LoginPage() {
  const { t } = useTranslation('auth');

  return (
    <div>
      <h1>{t('login.title')}</h1>
      <input placeholder={t('login.email')} />
      <input placeholder={t('login.password')} />
      <button>{t('login.signIn')}</button>
    </div>
  );
}
```

**Translation keys already exist!** Just replace the strings.

## 🌍 Language Switching

### In Code

```tsx
const { i18n } = useTranslation();

// Change language
i18n.changeLanguage('ar');  // Arabic
i18n.changeLanguage('en');  // English

// Get current language
console.log(i18n.language); // "en" or "ar"
```

### User Action

Click the Globe icon (🌐) in header → Select language

## ✅ Status Overview

| Item | Status |
|------|--------|
| **Dependencies** | ✅ Installed |
| **Configuration** | ✅ Complete |
| **Translation Files** | ✅ Created (150+ keys) |
| **Language Selector** | ✅ Working |
| **RTL Support** | ✅ Working |
| **Build** | ✅ Successful |
| **Dev Server** | ✅ Running |
| **Component Migration** | ⏳ 16% (2 of 12) |
| **Arabic Review** | ❌ Needed |

## 📚 Documentation

- **Full Setup Guide:** `I18N_SETUP.md`
- **Migration Checklist:** `I18N_MIGRATION_CHECKLIST.md`
- **Summary:** `I18N_SUMMARY.md`
- **This Card:** `I18N_QUICK_REFERENCE.md`

## 🔥 Most Common Issues & Solutions

### Issue: "Translation not showing"
✅ **Solution:** Make sure you added `'use client'` at the top of the file

### Issue: "Missing translation key"
✅ **Solution:** Check the JSON file has the key. Example: `t('dashboard.title')` needs `title` in `dashboard.json`

### Issue: "Wrong namespace"
✅ **Solution:** Use the correct namespace: `useTranslation('dashboard')` for dashboard.json

### Issue: "RTL not working"
✅ **Solution:** It's automatic! The `<html>` element gets `dir="rtl"` when Arabic is selected

## 🎨 Translation File Structure

```json
{
  "section": {
    "key": "English Text",
    "anotherKey": "More text"
  },
  "otherSection": {
    "title": "Title here"
  }
}
```

**Access:** `t('section.key')` → "English Text"

## 🌟 Pro Tips

1. **Test frequently** - Switch language after each change
2. **Use existing keys** - 150+ keys already created, reuse them
3. **Keep it simple** - Don't over-nest translation keys
4. **Check both languages** - Always test EN and AR
5. **RTL is automatic** - Don't manually add RTL CSS

## 🎯 Next Action

**Start with Login Page** - It's the quickest win and highest user impact!

```bash
File: /src/app/(auth)/login/page.tsx
Time: ~30 minutes
Keys: All ready in public/locales/*/auth.json
```

---

**🌐 Test Now:** http://localhost:3000

**📖 Full Docs:** See `I18N_SETUP.md`

**✅ Infrastructure:** 100% Complete

**⏳ Migration:** 16% Complete (8 pages remaining)
