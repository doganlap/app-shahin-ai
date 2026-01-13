# Bootstrap Integration Guide

This guide explains how to integrate the Unified Enterprise Design System with Bootstrap-based applications.

## Overview

The design system can work alongside Bootstrap or replace it entirely. This document covers both scenarios.

## Option 1: Using with Bootstrap (Recommended)

### Step 1: Install Bootstrap

```bash
npm install bootstrap
```

### Step 2: Create Custom Bootstrap Build

Create a custom SCSS file that imports design system tokens before Bootstrap:

**custom-bootstrap.scss**

```scss
// 1. Import design system variables
@import '../design-system/tokens/design-tokens.scss';

// 2. Import Bootstrap (variables will be overridden by design system)
@import 'bootstrap/scss/bootstrap';

// 3. Optionally import additional design system components
@import '../design-system/components/custom-components.scss';
```

### Step 3: Build Your SCSS

```bash
# Using node-sass
node-sass custom-bootstrap.scss -o dist/css/

# Or using sass
sass custom-bootstrap.scss dist/css/custom-bootstrap.css
```

### Step 4: Include in HTML

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>My App</title>

  <!-- Your custom Bootstrap build with design system tokens -->
  <link rel="stylesheet" href="dist/css/custom-bootstrap.css">
</head>
<body>
  <!-- Use Bootstrap components with design system styling -->
  <button class="btn btn-primary">Primary Button</button>
</body>
</html>
```

## Option 2: Standalone (Without Bootstrap)

If you're not using Bootstrap, simply include the design system CSS files:

```html
<link rel="stylesheet" href="design-system/tokens/design-tokens.css">
<link rel="stylesheet" href="design-system/components/components.css">
```

## ASP.NET MVC Integration

### Method 1: Bundle Configuration

In your `BundleConfig.cs`:

```csharp
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {
        // Design System CSS Bundle
        bundles.Add(new StyleBundle("~/Content/designsystem").Include(
            "~/Content/design-system/tokens/design-tokens.css",
            "~/Content/design-system/components/components.css"
        ));

        // Or if using custom Bootstrap build
        bundles.Add(new StyleBundle("~/Content/css").Include(
            "~/Content/dist/custom-bootstrap.css"
        ));
    }
}
```

In your `_Layout.cshtml`:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - My Application</title>

    @Styles.Render("~/Content/designsystem")
</head>
<body>
    @RenderBody()
</body>
</html>
```

### Method 2: Direct Link (Development)

```html
<link rel="stylesheet" href="~/Content/design-system/tokens/design-tokens.css">
<link rel="stylesheet" href="~/Content/design-system/components/components.css">
```

## Variable Mapping

The design system SCSS variables map to Bootstrap variables as follows:

### Colors

| Design System | Bootstrap | Value |
|---------------|-----------|-------|
| `$blue-500` | `$primary` | #0d6efd |
| `$green-500` | `$success` | #4caf50 |
| `$amber-500` | `$warning` | #ffc107 |
| `$red-500` | `$danger` | #dc3545 |
| `$teal-500` | `$info` | #17a2b8 |
| `$gray-900` | `$dark` | #333333 |
| `$gray-100` | `$light` | #f0f2f5 |

### Typography

| Design System | Bootstrap |
|---------------|-----------|
| `$font-family-sans-serif` | `$font-family-base` |
| `$font-size-base: 1rem` | `$font-size-base` |
| `$font-weight-medium` | `$btn-font-weight` |
| `$headings-font-weight: 600` | `$headings-font-weight` |

### Spacing

| Design System | Bootstrap |
|---------------|-----------|
| `$spacer: 1rem` | `$spacer` |
| `$spacers` map | `$spacers` |

### Components

| Design System | Bootstrap |
|---------------|-----------|
| `$btn-padding-y: 0.75rem` | `$btn-padding-y` |
| `$btn-border-radius: 0.375rem` | `$btn-border-radius` |
| `$input-padding-y: 0.75rem` | `$input-padding-y` |
| `$card-spacer-y: 1.5rem` | `$card-spacer-y` |

## Component Examples

### Using Bootstrap Components with Design System Styling

```html
<!-- Bootstrap Button with Design System Colors -->
<button type="button" class="btn btn-primary">Primary Action</button>
<button type="button" class="btn btn-success">Success</button>
<button type="button" class="btn btn-warning">Warning</button>
<button type="button" class="btn btn-danger">Danger</button>

<!-- Bootstrap Form with Design System Styling -->
<form>
  <div class="mb-3">
    <label for="email" class="form-label">Email address</label>
    <input type="email" class="form-control" id="email" placeholder="name@example.com">
    <div class="form-text">We'll never share your email with anyone else.</div>
  </div>
  <button type="submit" class="btn btn-primary">Submit</button>
</form>

<!-- Bootstrap Card with Design System Styling -->
<div class="card">
  <div class="card-header">
    Featured
  </div>
  <div class="card-body">
    <h5 class="card-title">Special title treatment</h5>
    <p class="card-text">With supporting text below.</p>
    <a href="#" class="btn btn-primary">Go somewhere</a>
  </div>
</div>

<!-- Bootstrap Alert with Design System Colors -->
<div class="alert alert-success" role="alert">
  A simple success alert with design system green!
</div>
```

## Customization

### Override Design System Variables

Create a `custom-variables.scss` file:

```scss
// Custom overrides
$blue-500: #0066cc; // Change primary color
$font-size-base: 18px; // Increase base font size
$border-radius: 0.5rem; // More rounded corners

// Import design system tokens
@import 'design-system/tokens/design-tokens.scss';

// Import Bootstrap
@import 'bootstrap/scss/bootstrap';
```

### Add Custom Components

```scss
// Import design system
@import 'design-system/tokens/design-tokens.scss';
@import 'bootstrap/scss/bootstrap';

// Add your custom components using design system tokens
.custom-widget {
  background-color: $primary;
  padding: $spacer;
  border-radius: $border-radius;
  box-shadow: $box-shadow;
}
```

## Build Process

### Using Webpack

**webpack.config.js**

```javascript
module.exports = {
  module: {
    rules: [
      {
        test: /\.scss$/,
        use: [
          'style-loader',
          'css-loader',
          {
            loader: 'sass-loader',
            options: {
              includePaths: [
                './design-system/tokens',
                './node_modules/bootstrap/scss'
              ]
            }
          }
        ]
      }
    ]
  }
};
```

### Using Gulp

**gulpfile.js**

```javascript
const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));

gulp.task('sass', function() {
  return gulp.src('./scss/custom-bootstrap.scss')
    .pipe(sass({
      includePaths: [
        './design-system/tokens',
        './node_modules/bootstrap/scss'
      ]
    }).on('error', sass.logError))
    .pipe(gulp.dest('./dist/css'));
});

gulp.task('watch', function() {
  gulp.watch('./scss/**/*.scss', gulp.series('sass'));
});
```

## NuGet Package Approach (Enterprise)

For large organizations with multiple ASP.NET MVC apps:

### 1. Create NuGet Package

Create a `DesignSystem.nuspec`:

```xml
<?xml version="1.0"?>
<package>
  <metadata>
    <id>CompanyName.DesignSystem</id>
    <version>1.0.0</version>
    <authors>Your Company</authors>
    <description>Unified Enterprise Design System</description>
    <dependencies>
      <dependency id="Bootstrap" version="5.3.0" />
    </dependencies>
  </metadata>
  <files>
    <file src="design-system\**\*" target="content\Content\design-system" />
    <file src="dist\**\*" target="content\Content\dist" />
  </files>
</package>
```

### 2. Build and Publish

```bash
nuget pack DesignSystem.nuspec
nuget push CompanyName.DesignSystem.1.0.0.nupkg -Source your-nuget-feed
```

### 3. Install in MVC Apps

```bash
Install-Package CompanyName.DesignSystem
```

## Migration Strategy

### Phase 1: Add Design System Alongside Existing Styles

```html
<!-- Keep existing Bootstrap -->
<link rel="stylesheet" href="~/Content/bootstrap.css">

<!-- Add design system -->
<link rel="stylesheet" href="~/Content/design-system/tokens/design-tokens.css">
<link rel="stylesheet" href="~/Content/design-system/components/components.css">
```

### Phase 2: Gradual Component Migration

Update components one at a time:

```html
<!-- Old Bootstrap button -->
<button class="btn btn-primary">Old Button</button>

<!-- New design system button -->
<button class="btn btn-primary">New Button</button>
<!-- Both look the same due to variable overrides! -->
```

### Phase 3: Switch to Custom Bootstrap Build

Replace Bootstrap with custom build:

```html
<!-- Remove old Bootstrap -->
<!-- <link rel="stylesheet" href="~/Content/bootstrap.css"> -->

<!-- Use custom build -->
<link rel="stylesheet" href="~/Content/dist/custom-bootstrap.css">
```

## Troubleshooting

### Issue: Styles Not Applied

**Solution**: Check CSS load order. Design system variables must load before Bootstrap.

```html
<!-- Correct order -->
<link rel="stylesheet" href="design-system/tokens/design-tokens.css">
<link rel="stylesheet" href="bootstrap.css">

<!-- Incorrect order -->
<link rel="stylesheet" href="bootstrap.css">
<link rel="stylesheet" href="design-system/tokens/design-tokens.css">
```

### Issue: Colors Don't Match

**Solution**: Ensure SCSS variables are imported before Bootstrap:

```scss
// Correct
@import 'design-system/tokens/design-tokens';
@import 'bootstrap';

// Incorrect
@import 'bootstrap';
@import 'design-system/tokens/design-tokens';
```

### Issue: Bundle Minification Breaks CSS Variables

**Solution**: Update `Web.config` to preserve CSS custom properties:

```xml
<system.webServer>
  <staticContent>
    <remove fileExtension=".css" />
    <mimeMap fileExtension=".css" mimeType="text/css" />
  </staticContent>
</system.webServer>
```

## Best Practices

1. **Single Source of Truth**: Import design system variables once at the root
2. **Version Control**: Pin design system version in package.json or packages.config
3. **Consistent Updates**: Update all apps when design system changes
4. **Component Audit**: Periodically review components for consistency
5. **Documentation**: Keep internal docs updated with examples

## Resources

- [Bootstrap Theming Documentation](https://getbootstrap.com/docs/5.3/customize/sass/)
- [SASS @import Documentation](https://sass-lang.com/documentation/at-rules/import)
- [CSS Custom Properties](https://developer.mozilla.org/en-US/docs/Web/CSS/--*)
- [ASP.NET Bundling](https://learn.microsoft.com/en-us/aspnet/mvc/overview/performance/bundling-and-minification)
