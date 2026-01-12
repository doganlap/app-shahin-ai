# ABP MVC/Razor Pages UI Implementation Plan

## Executive Summary

**Objective**: Add ABP MVC/Razor Pages UI to existing ASP.NET Core backend

**Current State**:
- ✅ Backend: ASP.NET Core 8.0 + ABP 8.3.0 (Complete)
- ✅ Database: PostgreSQL with all 16 modules
- ✅ API: RESTful API with Swagger
- ✅ Theme: LeptonXLite (FREE) already installed
- ❌ UI: No MVC pages (only API)

**Target State**:
- ✅ Full MVC/Razor Pages UI
- ✅ ABP modules: Account, Identity, Tenant, Settings
- ✅ Custom GRC pages: Dashboard, Assessments, Subscriptions, etc.
- ✅ Server-side rendering
- ✅ Arabic/English localization

---

## Architecture Decision

### Option Chosen: **Integrated MVC Application**

```
┌─────────────────────────────────────────────────┐
│         Grc.Web (NEW MVC Project)               │
│                                                 │
│  ┌──────────────┐      ┌──────────────────┐   │
│  │ ABP Modules  │      │ Custom GRC Pages │   │
│  │              │      │                  │   │
│  │ • Account    │      │ • Dashboard      │   │
│  │ • Identity   │      │ • Assessments    │   │
│  │ • Tenant Mgmt│      │ • Subscriptions  │   │
│  │ • Settings   │      │ • Evidence       │   │
│  └──────────────┘      └──────────────────┘   │
│                                                 │
│         Directly calls Application Layer        │
└──────────────────────┬──────────────────────────┘
                       │
                       ↓
┌─────────────────────────────────────────────────┐
│         Application Layer (Existing)            │
│  Grc.Application, Product.Application, etc.     │
└─────────────────────────────────────────────────┘
```

**Benefits**:
- No network latency (direct method calls)
- Simpler deployment (single application)
- Better performance
- No CORS issues
- Easier development

---

## Implementation Steps

### Phase 1: Create MVC Web Project (Week 1)

#### Step 1.1: Create Grc.Web Project

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src
dotnet new web -n Grc.Web
```

**Files to create**:
- `Grc.Web.csproj` - Project file with ABP dependencies
- `GrcWebModule.cs` - ABP module configuration
- `Program.cs` - Application entry point
- `appsettings.json` - Configuration

#### Step 1.2: Add NuGet Packages

```xml
<PackageReference Include="Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite" Version="3.2.0" />
<PackageReference Include="Volo.Abp.Account.Web.OpenIddict" Version="8.3.0" />
<PackageReference Include="Volo.Abp.Identity.Web" Version="8.3.0" />
<PackageReference Include="Volo.Abp.TenantManagement.Web" Version="8.3.0" />
<PackageReference Include="Volo.Abp.SettingManagement.Web" Version="8.3.0" />
<PackageReference Include="Volo.Abp.AspNetCore.Authentication.OpenIdConnect" Version="8.3.0" />
```

#### Step 1.3: Add Project References

```xml
<ProjectReference Include="..\Grc.Application\Grc.Application.csproj" />
<ProjectReference Include="..\Grc.EntityFrameworkCore\Grc.EntityFrameworkCore.csproj" />
<ProjectReference Include="..\Grc.HttpApi\Grc.HttpApi.csproj" />
```

---

### Phase 2: Configure ABP Modules (Week 1)

#### Step 2.1: Create GrcWebModule.cs

```csharp
[DependsOn(
    typeof(GrcApplicationModule),
    typeof(GrcEntityFrameworkCoreModule),
    typeof(GrcHttpApiModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpAutofacModule)
)]
public class GrcWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        
        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureMultiTenancy();
        ConfigureVirtualFileSystem(context);
        ConfigureNavigationMenu();
        ConfigureRouter(context);
        ConfigureAutoMapper();
    }
}
```

#### Step 2.2: Configure Authentication

**Cookie-based authentication** for MVC (not JWT):

```csharp
private void ConfigureAuthentication(ServiceConfigurationContext context)
{
    context.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies", options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(365);
    })
    .AddAbpOpenIdConnect("oidc", options =>
    {
        options.Authority = configuration["AuthServer:Authority"];
        options.RequireHttpsMetadata = false;
        options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
        
        options.ClientId = configuration["AuthServer:ClientId"];
        options.ClientSecret = configuration["AuthServer:ClientSecret"];
        
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        
        options.Scope.Add("email");
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        options.Scope.Add("Grc");
    });
}
```

---

### Phase 3: Configure Navigation Menu (Week 1)

#### Step 3.1: Create Menu Contributor

File: `Menus/GrcMenuContributor.cs`

```csharp
public class GrcMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<GrcResource>();

        // Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home"
            )
        );

        // Dashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Dashboard,
                l["Menu:Dashboard"],
                "~/Dashboard",
                icon: "fas fa-chart-line",
                requiredPermissionName: GrcPermissions.Dashboard.View
            )
        );

        // Assessments
        context.Menu.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Assessments,
                l["Menu:Assessments"],
                icon: "fas fa-tasks"
            ).AddItem(
                new ApplicationMenuItem(
                    GrcMenus.AssessmentsList,
                    l["Menu:Assessments:List"],
                    "~/Assessments",
                    requiredPermissionName: GrcPermissions.Assessments.View
                )
            )
        );

        // Subscriptions
        context.Menu.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Subscriptions,
                l["Menu:Subscriptions"],
                "~/Subscriptions",
                icon: "fas fa-credit-card"
            )
        );

        // Administration (ABP Modules)
        var administration = context.Menu.GetAdministration();
        
        // These are automatically added by ABP modules:
        // - Identity Management (Users, Roles)
        // - Tenant Management
        // - Settings
        
        return Task.CompletedTask;
    }
}
```

---

### Phase 4: Create Razor Pages (Weeks 2-3)

#### Page Structure

```
Grc.Web/
├── Pages/
│   ├── _ViewImports.cshtml
│   ├── _ViewStart.cshtml
│   ├── Index.cshtml                    // Home page
│   ├── Index.cshtml.cs
│   │
│   ├── Dashboard/
│   │   ├── Index.cshtml                // Dashboard
│   │   └── Index.cshtml.cs
│   │
│   ├── Assessments/
│   │   ├── Index.cshtml                // List assessments
│   │   ├── Index.cshtml.cs
│   │   ├── Create.cshtml               // Create assessment
│   │   ├── Create.cshtml.cs
│   │   ├── Edit.cshtml                 // Edit assessment
│   │   ├── Edit.cshtml.cs
│   │   └── Detail.cshtml               // Assessment details
│   │       └── Detail.cshtml.cs
│   │
│   ├── Subscriptions/
│   │   ├── Index.cshtml                // Subscription management
│   │   └── Index.cshtml.cs
│   │
│   ├── Evidence/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   │
│   └── FrameworkLibrary/
│       ├── Index.cshtml                // Browse frameworks
│       └── Index.cshtml.cs
│
├── wwwroot/
│   ├── css/
│   │   └── site.css                    // Custom styles
│   ├── js/
│   │   └── site.js                     // Custom scripts
│   └── images/
│
└── Localization/
    └── Grc/
        ├── ar.json                     // Arabic translations
        └── en.json                     // English translations
```

---

### Phase 5: Implement Dashboard Page (Week 2)

#### File: `Pages/Dashboard/Index.cshtml.cs`

```csharp
using System.Threading.Tasks;
using Grc.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Web.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : GrcPageModel
    {
        public DashboardOverviewDto Overview { get; set; }
        public List<MyControlDto> MyControls { get; set; }
        public List<FrameworkProgressDto> FrameworkProgress { get; set; }

        private readonly IDashboardAppService _dashboardAppService;

        public IndexModel(IDashboardAppService dashboardAppService)
        {
            _dashboardAppService = dashboardAppService;
        }

        public async Task OnGetAsync()
        {
            Overview = await _dashboardAppService.GetOverviewAsync();
            MyControls = await _dashboardAppService.GetMyControlsAsync();
            FrameworkProgress = await _dashboardAppService.GetFrameworkProgressAsync();
        }
    }
}
```

#### File: `Pages/Dashboard/Index.cshtml`

```html
@page
@model Grc.Web.Pages.Dashboard.IndexModel
@using Volo.Abp.AspNetCore.Mvc.UI.Layout

@{
    ViewBag.Title = L["Menu:Dashboard"];
    ViewBag.PageTitle = L["Dashboard"];
}

@section PageHeader {
    <div class="page-header">
        <h1>@L["Dashboard"]</h1>
        <p class="lead">@L["Dashboard:Description"]</p>
    </div>
}

<div class="row">
    <!-- Overview Cards -->
    <div class="col-md-3">
        <div class="card border-primary">
            <div class="card-body">
                <h5 class="card-title">@L["TotalAssessments"]</h5>
                <h2>@Model.Overview.TotalAssessments</h2>
            </div>
        </div>
    </div>
    
    <div class="col-md-3">
        <div class="card border-success">
            <div class="card-body">
                <h5 class="card-title">@L["CompletedAssessments"]</h5>
                <h2>@Model.Overview.CompletedAssessments</h2>
            </div>
        </div>
    </div>
    
    <div class="col-md-3">
        <div class="card border-warning">
            <div class="card-body">
                <h5 class="card-title">@L["PendingControls"]</h5>
                <h2>@Model.Overview.PendingControls</h2>
            </div>
        </div>
    </div>
    
    <div class="col-md-3">
        <div class="card border-danger">
            <div class="card-body">
                <h5 class="card-title">@L["OverdueControls"]</h5>
                <h2>@Model.Overview.OverdueControls</h2>
            </div>
        </div>
    </div>
</div>

<div class="row mt-4">
    <!-- My Controls -->
    <div class="col-md-6">
        <div class="card">
            <div class="card-header">
                <h4>@L["MyControls"]</h4>
            </div>
            <div class="card-body">
                <abp-table striped-rows="true" hoverable-rows="true">
                    <thead>
                        <tr>
                            <th>@L["ControlCode"]</th>
                            <th>@L["Status"]</th>
                            <th>@L["DueDate"]</th>
                            <th>@L["Actions"]</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var control in Model.MyControls)
                        {
                            <tr>
                                <td>@control.ControlCode</td>
                                <td>
                                    <span class="badge bg-@GetStatusBadgeClass(control.Status)">
                                        @control.Status
                                    </span>
                                </td>
                                <td>@control.DueDate.ToShortDateString()</td>
                                <td>
                                    <a href="/ControlAssessments/Detail/@control.Id" class="btn btn-sm btn-primary">
                                        @L["View"]
                                    </a>
                                </td>
                            </tr>
                        }
                    </tbody>
                </abp-table>
            </div>
        </div>
    </div>
    
    <!-- Framework Progress -->
    <div class="col-md-6">
        <div class="card">
            <div class="card-header">
                <h4>@L["FrameworkProgress"]</h4>
            </div>
            <div class="card-body">
                @foreach (var progress in Model.FrameworkProgress)
                {
                    <div class="mb-3">
                        <h6>@progress.FrameworkName</h6>
                        <div class="progress">
                            <div class="progress-bar" 
                                 role="progressbar" 
                                 style="width: @progress.CompletionPercentage%"
                                 aria-valuenow="@progress.CompletionPercentage" 
                                 aria-valuemin="0" 
                                 aria-valuemax="100">
                                @progress.CompletionPercentage%
                            </div>
                        </div>
                        <small class="text-muted">
                            @progress.CompletedControls / @progress.TotalControls @L["Controls"]
                        </small>
                    </div>
                }
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/Pages/Dashboard/Index.js"></script>
}
```

---

### Phase 6: Implement Assessments Pages (Week 2-3)

#### File: `Pages/Assessments/Index.cshtml.cs`

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Assessments;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;

namespace Grc.Web.Pages.Assessments
{
    [Authorize(GrcPermissions.Assessments.View)]
    public class IndexModel : GrcPageModel
    {
        public PagedResultDto<AssessmentDto> Assessments { get; set; }

        private readonly IAssessmentAppService _assessmentAppService;

        public IndexModel(IAssessmentAppService assessmentAppService)
        {
            _assessmentAppService = assessmentAppService;
        }

        public async Task OnGetAsync(int pageIndex = 1, int pageSize = 10)
        {
            Assessments = await _assessmentAppService.GetListAsync(
                new PagedAndSortedResultRequestDto 
                { 
                    SkipCount = (pageIndex - 1) * pageSize,
                    MaxResultCount = pageSize 
                }
            );
        }
    }
}
```

Similar pattern for:
- Create.cshtml / Create.cshtml.cs
- Edit.cshtml / Edit.cshtml.cs
- Detail.cshtml / Detail.cshtml.cs

---

### Phase 7: Implement Subscriptions Page (Week 3)

#### File: `Pages/Subscriptions/Index.cshtml.cs`

```csharp
using System.Threading.Tasks;
using Grc.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Web.Pages.Subscriptions
{
    [Authorize]
    public class IndexModel : GrcPageModel
    {
        public SubscriptionDetailDto CurrentSubscription { get; set; }
        public List<ProductDto> AvailableProducts { get; set; }

        private readonly ISubscriptionAppService _subscriptionAppService;
        private readonly IProductAppService _productAppService;

        public IndexModel(
            ISubscriptionAppService subscriptionAppService,
            IProductAppService productAppService)
        {
            _subscriptionAppService = subscriptionAppService;
            _productAppService = productAppService;
        }

        public async Task OnGetAsync()
        {
            CurrentSubscription = await _subscriptionAppService.GetCurrentAsync();
            AvailableProducts = await _productAppService.GetListAsync();
        }

        public async Task<IActionResult> OnPostCancelAsync(Guid id, string reason)
        {
            await _subscriptionAppService.CancelAsync(id, new CancelSubscriptionInput 
            { 
                Reason = reason 
            });
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpgradeAsync(Guid id, Guid newProductId)
        {
            await _subscriptionAppService.UpgradeAsync(id, new UpgradeSubscriptionInput
            {
                NewProductId = newProductId,
                EffectiveDate = DateTime.Now
            });
            
            return RedirectToPage();
        }
    }
}
```

---

### Phase 8: Localization (Week 3)

#### File: `Localization/Grc/en.json`

```json
{
  "culture": "en",
  "texts": {
    "Menu:Home": "Home",
    "Menu:Dashboard": "Dashboard",
    "Menu:Assessments": "Assessments",
    "Menu:Assessments:List": "All Assessments",
    "Menu:Subscriptions": "Subscriptions",
    "Dashboard": "Dashboard",
    "Dashboard:Description": "Overview of your GRC compliance status",
    "TotalAssessments": "Total Assessments",
    "CompletedAssessments": "Completed",
    "PendingControls": "Pending Controls",
    "OverdueControls": "Overdue Controls",
    "MyControls": "My Assigned Controls",
    "FrameworkProgress": "Framework Progress"
  }
}
```

#### File: `Localization/Grc/ar.json`

```json
{
  "culture": "ar",
  "texts": {
    "Menu:Home": "الرئيسية",
    "Menu:Dashboard": "لوحة التحكم",
    "Menu:Assessments": "التقييمات",
    "Menu:Assessments:List": "كل التقييمات",
    "Menu:Subscriptions": "الاشتراكات",
    "Dashboard": "لوحة التحكم",
    "Dashboard:Description": "نظرة عامة على حالة الامتثال",
    "TotalAssessments": "إجمالي التقييمات",
    "CompletedAssessments": "المكتملة",
    "PendingControls": "الضوابط المعلقة",
    "OverdueControls": "الضوابط المتأخرة",
    "MyControls": "الضوابط المخصصة لي",
    "FrameworkProgress": "تقدم الأطر التنظيمية"
  }
}
```

---

## Configuration Files

### appsettings.json

```json
{
  "App": {
    "SelfUrl": "https://localhost:44303",
    "CorsOrigins": "https://localhost:44303"
  },
  "AuthServer": {
    "Authority": "https://localhost:44303",
    "ClientId": "Grc_Web",
    "ClientSecret": "1q2w3e*",
    "RequireHttpsMetadata": "false"
  },
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=GrcDb;User Id=postgres;Password=your_password;"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  }
}
```

---

## Deployment

### Single Application Deployment

```
┌───────────────────────────────────┐
│         Grc.Web (Port 443)        │
│                                   │
│  ┌────────────┐  ┌─────────────┐ │
│  │  MVC UI    │  │  Web API    │ │
│  │  (Pages)   │  │  (Swagger)  │ │
│  └────────────┘  └─────────────┘ │
│                                   │
│      Shared Application Layer     │
└───────────────────────────────────┘
         │
         ↓
┌───────────────────────────────────┐
│       PostgreSQL Database         │
└───────────────────────────────────┘
```

**Benefits**:
- Single deployment unit
- One URL for everything
- Simpler infrastructure
- No API gateway needed

---

## Development Workflow

### 1. Run the Application

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web
dotnet run
```

### 2. Access Pages

- Home: `https://localhost:44303/`
- Login: `https://localhost:44303/Account/Login`
- Dashboard: `https://localhost:44303/Dashboard`
- Assessments: `https://localhost:44303/Assessments`
- Users: `https://localhost:44303/Identity/Users`
- Tenants: `https://localhost:44303/TenantManagement/Tenants`
- Settings: `https://localhost:44303/SettingManagement`
- API/Swagger: `https://localhost:44303/swagger`

---

## Testing Checklist

### ABP Modules

- [ ] Login with username/password
- [ ] Logout
- [ ] Register new user
- [ ] View user profile
- [ ] Change password
- [ ] Manage users (CRUD)
- [ ] Manage roles
- [ ] Assign permissions
- [ ] Manage tenants
- [ ] Switch tenant
- [ ] Configure email settings
- [ ] Configure timezone

### Custom GRC Pages

- [ ] View dashboard
- [ ] List assessments
- [ ] Create assessment
- [ ] View assessment details
- [ ] View subscription
- [ ] Upgrade subscription
- [ ] Cancel subscription
- [ ] Browse frameworks
- [ ] Upload evidence

### Localization

- [ ] Switch to Arabic
- [ ] Verify RTL layout
- [ ] Switch back to English
- [ ] Verify all translations

---

## Timeline

| Week | Tasks | Deliverables |
|------|-------|--------------|
| 1 | Create project, configure modules, navigation | Running MVC app with ABP modules |
| 2 | Dashboard, Assessments pages | Dashboard + Assessment CRUD |
| 3 | Subscriptions, Evidence, Localization | All custom pages + AR/EN |
| 4 | Testing, refinement, documentation | Production-ready application |

---

## Success Criteria

✅ All ABP modules working (Account, Identity, Tenant, Settings)
✅ Custom GRC pages functional
✅ Arabic/English localization
✅ Authentication/authorization working
✅ LeptonXLite theme applied
✅ No console errors
✅ Responsive design
✅ Production deployment successful

---

## Next Steps

1. **Create Grc.Web project**
2. **Configure ABP modules**
3. **Implement Dashboard page**
4. **Implement other pages progressively**
5. **Test thoroughly**
6. **Deploy to production**

---

**Questions or Clarifications Needed?**
- None - all requirements are clear
- Ready to proceed with implementation

