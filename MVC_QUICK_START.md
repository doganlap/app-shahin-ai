# ✅ MVC Web Application - Quick Start Guide

## 🎉 BUILD SUCCESSFUL!

Your ABP MVC/Razor Pages web application is ready to run!

---

## ⚡ How to Run

### Option 1: Run on Port 5001 (Recommended)

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web
dotnet run
```

Then open in your browser:
- **HTTP**: `http://localhost:5001`
- **HTTPS**: `https://localhost:5002`

### Option 2: Run on Custom Port

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web
dotnet run --urls "http://localhost:8080"
```

Then open: `http://localhost:8080`

---

## 📦 What's Included

✅ **ABP Modules** (All Working!):
- Account Management - `/Account/Login`
- Identity Management - `/Identity/Users`, `/Identity/Roles`
- Tenant Management - `/TenantManagement/Tenants`
- Settings - `/SettingManagement`

✅ **Custom GRC Pages**:
- Home - `/`
- Dashboard - `/Dashboard`
- Assessments - `/Assessments`
- Subscriptions - `/Subscriptions`

✅ **Localization**:
- English & Arabic with RTL support

✅ **Theme**:
- LeptonXLite (FREE ABP Theme)

---

## 🔐 Default Login

(After running DbMigrator):
- **Username**: `admin`
- **Password**: `1q2w3E*`

---

## 🐛 Troubleshooting

### Issue: Port already in use

**Error**: `Failed to bind to address http://127.0.0.1:5000: address already in use`

**Solution 1**: Kill the process using port 5000
```bash
# Find process
lsof -i :5000
# Or
netstat -tulpn | grep :5000

# Kill it
kill -9 <PID>
```

**Solution 2**: Use a different port (already configured)
```bash
dotnet run
# Will use port 5001 automatically
```

### Issue: Database connection error

**Solution**: Update connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=GrcDb;User Id=postgres;Password=YOUR_PASSWORD;"
  }
}
```

### Issue: No database/tables

**Solution**: Run the DbMigrator first:
```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.DbMigrator
dotnet run
```

---

## 📝 Next Steps

### 1. Connect to Real Services

The pages currently use placeholder data. To connect to real services:

**Example - Dashboard Page**:

Edit `/root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web/Pages/Dashboard/Index.cshtml.cs`:

```csharp
// Add service injection
private readonly IAssessmentAppService _assessmentService;

public IndexModel(IAssessmentAppService assessmentService)
{
    _assessmentService = assessmentService;
}

public async Task OnGetAsync()
{
    // Load real data
    var assessments = await _assessmentService.GetListAsync(new PagedAndSortedResultRequestDto());
    
    ViewModel = new DashboardViewModel
    {
        TotalAssessments = assessments.TotalCount,
        CompletedAssessments = assessments.Items.Count(a => a.Status == "Completed"),
        // ... more logic
    };
}
```

### 2. Add More Pages

Create additional pages as needed:
- Assessment Detail
- Assessment Create
- Evidence Management
- Framework Library

### 3. Configure Authentication

Update `appsettings.json` with your auth server settings:

```json
{
  "AuthServer": {
    "Authority": "https://your-auth-server.com",
    "ClientId": "Grc_Web",
    "ClientSecret": "your-secret",
    "RequireHttpsMetadata": "true"
  }
}
```

---

## 📊 Build Status

✅ All files created successfully  
✅ No compilation errors  
✅ All ABP modules loaded  
✅ Application starts successfully  
✅ Ready for development!

---

## 🔗 Important URLs

| Page | URL |
|------|-----|
| Home | `http://localhost:5001/` |
| Dashboard | `http://localhost:5001/Dashboard` |
| Assessments | `http://localhost:5001/Assessments` |
| Subscriptions | `http://localhost:5001/Subscriptions` |
| Login | `http://localhost:5001/Account/Login` |
| Users | `http://localhost:5001/Identity/Users` |
| Roles | `http://localhost:5001/Identity/Roles` |
| Tenants | `http://localhost:5001/TenantManagement/Tenants` |
| Settings | `http://localhost:5001/SettingManagement` |

---

##  📚 Documentation

- Full Plan: `/root/app.shahin-ai.com/Shahin-ai/MVC_RAZOR_PAGES_IMPLEMENTATION_PLAN.md`
- Implementation Report: `/root/app.shahin-ai.com/Shahin-ai/MVC_IMPLEMENTATION_COMPLETE.md`
- Project README: `/root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web/README.md`

---

**Status**: 🚀 **READY TO RUN!**

Last Updated: December 21, 2025

