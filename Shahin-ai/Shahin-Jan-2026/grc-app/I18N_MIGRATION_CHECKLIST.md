# i18n Migration Checklist

## Infrastructure Setup ✅

- [x] Install i18n dependencies
- [x] Create i18n configuration files
- [x] Create translation directory structure
- [x] Extract all English translations
- [x] Create Arabic translation files (placeholders)
- [x] Configure Next.js for i18n routing
- [x] Update root layout with i18n provider
- [x] Create language selector component
- [x] Add language selector to Header

## Components Migration Status

### Layout Components

| Component | Status | Priority | Notes |
|-----------|--------|----------|-------|
| `/src/app/layout.tsx` | ✅ Complete | Critical | Root layout with I18nextProvider |
| `/src/components/layout/Header.tsx` | ✅ Complete | Critical | Language selector added, translations active |
| `/src/components/layout/Sidebar.tsx` | ⏳ Pending | High | 16 menu items need translation |

### Page Components

| Page | Status | Priority | Hardcoded Strings | Notes |
|------|--------|----------|-------------------|-------|
| `/src/app/page.tsx` | ⏳ Pending | Low | 7 strings | Default Next.js template - consider replacing |
| `/src/app/(auth)/login/page.tsx` | ⏳ Pending | Critical | 20+ strings | User-facing auth page |
| `/src/app/(app)/dashboard/page.tsx` | ⏳ Pending | High | 30+ strings | Main landing page after login |
| `/src/app/(app)/controls/page.tsx` | ⏳ Pending | High | 40+ strings | Controls management |
| `/src/app/(app)/evidence/page.tsx` | ⏳ Pending | High | 20+ strings | Evidence management |
| `/src/app/(app)/reports/page.tsx` | ⏳ Pending | Medium | 15+ strings | Reports generation |
| `/src/app/(app)/remediation/page.tsx` | ⏳ Pending | Medium | 25+ strings | Remediation tracking |

### Shared Components

| Component | Status | Priority | Notes |
|-----------|--------|----------|-------|
| `/src/components/LanguageSelector.tsx` | ✅ Complete | Critical | Language switcher |
| Error Messages | ⏳ Pending | High | Scattered across components |
| Loading States | ⏳ Pending | Medium | "Loading..." messages |
| Empty States | ⏳ Pending | Medium | "No data" messages |

## Priority Migration Order

### Phase 1: Critical User-Facing Pages (Do First)

1. **Login Page** (`/src/app/(auth)/login/page.tsx`)
   - Impact: First user interaction
   - Complexity: Low
   - Estimated time: 30 minutes
   - Translation keys: All ready in `auth.json`

2. **Sidebar** (`/src/components/layout/Sidebar.tsx`)
   - Impact: Visible on every app page
   - Complexity: Medium
   - Estimated time: 45 minutes
   - Translation keys: All ready in `common.json`

3. **Dashboard** (`/src/app/(app)/dashboard/page.tsx`)
   - Impact: Main landing page
   - Complexity: Medium
   - Estimated time: 1 hour
   - Translation keys: All ready in `dashboard.json`

### Phase 2: Core Feature Pages

4. **Controls Page** (`/src/app/(app)/controls/page.tsx`)
   - Impact: Primary feature
   - Complexity: High (tables, filters, stats)
   - Estimated time: 1.5 hours
   - Translation keys: All ready in `controls.json`

5. **Evidence Page** (`/src/app/(app)/evidence/page.tsx`)
   - Impact: Primary feature
   - Complexity: Medium
   - Estimated time: 1 hour
   - Translation keys: All ready in `evidence.json`

### Phase 3: Secondary Pages

6. **Reports Page** (`/src/app/(app)/reports/page.tsx`)
   - Impact: Medium
   - Complexity: Medium
   - Estimated time: 1 hour
   - Translation keys: All ready in `reports.json`

7. **Remediation Page** (`/src/app/(app)/remediation/page.tsx`)
   - Impact: Medium
   - Complexity: High (Kanban, tables)
   - Estimated time: 1.5 hours
   - Translation keys: All ready in `remediation.json`

### Phase 4: Polish & Testing

8. **Landing Page** (`/src/app/page.tsx`)
   - Impact: Low (likely to be replaced)
   - Complexity: Low
   - Estimated time: 15 minutes

9. **Error & Loading States**
   - Impact: Medium
   - Complexity: Low
   - Estimated time: 30 minutes

10. **Testing & QA**
    - Test all pages in both languages
    - Verify RTL layout
    - Check responsiveness
    - Estimated time: 2 hours

## Total Estimated Migration Time

- **Phase 1 (Critical)**: ~2.25 hours
- **Phase 2 (Core)**: ~2.5 hours
- **Phase 3 (Secondary)**: ~2.5 hours
- **Phase 4 (Polish)**: ~2.75 hours
- **Total**: ~10 hours

## Migration Template

For each component, follow this pattern:

### 1. Add 'use client' directive

```tsx
'use client';
```

### 2. Import useTranslation

```tsx
import { useTranslation } from 'react-i18next';
```

### 3. Initialize translation hooks

```tsx
const { t } = useTranslation('namespace');
// Add more namespaces as needed
const { t: tCommon } = useTranslation('common');
```

### 4. Replace hardcoded strings

Replace:
```tsx
<h1>Dashboard</h1>
```

With:
```tsx
<h1>{t('title')}</h1>
```

## Testing Checklist

After migrating each component, verify:

- [ ] Component renders without errors
- [ ] All text is translated in English
- [ ] All text is translated in Arabic
- [ ] No hardcoded strings remain
- [ ] RTL layout works correctly (if applicable)
- [ ] Dynamic content (counts, dates) displays properly
- [ ] Language switching works without page reload
- [ ] No console errors related to missing keys

## Known Issues to Address During Migration

### 1. Status Value Enums

Components use status values like `"Compliant"`, `"Pending"` for both:
- UI display
- Filtering logic
- Styling classes

**Solution**: Keep enum values in English, translate only for display:

```tsx
// Instead of:
<Badge>{status}</Badge>

// Do:
<Badge>{t(`status.${status.toLowerCase()}`)}</Badge>
```

### 2. Framework Names

Names like "NCA ECC", "SAMA CSF" may be:
- Proper nouns (don't translate)
- Product names (don't translate)
- Or require localized versions

**Action Required**: Confirm with stakeholders which framework names should be translated.

### 3. Sample Data

Components have hardcoded sample data (activities, issues, etc.).

**Solution**: Move sample data to translation files or fetch from API.

### 4. Date Formatting

Dates need locale-aware formatting:

```tsx
// Add to i18n setup for proper date formatting
import { format } from 'date-fns';
import { enUS, ar } from 'date-fns/locale';

const locale = i18n.language === 'ar' ? ar : enUS;
format(date, 'PPP', { locale });
```

### 5. Number Formatting

Numbers should be formatted per locale:

```tsx
const number = 1234.56;
// Arabic: ١٬٢٣٤٫٥٦
// English: 1,234.56

const formatted = new Intl.NumberFormat(i18n.language).format(number);
```

## Arabic Translation Review Needed

The following items need professional Arabic translation review:

- [ ] All technical terminology
- [ ] Framework/standard names (if translatable)
- [ ] Error messages
- [ ] UI labels and descriptions
- [ ] Help text and tooltips

**Recommendation**: Engage a professional Arabic translator familiar with:
- GRC/compliance terminology
- Saudi Arabian dialect (if targeting Saudi market)
- Technical/software terminology

## RTL Styling Adjustments Needed

Components that may need RTL-specific CSS:

- [ ] Sidebar (menu alignment)
- [ ] Tables (column order)
- [ ] Cards (text alignment)
- [ ] Modals (close button position)
- [ ] Dropdowns (menu alignment)
- [ ] Forms (label/input alignment)

Use Tailwind's RTL utilities:
```tsx
className="mr-4 rtl:mr-0 rtl:ml-4"
```

Or use logical properties:
```tsx
className="ms-4" // margin-inline-start (RTL-aware)
```

## Performance Optimization

After migration:

- [ ] Consider code-splitting translation files
- [ ] Implement translation file lazy loading
- [ ] Add loading states during language switch
- [ ] Cache translations in localStorage
- [ ] Minimize translation file sizes

## Documentation Updates Needed

- [ ] Update README with i18n information
- [ ] Document translation key naming conventions
- [ ] Create contributor guide for adding new translations
- [ ] Add examples of common translation patterns
- [ ] Document testing procedures

## Next Actions

1. **Start with Phase 1**: Begin with Login page and Sidebar (highest impact)
2. **Test frequently**: Test language switching after each component
3. **Get feedback**: Show to stakeholders early for feedback
4. **Professional translation**: Arrange Arabic translation review
5. **Monitor performance**: Check bundle size and load times

---

**Progress**: 2/12 components complete (16%)

**Last Updated**: 2026-01-11

**Status**: Infrastructure complete, ready for component migration
