# Unified Enterprise Design System

A modern, mobile-first design system tailored for enterprise web and mobile applications. This system standardizes styles across applications by defining a cohesive light-mode theme with a professional color palette, design tokens, and UI component guidelines.

## Features

- **Token-Based Architecture**: Three-tier token hierarchy (Global, Alias/Semantic, Component)
- **WCAG AA Compliant**: All color combinations meet accessibility standards
- **Mobile-First Responsive**: Seamless scaling from mobile to desktop
- **Professional Blue Palette**: Industry-standard color scheme inspired by IBM Carbon, Google Material, and Microsoft Fluent
- **CSS Custom Properties**: Easy theming and customization
- **Consistent Components**: Standardized buttons, forms, alerts, cards, modals, and navigation

## Getting Started

### Installation

Include the design system CSS files in your project:

```html
<!-- Include design tokens first -->
<link rel="stylesheet" href="design-system/tokens/design-tokens.css">

<!-- Then include component styles -->
<link rel="stylesheet" href="design-system/components/components.css">
```

### Basic Usage

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>My App</title>
  <link rel="stylesheet" href="design-system/tokens/design-tokens.css">
  <link rel="stylesheet" href="design-system/components/components.css">
</head>
<body>
  <button class="btn btn-primary">Primary Action</button>
</body>
</html>
```

## Design Tokens

### Color Palette

#### Primary Colors
- **Primary Blue**: `--color-primary` (#0d6efd) - Main brand and action color
- **Success Green**: `--color-success` (#4caf50) - Success states
- **Warning Amber**: `--color-warning` (#ffc107) - Warning states
- **Error Red**: `--color-error` (#dc3545) - Error states
- **Info Teal**: `--color-info` (#17a2b8) - Informational states

#### Neutral Colors
- **White**: `--global-white` (#ffffff) - Primary backgrounds
- **Light Grays**: `--global-gray-50` to `--global-gray-300` - Subtle shading
- **Dark Grays**: `--global-gray-800`, `--global-gray-900` - Text colors

### Typography

```css
/* Font Families */
--font-family-base: System font stack
--font-family-code: Monospace font stack

/* Font Sizes */
--font-size-base: 1rem (16px)
--font-size-small: 0.875rem (14px)
--font-size-large: 1.125rem (18px)
--font-size-heading-sm: 1.25rem (20px)
--font-size-heading-md: 1.5rem (24px)
--font-size-heading-lg: 2rem (32px)
--font-size-heading-xl: 2.5rem (40px)
```

### Spacing

Based on 8px spacing unit:

```css
--space-padding-xs: 4px
--space-padding-sm: 8px
--space-padding-md: 16px
--space-padding-lg: 24px
--space-padding-xl: 32px
```

## Components

### Buttons

```html
<!-- Primary button (main call-to-action) -->
<button class="btn btn-primary">Primary Action</button>

<!-- Secondary button (outline style) -->
<button class="btn btn-secondary">Secondary Action</button>

<!-- Danger button (destructive actions) -->
<button class="btn btn-danger">Delete</button>

<!-- Disabled button -->
<button class="btn btn-primary" disabled>Disabled</button>

<!-- Button sizes -->
<button class="btn btn-primary btn-sm">Small</button>
<button class="btn btn-primary btn-lg">Large</button>

<!-- Full width button -->
<button class="btn btn-primary btn-block">Full Width</button>
```

### Forms

```html
<div class="form-group">
  <label class="form-label" for="email">Email Address</label>
  <input type="email" id="email" class="form-input" placeholder="you@example.com">
  <span class="form-helper-text">We'll never share your email</span>
</div>

<div class="form-group">
  <label class="form-label" for="password">Password</label>
  <input type="password" id="password" class="form-input is-invalid">
  <span class="form-error-text">Password is required</span>
</div>

<div class="form-group">
  <label class="form-label" for="message">Message</label>
  <textarea id="message" class="form-textarea" placeholder="Your message"></textarea>
</div>

<div class="form-check">
  <input type="checkbox" id="agree" class="form-check-input">
  <label for="agree" class="form-check-label">I agree to terms</label>
</div>
```

### Alerts

```html
<!-- Success alert -->
<div class="alert alert-success">
  <span class="alert-icon">✓</span>
  <div class="alert-content">Your changes have been saved successfully!</div>
  <button class="alert-close">×</button>
</div>

<!-- Warning alert -->
<div class="alert alert-warning">
  <span class="alert-icon">⚠</span>
  <div class="alert-content">Please review your information before submitting.</div>
</div>

<!-- Error alert -->
<div class="alert alert-error">
  <span class="alert-icon">✕</span>
  <div class="alert-content">An error occurred. Please try again.</div>
</div>

<!-- Info alert -->
<div class="alert alert-info">
  <span class="alert-icon">ℹ</span>
  <div class="alert-content">New features are now available.</div>
</div>
```

### Cards

```html
<div class="card">
  <div class="card-header">
    <h3 class="card-title">Card Title</h3>
  </div>
  <div class="card-body">
    <p>Card content goes here. This is a flexible container for grouping related information.</p>
  </div>
  <div class="card-footer">
    <button class="btn btn-primary">Action</button>
    <button class="btn btn-secondary">Cancel</button>
  </div>
</div>

<!-- Compact card -->
<div class="card card-compact">
  <div class="card-body">
    <p>Compact card with reduced padding.</p>
  </div>
</div>
```

### Modals

```html
<div class="modal-backdrop">
  <div class="modal">
    <div class="modal-header">
      <h2 class="modal-title">Confirm Action</h2>
      <button class="modal-close">×</button>
    </div>
    <div class="modal-body">
      <p>Are you sure you want to proceed with this action?</p>
    </div>
    <div class="modal-footer">
      <button class="btn btn-secondary">Cancel</button>
      <button class="btn btn-primary">Confirm</button>
    </div>
  </div>
</div>

<!-- Modal sizes -->
<div class="modal modal-sm">...</div>  <!-- Small -->
<div class="modal modal-lg">...</div>  <!-- Large -->
```

### Navigation

```html
<nav class="navbar">
  <a href="#" class="navbar-brand">My App</a>
  <button class="navbar-toggle">☰</button>
  <ul class="navbar-nav">
    <li><a href="#" class="nav-link active">Home</a></li>
    <li><a href="#" class="nav-link">Products</a></li>
    <li><a href="#" class="nav-link">About</a></li>
    <li><a href="#" class="nav-link">Contact</a></li>
  </ul>
</nav>
```

## Responsive Breakpoints

```css
/* Extra Small (xs): 0px - 575px (default, mobile-first) */
/* Small (sm): 576px and up */
/* Medium (md): 768px and up */
/* Large (lg): 992px and up */
/* Extra Large (xl): 1200px and up */
/* Extra Extra Large (xxl): 1400px and up */
```

### Responsive Utilities

```html
<!-- Hide on mobile -->
<div class="d-none d-md-block">Visible on tablet and desktop only</div>

<!-- Hide on desktop -->
<div class="d-block d-md-none">Visible on mobile only</div>
```

## Utility Classes

### Display & Flexbox

```html
<div class="d-flex align-items-center justify-content-between">
  <span>Left</span>
  <span>Right</span>
</div>

<div class="d-flex flex-column gap-md">
  <div>Item 1</div>
  <div>Item 2</div>
</div>
```

### Spacing

```html
<div class="mt-md mb-lg p-md">
  Margin top medium, margin bottom large, padding medium
</div>
```

### Text & Colors

```html
<p class="text-center text-primary">Centered primary text</p>
<p class="text-success">Success message</p>
<p class="text-error">Error message</p>
```

### Borders & Shadows

```html
<div class="card border rounded shadow-md">
  Card with border, rounded corners, and shadow
</div>
```

## Accessibility

This design system follows WCAG AA guidelines:

- All color combinations meet 4.5:1 contrast ratio minimum
- Interactive elements have minimum 44px touch target size
- Focus states are clearly visible with 3px outline
- Semantic HTML and ARIA labels supported
- Form validation doesn't rely on color alone

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Customization

Override any design token to customize the theme:

```css
:root {
  /* Change primary color */
  --color-primary: #0066cc;

  /* Change base font size */
  --font-size-base: 18px;

  /* Adjust spacing scale */
  --space-padding-md: 20px;
}
```

## Integration with Bootstrap

This system can work alongside Bootstrap. Override Bootstrap variables:

```scss
// Import design tokens first
@import 'design-system/tokens/design-tokens';

// Override Bootstrap variables
$primary: var(--color-primary);
$success: var(--color-success);
$warning: var(--color-warning);
$danger: var(--color-error);
$info: var(--color-info);

// Import Bootstrap
@import 'bootstrap';
```

## Contributing

Follow these guidelines when contributing:

1. Maintain the three-tier token hierarchy
2. Use kebab-case naming: `[category]-[property]-[modifier]`
3. Ensure WCAG AA compliance
4. Test across all breakpoints
5. Document new components

## License

MIT License

## References

Inspired by:
- IBM Carbon Design System
- Google Material Design
- Microsoft Fluent Design System
