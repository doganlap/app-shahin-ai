# Grc.Web - MVC/Razor Pages Application

## Overview

This is the ABP.io MVC/Razor Pages web application for the Saudi GRC Compliance Platform.

## Features

- ✅ **ABP Modules Integrated**:
  - Account Management (Login/Register/Profile)
  - Identity Management (Users/Roles)
  - Tenant Management
  - Settings Management

- ✅ **Custom GRC Pages**:
  - Dashboard with overview cards
  - Assessments management
  - Subscriptions management
  - Evidence management (placeholder)
  - Framework Library (placeholder)

- ✅ **Localization**:
  - English (en)
  - Arabic (ar) with RTL support

- ✅ **Theme**:
  - LeptonXLite (Free ABP Theme)
  - Bootstrap 5
  - Responsive design

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 16+
- Redis (optional, for distributed caching)

## How to Run

### 1. Update Connection String

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=GrcDb;User Id=postgres;Password=YOUR_PASSWORD;"
  }
}
```

### 2. Run Database Migrations

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.DbMigrator
dotnet run
```

### 3. Run the Application

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web
dotnet run
```

### 4. Access the Application

- Web UI: `https://localhost:44303`
- Swagger API: `https://localhost:44303/swagger`

## Default Credentials

Use the credentials created by DbMigrator:
- Username: `admin`
- Password: `1q2w3E*`

## Project Structure

```
Grc.Web/
├── Menus/
│   ├── GrcMenus.cs                   # Menu constants
│   └── GrcMenuContributor.cs         # Navigation menu configuration
├── Pages/
│   ├── Dashboard/
│   │   ├── Index.cshtml              # Dashboard page
│   │   └── Index.cshtml.cs
│   ├── Assessments/
│   │   ├── Index.cshtml              # Assessments list
│   │   └── Index.cshtml.cs
│   ├── Subscriptions/
│   │   ├── Index.cshtml              # Subscriptions page
│   │   └── Index.cshtml.cs
│   ├── _ViewImports.cshtml           # Global imports
│   ├── _ViewStart.cshtml             # Global layout
│   ├── Index.cshtml                  # Home page
│   ├── Index.cshtml.cs
│   └── GrcPageModel.cs               # Base page model
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── global-styles.css
├── appsettings.json
├── appsettings.Development.json
├── GrcWebModule.cs                   # ABP module configuration
├── Program.cs                        # Application entry point
└── Grc.Web.csproj                    # Project file
```

## Available Pages

| Page | URL | Description |
|------|-----|-------------|
| Home | `/` | Welcome page |
| Dashboard | `/Dashboard` | Overview dashboard |
| Assessments | `/Assessments` | Assessment list |
| Subscriptions | `/Subscriptions` | Subscription management |
| Login | `/Account/Login` | User login (ABP) |
| Register | `/Account/Register` | User registration (ABP) |
| Users | `/Identity/Users` | User management (ABP) |
| Roles | `/Identity/Roles` | Role management (ABP) |
| Tenants | `/TenantManagement/Tenants` | Tenant management (ABP) |
| Settings | `/SettingManagement` | Application settings (ABP) |

## Localization

The application supports English and Arabic:

- Switch language using the language selector in the top menu
- Arabic includes RTL (Right-to-Left) support
- Translations are in `/root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Domain.Shared/Localization/Grc/`

## Next Steps

1. **Connect to Real Services**:
   - Update Dashboard page to use actual assessment services
   - Connect Assessments page to `IAssessmentAppService`
   - Connect Subscriptions page to subscription services

2. **Add More Pages**:
   - Assessment Detail page
   - Evidence management
   - Framework library browser

3. **Authentication**:
   - Configure OpenIddict in backend
   - Test login/logout flow
   - Verify permissions

4. **Deploy to Production**:
   - Update `appsettings.Production.json`
   - Configure HTTPS certificates
   - Set up reverse proxy (nginx/IIS)

## Development

### Adding a New Page

1. Create folder in `Pages/` directory
2. Add `Index.cshtml` and `Index.cshtml.cs`
3. Inherit from `GrcPageModel`
4. Add route to `GrcMenuContributor.cs`
5. Add localization keys to `en.json` and `ar.json`

### Testing

```bash
# Build the project
dotnet build

# Run tests (if any)
dotnet test

# Check for errors
dotnet build --no-incremental
```

## Troubleshooting

### Issue: Port already in use
**Solution**: Change port in `Properties/launchSettings.json`

### Issue: Database connection error
**Solution**: Verify PostgreSQL is running and connection string is correct

### Issue: Localization not working
**Solution**: Ensure `Grc.Domain.Shared` project is referenced and localization files are embedded resources

### Issue: ABP modules not showing
**Solution**: Verify module dependencies in `GrcWebModule.cs`

## Resources

- [ABP Documentation](https://docs.abp.io)
- [ABP MVC UI](https://docs.abp.io/en/abp/latest/UI/AspNetCore/Overall)
- [LeptonX Theme](https://docs.abp.io/en/commercial/latest/themes/lepton-x)

## Support

For issues or questions, refer to the main project documentation.

