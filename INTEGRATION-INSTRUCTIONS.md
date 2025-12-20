# Product Module Integration Instructions

## ✅ Completed Integration Files

All integration files have been created:

1. **DbContext** - `src/Grc.EntityFrameworkCore/GrcDbContext.cs`
   - Added all Product module DbSets
   - Configured OnModelCreating to apply configurations

2. **Entity Configurations Extension** - `src/Grc.EntityFrameworkCore/Extensions/ModelBuilderExtensions.cs`
   - Extension methods to configure Product and Subscription entities

3. **Module Files** (6 files):
   - `src/Grc.Product.Domain/GrcProductDomainModule.cs`
   - `src/Grc.Product.Application.Contracts/GrcProductApplicationContractsModule.cs`
   - `src/Grc.Product.Application/GrcProductApplicationModule.cs`
   - `src/Grc.Product.EntityFrameworkCore/GrcProductEntityFrameworkCoreModule.cs`
   - `src/Grc.Product.HttpApi/GrcProductHttpApiModule.cs`

4. **AutoMapper Profile** - `src/Grc.Product.Application/GrcProductApplicationAutoMapperProfile.cs`
   - Configured entity to DTO mappings

## 🔧 Next Steps to Complete Integration

### Step 1: Update Main Application/Host Module

In your main application module (typically `GrcHttpApiHostModule.cs` or similar), add dependencies:

```csharp
[DependsOn(
    // ... existing dependencies ...
    typeof(GrcProductHttpApiModule),
    typeof(GrcProductEntityFrameworkCoreModule)
)]
public class GrcHttpApiHostModule : AbpModule
{
    // ...
}
```

### Step 2: Create EF Core Migration

Navigate to the solution root and run:

```bash
cd src/Grc.EntityFrameworkCore
dotnet ef migrations add AddProductModule --startup-project ../Grc.HttpApi.Host
```

Or if using a different startup project:

```bash
dotnet ef migrations add AddProductModule --startup-project <YourHostProject>
```

### Step 3: Update Database

```bash
dotnet ef database update --startup-project <YourHostProject>
```

### Step 4: Verify Seed Data

The `ProductSeedData` class will automatically seed data when the database is initialized. It seeds:
- **Trial** product (14-day trial, limited features)
- **Standard** product (25 users, 5 assessments, 10GB storage)
- **Professional** product (100 users, 50 assessments, 100GB storage)
- **Enterprise** product (unlimited quotas)

### Step 5: Register Data Seed Contributor (if needed)

In your module configuration, ensure data seeding is enabled:

```csharp
public override void OnApplicationInitialization(ApplicationInitializationContext context)
{
    // Data seeding is typically handled automatically by ABP
    // But you may need to ensure it's configured
}
```

### Step 6: Test API Endpoints

After starting the application, test the endpoints:

- `GET /api/grc/products` - List products
- `GET /api/grc/products/{id}` - Get product details
- `POST /api/grc/subscriptions/subscribe` - Subscribe to a product
- `GET /api/grc/subscriptions/current` - Get current subscription
- `GET /api/grc/subscriptions/quota-usage` - Get quota usage

## ⚠️ Important Notes

1. **Permissions**: You may need to define permissions in your permission definition provider:
   ```csharp
   public static class GrcPermissions
   {
       public const string GroupName = "Grc";
       
       public static class Products
       {
           public const string Default = GroupName + ".Products";
           public const string Create = Default + ".Create";
           public const string Update = Default + ".Update";
           public const string Delete = Default + ".Delete";
       }
   }
   ```

2. **Connection String**: Ensure your `appsettings.json` has the correct database connection string:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Host=localhost;Database=GrcDb;Username=postgres;Password=..."
     }
   }
   ```

3. **Multi-tenancy**: The Product module supports multi-tenancy. Ensure multi-tenancy is properly configured in your application.

4. **LocalizedString**: Ensure the `LocalizedString` value object is properly configured in your shared domain.

## 📋 Verification Checklist

- [ ] DbContext compiled successfully
- [ ] Module dependencies added to host module
- [ ] Migration created successfully
- [ ] Database updated successfully
- [ ] Seed data populated (check Products table)
- [ ] API endpoints accessible
- [ ] Swagger UI shows Product endpoints
- [ ] Can list products via API
- [ ] Can subscribe to a product (with tenant context)


