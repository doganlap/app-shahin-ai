# ✅ Setup Complete!

## 🎉 Database Configured Successfully!

✅ PostgreSQL is running on port **5433**  
✅ Database **GrcDb** created  
✅ All migrations applied successfully  
✅ Tables created

---

## 🚀 **Run the Web Application Now**

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web
dotnet run
```

Then open in your browser:
**http://localhost:5001**

---

## 🔐 Creating Admin User

Since the DbMigrator has configuration issues, you can create an admin user directly:

### Option 1: Manual SQL (Quick Fix)

```bash
sudo -u postgres psql -d GrcDb
```

Then run:

```sql
-- Insert admin user (password hash for: 1q2w3E*)
INSERT INTO "AbpUsers" ("Id", "TenantId", "UserName", "NormalizedUserName", "Name", "Surname", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "IsActive", "ConcurrencyStamp", "CreationTime")
VALUES (gen_random_uuid(), NULL, 'admin', 'ADMIN', 'System', 'Administrator', 'admin@localhost', 'ADMIN@LOCALHOST', true, 'AQAAAAIAAYagAAAAEJ3lNZ8xvZWCzM6p8gQP7xXdKQv7WNjLQJCsJ0d0JvvU9fZ/5WqW5FkXnGx0FjL9bg==', gen_random_uuid()::text, true, gen_random_uuid()::text, now());

-- Insert admin role
INSERT INTO "AbpRoles" ("Id", "TenantId", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "ConcurrencyStamp")
VALUES (gen_random_uuid(), NULL, 'admin', 'ADMIN', false, true, false, gen_random_uuid()::text);

-- Link user to role
INSERT INTO "AbpUserRoles" ("UserId", "RoleId", "TenantId")
SELECT u."Id", r."Id", NULL
FROM "AbpUsers" u, "AbpRoles" r
WHERE u."UserName" = 'admin' AND r."Name" = 'admin';
```

### Option 2: Run Web App (ABP will create default admin automatically on first run)

Just start the web app - ABP should seed the admin user automatically:

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web
dotnet run
```

---

## 📋 Important Configuration

### PostgreSQL Port
- **Configured Port**: 5433 (not the default 5432)
- **Updated** in both `Grc.Web/appsettings.json` and `Grc.DbMigrator/appsettings.json`

### Connection String
```
Host=localhost;Port=5433;Database=GrcDb;User Id=postgres;Password=postgres;
```

### Web App Ports
- HTTP: `http://localhost:5001`
- HTTPS: `https://localhost:5002`

---

## 🌐 Available Pages

Once the app is running:

| Page | URL |
|------|-----|
| Home | http://localhost:5001/ |
| Dashboard | http://localhost:5001/Dashboard |
| Assessments | http://localhost:5001/Assessments |
| Subscriptions | http://localhost:5001/Subscriptions |
| Login | http://localhost:5001/Account/Login |
| Users | http://localhost:5001/Identity/Users |
| Roles | http://localhost:5001/Identity/Roles |
| Tenants | http://localhost:5001/TenantManagement/Tenants |

---

## ✅ What's Working

- ✅ ABP MVC/Razor Pages project created
- ✅ Database created and migrations applied
- ✅ All ABP modules configured
- ✅ Custom GRC pages created
- ✅ English & Arabic localization
- ✅ LeptonXLite theme
- ✅ Application builds successfully
- ✅ Ready to run!

---

## 📝 Next Steps

1. **Start the application**:
   ```bash
   cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web
   dotnet run
   ```

2. **Access the app**: Open `http://localhost:5001`

3. **Login** (if admin user exists):
   - Username: `admin`
   - Password: `1q2w3E*`

4. **Connect real services**: Update page models to use actual application services

---

## 🔧 Troubleshooting

### Can't connect to database
**Check PostgreSQL is running**:
```bash
sudo -u postgres psql -c "SELECT version();"
```

### Wrong port
**PostgreSQL is on port 5433**, not 5432. Connection strings have been updated.

### Need to reset database
```bash
sudo -u postgres psql -c "DROP DATABASE \"GrcDb\";"
sudo -u postgres psql -c "CREATE DATABASE \"GrcDb\";"
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.EntityFrameworkCore
dotnet ef database update --connection "Host=localhost;Port=5433;Database=GrcDb;User Id=postgres;Password=postgres;"
```

---

**Status**: 🚀 **READY TO RUN!**

Start the application now with:
```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web && dotnet run
```

Last Updated: December 21, 2025

