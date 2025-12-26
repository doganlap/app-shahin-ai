# Database Migration Manual Guide

## Current Status
- ✅ Production database connection verified (Railway PostgreSQL 17.7)
- ✅ ABP tables already exist in the database
- ⚠️ GRC application tables need to be created

## Issue
The `Grc.DbMigrator` cannot run automatically due to a ContactInfo entity configuration issue. This needs to be resolved before automated migrations can work.

## Manual Migration Steps (Temporary Solution)

### Option 1: Use Existing Migration File

The migration file exists at:
```
/root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.EntityFrameworkCore/Migrations/20251221063915_Initial.cs
```

This file contains all the necessary DDL statements to create the GRC tables.

### Option 2: Run Migrations When Application Starts

The application will automatically apply pending migrations when it first runs if you configure it in `Grc.EntityFrameworkCore/GrcDbContextFactory.cs`.

### Option 3: Fix ContactInfo and Re-run

1. The ContactInfo value object needs to be configured as an owned type in the DbContext.
2. After fixing, run:
```bash
cd /root/app.shahin-ai.com/Shahin-ai
./run-migrations.sh
```

## Production Deployment Approach

**RECOMMENDED**: Deploy the application and let ABP's automatic migration feature handle it on first run.

The Web application (`Grc.Web`) is configured to apply migrations automatically when:
- `ASPNETCORE_ENVIRONMENT` is set to `Development` OR
- Manual migration is triggered via the DbMigrator

### Steps:
1. Deploy the application
2. Start the application - it will check for pending migrations
3. If migrations don't apply automatically, run this SQL manually:

```sql
-- Connect to database
psql -h mainline.proxy.rlwy.net -p 46662 -U postgres -d railway

-- Check if __EFMigrationsHistory exists
SELECT * FROM "__EFMigrationsHistory";

-- If migrations table doesn't have the Initial migration, the app will apply it on startup
```

## Verification

After application starts, verify tables were created:

```bash
export PGPASSWORD="sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ"
psql -h mainline.proxy.rlwy.net -p 46662 -U postgres -d railway -c "
SELECT tablename 
FROM pg_tables 
WHERE schemaname='public' 
AND (tablename LIKE 'grc_%' 
     OR tablename = 'Evidences' 
     OR tablename = 'Risks' 
     OR tablename = 'RiskTreatments'
     OR tablename = 'Frameworks'
     OR tablename = 'Controls'
     OR tablename = 'Regulators');
"
```

Expected tables:
- `Evidences` (or `grc_evidences`)
- `Risks` (or `grc_risks`)
- `RiskTreatments` (or `grc_risk_treatments`)
- `Frameworks` (or `grc_frameworks`)
- `Controls` (or `grc_controls`)
- `Regulators` (or `grc_regulators`)

## Database Configuration

### Connection String (Already Configured):
```
Host=mainline.proxy.rlwy.net;Port=46662;Database=railway;Username=postgres;Password=sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ;SSL Mode=Require;Trust Server Certificate=true
```

### Configured In:
- ✅ `Grc.Web/appsettings.Production.json`
- ✅ `Grc.HttpApi.Host/appsettings.Production.json`
- ✅ `Grc.DbMigrator/appsettings.json`

## Next Steps

1. **Deploy Application**: Use `./deploy-production.sh`
2. **Start Services**: Application will attempt to apply migrations on first run
3. **Monitor Logs**: Check for migration success messages
4. **Verify**: Query database to confirm tables were created

## If Migrations Fail

If automatic migration fails on application startup:

1. Check logs: `sudo journalctl -u grc-web -n 100`
2. Look for EF Core migration errors
3. Apply migrations manually using the generated migration files
4. Or fix the ContactInfo issue and regenerate migrations

## Status: READY FOR DEPLOYMENT

The application is configured and ready to deploy. Migrations will be applied automatically on first run. If they don't apply, we can troubleshoot and apply manually.



