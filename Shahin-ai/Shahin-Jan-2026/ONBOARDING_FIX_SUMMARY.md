# Onboarding Pages - Direct CSS Variable Replacement Summary

## Status

### ✅ Completed
- **GuidedWelcome.cshtml**: Removed `:root` override, added landing.css link
  - Uses CSS variables already (var(--primary), var(--success), etc.)
  - No Bootstrap utility classes found

### 📋 In Progress  
- **Index.cshtml**: Needs Bootstrap class replacement
  - Found: `bg-success`, `text-primary`, `list-group-item-success`
  
### ⏳ Pending
- **OrgProfile.cshtml**: `bg-light`, `bg-primary`, `bg-success`, `text-white`
- **ReviewScope.cshtml**: `bg-success`, `progress-bar`
- **Signup.cshtml**: `bg-success`
- **CreatePlan.cshtml**: `bg-success`, `border-success`, `alert-info`
- **Activate.cshtml**: Inline `max-width: 400px`

## Replacement Strategy

### Bootstrap Class → CSS Variable Mapping

1. **`bg-success`** → `style="background: var(--success);"`
2. **`text-primary`** → `style="color: var(--primary);"`
3. **`bg-primary`** → `style="background: var(--primary);"`
4. **`bg-light`** → `style="background: var(--bg-tertiary);"`
5. **`text-white`** → `style="color: var(--light);"`
6. **`border-success`** → `style="border-color: var(--success);"`
7. **`list-group-item-success`** → `style="background: var(--success);"`

### Implementation Approach

For each file:
1. Read the file
2. Find all Bootstrap utility classes in `class` attributes
3. For each occurrence:
   - Remove Bootstrap class from `class` attribute
   - Add corresponding inline style with CSS variable
   - If element already has a `style` attribute, merge styles
4. Ensure landing.css is linked (check layout or add to page)

## Next Steps

1. Fix Index.cshtml (highest priority)
2. Fix OrgProfile.cshtml
3. Fix remaining pages systematically
4. Test theme switching on all pages
5. Verify no Bootstrap utility classes remain

