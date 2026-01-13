# i18n (Internationalization) Setup Guide

## Overview

This GRC application now has complete i18n infrastructure to support multiple languages (English and Arabic). The setup uses `react-i18next` and `i18next` for translation management.

## What's Been Set Up

### 1. Installed Dependencies
- `next-i18next` - Next.js integration for i18next
- `react-i18next` - React bindings for i18next
- `i18next` - Core i18n library

### 2. Configuration Files Created

#### `/next-i18next.config.js`
Main i18n configuration for Next.js routing and locale detection.

#### `/src/lib/i18n.ts`
Client-side i18n initialization and configuration. Imports all translation files and sets up the i18n instance.

#### `/next.config.ts`
Updated with i18n routing configuration:
- Supported locales: `en` (English), `ar` (Arabic)
- Default locale: `en`
- Automatic locale detection enabled

### 3. Translation Files Structure

```
public/locales/
├── en/                    # English translations
│   ├── common.json        # Shared UI elements (navigation, actions, status, etc.)
│   ├── auth.json          # Authentication page
│   ├── dashboard.json     # Dashboard page
│   ├── controls.json      # Controls page
│   ├── evidence.json      # Evidence page
│   ├── reports.json       # Reports page
│   └── remediation.json   # Remediation page
└── ar/                    # Arabic translations
    ├── common.json
    ├── auth.json
    ├── dashboard.json
    ├── controls.json
    ├── evidence.json
    ├── reports.json
    └── remediation.json
```

### 4. Components Created

#### `/src/components/LanguageSelector.tsx`
A language switcher dropdown component with:
- Globe icon from `lucide-react`
- Support for English and Arabic
- Persists language preference to localStorage
- Accessible dropdown UI

### 5. Updated Components

#### `/src/app/layout.tsx` (Root Layout)
- Added `I18nextProvider` wrapper
- Dynamic `dir` attribute (LTR/RTL) based on language
- Language change listener for real-time updates
- Updated metadata for SEO

#### `/src/components/layout/Header.tsx`
- Added `LanguageSelector` component
- Converted search placeholder to use translation
- Converted button text to use translation

## How to Use i18n in Your Components

### Basic Usage

```tsx
'use client';

import { useTranslation } from 'react-i18next';

export default function MyComponent() {
  // Specify which namespace to use (matches the JSON file name)
  const { t } = useTranslation('common');

  return (
    <div>
      <h1>{t('navigation.dashboard')}</h1>
      <p>{t('common.loading')}</p>
    </div>
  );
}
```

### Using Multiple Namespaces

```tsx
const { t: tCommon } = useTranslation('common');
const { t: tDashboard } = useTranslation('dashboard');

return (
  <div>
    <h1>{tDashboard('title')}</h1>
    <button>{tCommon('actions.export')}</button>
  </div>
);
```

### Translations with Variables

```tsx
// Translation file (dashboard.json):
// "controlsCompliant": "{{count}} of {{total}} controls compliant"

const { t } = useTranslation('dashboard');

<p>{t('complianceByFramework.controlsCompliant', { count: 42, total: 50 })}</p>
// Output: "42 of 50 controls compliant"
```

### Accessing Current Language

```tsx
import { useTranslation } from 'react-i18next';

export default function MyComponent() {
  const { i18n } = useTranslation();

  console.log(i18n.language); // "en" or "ar"

  return <div>Current language: {i18n.language}</div>;
}
```

### Changing Language Programmatically

```tsx
const { i18n } = useTranslation();

const switchToArabic = () => {
  i18n.changeLanguage('ar');
};

const switchToEnglish = () => {
  i18n.changeLanguage('en');
};
```

## Translation File Structure

### Example: `common.json`

```json
{
  "app": {
    "name": "Shahin AI",
    "platform": "GRC Platform"
  },
  "navigation": {
    "dashboard": "Dashboard",
    "controls": "Controls"
  },
  "actions": {
    "export": "Export",
    "add": "Add"
  },
  "status": {
    "compliant": "Compliant",
    "pending": "Pending Review"
  }
}
```

### Accessing Nested Keys

```tsx
t('app.name')           // "Shahin AI"
t('navigation.dashboard') // "Dashboard"
t('actions.export')     // "Export"
t('status.compliant')   // "Compliant"
```

## RTL (Right-to-Left) Support

The application automatically switches to RTL layout when Arabic is selected:

- The `<html>` element gets `dir="rtl"` attribute
- CSS automatically mirrors layout for RTL languages
- Language changes trigger immediate RTL/LTR switch

### Custom RTL Styles (if needed)

```css
/* In your CSS/Tailwind */
[dir="rtl"] .my-component {
  /* RTL-specific styles */
  margin-right: 0;
  margin-left: auto;
}
```

## Next Steps: Converting Existing Components

To convert an existing component to use i18n:

### Step 1: Add 'use client' directive (if not already present)

```tsx
'use client';
```

### Step 2: Import useTranslation hook

```tsx
import { useTranslation } from 'react-i18next';
```

### Step 3: Initialize translation in component

```tsx
const { t } = useTranslation('namespace-name');
```

### Step 4: Replace hardcoded strings

**Before:**
```tsx
<button>Export</button>
<h1>Dashboard</h1>
```

**After:**
```tsx
<button>{t('actions.export')}</button>
<h1>{t('navigation.dashboard')}</h1>
```

## Example: Converting a Page

**Before (`/src/app/(app)/dashboard/page.tsx`):**

```tsx
export default function Dashboard() {
  return (
    <div>
      <h1>Dashboard</h1>
      <button>Export</button>
    </div>
  );
}
```

**After:**

```tsx
'use client';

import { useTranslation } from 'react-i18next';

export default function Dashboard() {
  const { t } = useTranslation('dashboard');
  const { t: tCommon } = useTranslation('common');

  return (
    <div>
      <h1>{t('title')}</h1>
      <button>{tCommon('actions.export')}</button>
    </div>
  );
}
```

## Translation Keys Reference

### Common Namespace (`common.json`)

| Key | English | Arabic |
|-----|---------|--------|
| `app.name` | Shahin AI | شاهين الذكاء الاصطناعي |
| `navigation.dashboard` | Dashboard | لوحة التحكم |
| `navigation.controls` | Controls | الضوابط |
| `actions.export` | Export | تصدير |
| `actions.add` | Add | إضافة |
| `status.compliant` | Compliant | متوافق |
| `status.pending` | Pending Review | قيد المراجعة |

See individual JSON files for complete key listings.

## Arabic Translation Notes

⚠️ **IMPORTANT:** The Arabic translations provided are machine-generated placeholders. For production use:

1. **Professional Translation Required**: Have all Arabic text reviewed by a native Arabic speaker
2. **Cultural Appropriateness**: Ensure terminology is appropriate for your target region (Gulf Arabic, Egyptian, etc.)
3. **Technical Terms**: Some technical terms may need specific translations based on local standards
4. **Framework Names**: Some framework names (NCA ECC, SAMA CSF, etc.) are left untranslated as they are proper nouns

## Testing the Setup

### 1. Start the development server

```bash
npm run dev
```

### 2. Test language switching

- Open the application in your browser
- Click the language selector (Globe icon) in the header
- Switch between English and Arabic
- Verify that:
  - Text changes to the selected language
  - Layout switches to RTL for Arabic
  - Language preference persists on page reload

### 3. Test individual pages

Navigate to each page and verify translations are working:
- `/dashboard`
- `/controls`
- `/evidence`
- `/reports`
- `/remediation`
- `/login`

## Adding New Translations

### 1. Add to English translation file

```json
// public/locales/en/common.json
{
  "newFeature": {
    "title": "New Feature",
    "description": "This is a new feature"
  }
}
```

### 2. Add to Arabic translation file

```json
// public/locales/ar/common.json
{
  "newFeature": {
    "title": "ميزة جديدة",
    "description": "هذه ميزة جديدة"
  }
}
```

### 3. Use in component

```tsx
const { t } = useTranslation('common');

<div>
  <h2>{t('newFeature.title')}</h2>
  <p>{t('newFeature.description')}</p>
</div>
```

## Troubleshooting

### Translations not showing

1. Check that the component has `'use client'` directive
2. Verify the namespace name matches the JSON file name
3. Ensure the translation key path is correct
4. Check browser console for i18n errors

### Language not persisting

The language preference is stored in `localStorage`. Ensure:
- Browser allows localStorage
- No browser extensions are clearing storage
- Check browser console for errors

### RTL layout issues

If RTL layout isn't working:
- Check that `dir` attribute is present on `<html>` element
- Verify CSS doesn't have hardcoded directional properties (use logical properties instead)
- Use Tailwind's RTL-aware utilities when needed

## Best Practices

1. **Always use translation keys**: Never hardcode user-facing text
2. **Organize by feature**: Keep related translations in the same namespace
3. **Use descriptive keys**: `dashboard.stats.activePlans` is better than `dashboard.stat1`
4. **Keep translations flat**: Avoid deep nesting (max 3 levels)
5. **Handle plurals properly**: Use i18next's pluralization features
6. **Test both languages**: Always test UI in both English and Arabic
7. **Consider text expansion**: Arabic text may be 20-30% longer than English

## Performance Considerations

- Translation files are bundled with the application
- Lazy loading of translation namespaces is supported
- Language switching is instant (no page reload required)
- Translations are cached in memory

## Support and Resources

- [i18next Documentation](https://www.i18next.com/)
- [react-i18next Documentation](https://react.i18next.com/)
- [Next.js i18n Documentation](https://nextjs.org/docs/advanced-features/i18n-routing)

---

**Last Updated:** 2026-01-11
**Version:** 1.0.0
**Status:** Infrastructure Complete - Component Migration Pending
