# Onboarding Pages - Theme Unification Plan

## Goal
Unify all onboarding pages with the centralized landing theme system from `landing.css`.

## Current Status

### ✅ Completed
1. **GuidedWelcome.cshtml**: Removed inline `:root` CSS variable override
   - Removed: `--primary: #dc2626`, `--secondary: #10b981`, `--dark: #0f172a`, `--darker: #020617`, `--light: #f8fafc`
   - Added: Link to centralized `landing.css`

### ❌ Remaining Issues

#### 1. GuidedWelcome.cshtml
- Still uses Bootstrap utility classes (`bg-success`, `bg-primary`, etc.)
- No theme responsiveness (hardcoded colors won't change)
- Inconsistent with landing pages

#### 2. Other Onboarding Pages
- **Index.cshtml**: Uses `bg-success`, `text-primary`, `list-group-item-success`
- **OrgProfile.cshtml**: Uses `bg-light`, `bg-primary`, `bg-success`, `text-white`
- **ReviewScope.cshtml**: Uses `bg-success`, `progress-bar`
- **Signup.cshtml**: Uses `bg-success`
- **CreatePlan.cshtml**: Uses `bg-success`, `border-success`, `alert-info`
- **Activate.cshtml**: Uses inline `max-width: 400px`

## Solution Strategy

### Approach 1: Direct CSS Variable Replacement (Recommended)
Replace Bootstrap utility classes directly with CSS variables in inline styles or custom classes.

**Bootstrap Class → CSS Variable Mapping:**
| Bootstrap Class | CSS Variable | Usage |
|----------------|--------------|-------|
| `bg-success` | `var(--success)` | Success backgrounds |
| `text-primary` | `var(--primary)` | Primary text color |
| `bg-primary` | `var(--primary)` | Primary backgrounds |
| `bg-light` | `var(--bg-tertiary)` | Light backgrounds |
| `border-success` | `var(--success)` | Success borders |
| `text-white` | `var(--light)` | White text (adapts to theme) |
| `alert-info` | `var(--info)` | Info alerts |

### Approach 2: Theme-Aware Utility Classes (Optional)
Add theme-aware utility classes to `landing.css`:
```css
/* Theme-aware utility classes */
.bg-success { background-color: var(--success) !important; }
.text-primary { color: var(--primary) !important; }
.bg-primary { background-color: var(--primary) !important; }
.bg-light { background-color: var(--bg-tertiary) !important; }
.border-success { border-color: var(--success) !important; }
.text-white { color: var(--light) !important; }
```

**Pros:**
- Maintains Bootstrap class names
- Easier migration
- Can override Bootstrap defaults

**Cons:**
- Requires `!important` to override Bootstrap
- May conflict with Bootstrap utilities
- Less explicit about using theme system

## Implementation Steps

### Step 1: Ensure landing.css is Linked
All onboarding pages should include:
```html
<link rel="stylesheet" href="~/css/landing.css" asp-append-version="true" />
```

### Step 2: Replace Bootstrap Utility Classes
For each onboarding page:

1. **GuidedWelcome.cshtml** (High Priority)
   - Replace `bg-success` → `background: var(--success)`
   - Replace `bg-primary` → `background: var(--primary)`
   - Replace `text-primary` → `color: var(--primary)`
   - Replace `text-white` → `color: var(--light)`

2. **Index.cshtml**
   - Replace `bg-success` → `background: var(--success)`
   - Replace `text-primary` → `color: var(--primary)`
   - Replace `list-group-item-success` → `background: var(--success)`

3. **OrgProfile.cshtml**
   - Replace `bg-light` → `background: var(--bg-tertiary)`
   - Replace `bg-primary` → `background: var(--primary)`
   - Replace `bg-success` → `background: var(--success)`
   - Replace `text-white` → `color: var(--light)`

4. **ReviewScope.cshtml**
   - Replace `bg-success` → `background: var(--success)`
   - Replace `progress-bar` styles → use CSS variables

5. **Signup.cshtml**
   - Replace `bg-success` → `background: var(--success)`

6. **CreatePlan.cshtml**
   - Replace `bg-success` → `background: var(--success)`
   - Replace `border-success` → `border-color: var(--success)`
   - Replace `alert-info` → use CSS variables

7. **Activate.cshtml**
   - Replace inline `max-width: 400px` → CSS variable or class

### Step 3: Test Theme Switching
Verify that:
- Light/dark theme switching works on all pages
- Colors adapt correctly to theme
- No hardcoded colors remain
- Consistent appearance across all pages

## Files to Fix (Priority Order)

1. ✅ **GuidedWelcome.cshtml** (Partially done - needs Bootstrap class replacement)
2. **Index.cshtml**
3. **OrgProfile.cshtml**
4. **ReviewScope.cshtml**
5. **Signup.cshtml**
6. **CreatePlan.cshtml**
7. **Activate.cshtml**

## Benefits of Unification

✅ **Consistent Theme**: All pages use the same color palette  
✅ **Theme Responsive**: Colors adapt to light/dark mode  
✅ **Easy Maintenance**: Single source of truth for colors  
✅ **Better UX**: Consistent visual experience  
✅ **Accessibility**: Better contrast and WCAG compliance  

## Next Steps

1. **Choose approach**: Direct replacement (recommended) or utility classes
2. **Fix files systematically**: Start with GuidedWelcome.cshtml
3. **Test each page**: Verify theme switching works
4. **Document changes**: Update this plan as work progresses
