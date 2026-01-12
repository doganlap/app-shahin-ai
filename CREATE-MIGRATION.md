# Creating EF Core Migration for Product Module

## Prerequisites

1. Ensure you have .NET 8.0 SDK installed
2. EF Core tools installed: `dotnet tool install --global dotnet-ef`
3. PostgreSQL database server running
4. Connection string configured in `appsettings.json`

## Steps to Create Migration

### Option 1: Using Command Line (Recommended)

1. **Navigate to the solution root:**
   ```bash
   cd C:\Shahin-ai
   ```

2. **Navigate to EntityFrameworkCore project:**
   ```bash
   cd src\Grc.EntityFrameworkCore
   ```

3. **Create the migration:**
   ```bash
   dotnet ef migrations add AddProductModule --startup-project ..\Grc.HttpApi.Host
   ```
   
   Or if your startup project has a different name:
   ```bash
   dotnet ef migrations add AddProductModule --startup-project ..\<YourHostProject>
   ```

### Option 2: Using Package Manager Console (Visual Studio)

1. Open Package Manager Console
2. Set Default Project to `Grc.EntityFrameworkCore`
3. Run:
   ```powershell
   Add-Migration AddProductModule -StartupProject Grc.HttpApi.Host
   ```

## Expected Migration Files

After running the command, you should see a new migration file in:
- `src/Grc.EntityFrameworkCore/Migrations/YYYYMMDDHHMMSS_AddProductModule.cs`

The migration should include:
- CreateTable operations for:
  - `grc.products`
  - `grc.product_features`
  - `grc.product_quotas`
  - `grc.pricing_plans`
  - `grc.tenant_subscriptions`
  - `grc.quota_usages`
- CreateIndex operations for all indexes
- Foreign key constraints

## Apply Migration to Database

After creating the migration, apply it to the database:

```bash
dotnet ef database update --startup-project ..\Grc.HttpApi.Host
```

## Verify Migration

1. Check database tables were created:
   ```sql
   SELECT table_name 
   FROM information_schema.tables 
   WHERE table_schema = 'grc' 
   AND table_name LIKE 'product%' OR table_name LIKE '%subscription%' OR table_name LIKE 'quota%';
   ```

2. Verify seed data was populated:
   ```sql
   SELECT code, name_en, category FROM grc.products;
   ```
   
   Should return 4 products: Trial, Standard, Professional, Enterprise

## Troubleshooting

### Error: "No DbContext was found"

**Solution:** Ensure the startup project references the EntityFrameworkCore project and has the connection string configured.

### Error: "Package manager console error"

**Solution:** Use command line option instead, or ensure EF Core tools are installed.

### Error: "Connection string not found"

**Solution:** Verify `appsettings.json` or `appsettings.Development.json` has:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=GrcDb;Username=postgres;Password=yourpassword"
  }
}
```

### Migration creates wrong table names

**Solution:** Check that entity configurations specify the correct table names with schema (e.g., `grc.products`).


