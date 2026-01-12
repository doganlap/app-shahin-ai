using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Grc.Localization;
using Grc.Permissions;
using Volo.Abp.Account.Localization;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;

namespace Grc.Web.Menus;

public class GrcMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public GrcMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<GrcResource>();

        // Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        // Dashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Dashboard,
                l["Menu:Dashboard"],
                "~/Dashboard",
                icon: "fas fa-chart-line",
                order: 1
            ).RequirePermissions(GrcPermissions.Dashboard.Default)
        );

        // MY WORKSPACE - Personal user space
        var myWorkspace = context.Menu.AddItem(
            new ApplicationMenuItem(
                "MyWorkspace",
                l["Menu:MyWorkspace"],
                icon: "fas fa-user-circle",
                order: 2
            )
        );

        myWorkspace.AddItem(
            new ApplicationMenuItem(
                "MyProfile",
                l["Menu:MyProfile"],
                "/Account/Manage",
                icon: "fas fa-user"
            )
        );

        myWorkspace.AddItem(
            new ApplicationMenuItem(
                "MyNotifications",
                l["Menu:MyNotifications"],
                "~/MyNotifications",
                icon: "fas fa-bell"
            ).RequirePermissions(GrcPermissions.MyWorkspace.ViewNotifications)
        );

        myWorkspace.AddItem(
            new ApplicationMenuItem(
                "MyTasks",
                l["Menu:MyTasks"],
                "~/MyTasks",
                icon: "fas fa-tasks"
            ).RequirePermissions(GrcPermissions.MyWorkspace.ViewTasks)
        );

        myWorkspace.AddItem(
            new ApplicationMenuItem(
                "MySettings",
                l["Menu:MySettings"],
                "~/MySettings",
                icon: "fas fa-cog"
            ).RequirePermissions(GrcPermissions.MyWorkspace.ManageSettings)
        );

        // CORE MODULES
        var coreModules = context.Menu.AddItem(
            new ApplicationMenuItem(
                "GrcCore",
                l["Menu:Core"],
                icon: "fas fa-th",
                order: 2
            )
        );

        coreModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.FrameworkLibrary,
                l["Menu:FrameworkLibrary"],
                "~/FrameworkLibrary",
                icon: "fas fa-book"
            ).RequirePermissions(GrcPermissions.Frameworks.Default)
        );

        coreModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Regulators,
                l["Menu:Regulators"],
                "~/Regulators",
                icon: "fas fa-landmark"
            ).RequirePermissions(GrcPermissions.Regulators.Default)
        );

        coreModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Assessments,
                l["Menu:Assessments"],
                "~/Assessments",
                icon: "fas fa-tasks"
            ).RequirePermissions(GrcPermissions.Assessments.Default)
        );

        coreModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.ControlAssessments,
                l["Menu:ControlAssessments"],
                "~/ControlAssessments",
                icon: "fas fa-list-check"
            ).RequirePermissions(GrcPermissions.ControlAssessments.Default)
        );

        coreModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Evidence,
                l["Menu:Evidence"],
                "~/Evidence",
                icon: "fas fa-folder-open"
            ).RequirePermissions(GrcPermissions.Evidence.Default)
        );

        // COMPLIANCE & RISK MODULES
        var complianceModules = context.Menu.AddItem(
            new ApplicationMenuItem(
                "GrcCompliance",
                l["Menu:Compliance"],
                icon: "fas fa-shield-alt",
                order: 3
            )
        );

        complianceModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Risks,
                l["Menu:RiskManagement"],
                "~/Risks",
                icon: "fas fa-exclamation-triangle"
            ).RequirePermissions(GrcPermissions.Risks.Default)
        );

        complianceModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Audits,
                l["Menu:AuditManagement"],
                "~/Audits",
                icon: "fas fa-clipboard-check"
            ).RequirePermissions(GrcPermissions.Audits.Default)
        );

        complianceModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.ActionPlans,
                l["Menu:ActionPlans"],
                "~/ActionPlans",
                icon: "fas fa-tasks"
            ).RequirePermissions(GrcPermissions.ActionPlans.Default)
        );

        complianceModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Policies,
                l["Menu:PolicyManagement"],
                "~/Policies",
                icon: "fas fa-file-contract"
            ).RequirePermissions(GrcPermissions.Policies.Default)
        );

        complianceModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Calendar,
                l["Menu:ComplianceCalendar"],
                "~/Calendar",
                icon: "fas fa-calendar-alt"
            ).RequirePermissions(GrcPermissions.Calendar.Default)
        );

        // OPERATIONS & GOVERNANCE
        var operationsModules = context.Menu.AddItem(
            new ApplicationMenuItem(
                "GrcOperations",
                l["Menu:Operations"],
                icon: "fas fa-cogs",
                order: 4
            )
        );

        operationsModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Workflows,
                l["Menu:WorkflowEngine"],
                "~/Workflows",
                icon: "fas fa-project-diagram"
            ).RequirePermissions(GrcPermissions.Workflows.Default)
        );

        operationsModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Notifications,
                l["Menu:Notifications"],
                "~/Notifications",
                icon: "fas fa-bell"
            ).RequirePermissions(GrcPermissions.Notifications.Default)
        );

        operationsModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Vendors,
                l["Menu:VendorManagement"],
                "~/Vendors",
                icon: "fas fa-building"
            ).RequirePermissions(GrcPermissions.Vendors.Default)
        );

        operationsModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Reports,
                l["Menu:ReportingAnalytics"],
                "~/Reports",
                icon: "fas fa-chart-bar"
            ).RequirePermissions(GrcPermissions.Reports.Default)
        );

        // ADVANCED
        var advancedModules = context.Menu.AddItem(
            new ApplicationMenuItem(
                "GrcAdvanced",
                l["Menu:Advanced"],
                icon: "fas fa-rocket",
                order: 5
            )
        );

        advancedModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Integrations,
                l["Menu:IntegrationHub"],
                "~/Integrations",
                icon: "fas fa-plug"
            ).RequirePermissions(GrcPermissions.Integrations.Default)
        );

        advancedModules.AddItem(
            new ApplicationMenuItem(
                GrcMenus.AI,
                l["Menu:AIEngine"],
                "~/AI",
                icon: "fas fa-robot"
            ).RequirePermissions(GrcPermissions.AI.Default)
        );

        // Subscriptions
        context.Menu.AddItem(
            new ApplicationMenuItem(
                GrcMenus.Subscriptions,
                l["Menu:Subscriptions"],
                "~/Subscriptions",
                icon: "fas fa-credit-card",
                order: 6
            ).RequirePermissions(GrcPermissions.Subscriptions.Default)
        );

        // Administration (ABP modules will add their items here automatically)
        var administration = context.Menu.GetAdministration();
        administration.Order = 100;

        // Add Admin menu item for seeding data
        administration.AddItem(
            new ApplicationMenuItem(
                "GrcAdmin",
                l["Permission:Admin"],
                "~/Admin/SeedData",
                icon: "fas fa-database"
            ).RequirePermissions(GrcPermissions.Admin.Default)
        );

        // Add API Viewer
        administration.AddItem(
            new ApplicationMenuItem(
                "ApiViewer",
                l["Menu:ApiViewer"],
                "~/ApiViewer",
                icon: "fas fa-code"
            ).RequirePermissions(GrcPermissions.Admin.ApiManagement)
        );

        // Add Organization Units Management
        administration.AddItem(
            new ApplicationMenuItem(
                "OrganizationUnits",
                l["Menu:OrganizationUnits"],
                "/Identity/OrganizationUnits",
                icon: "fas fa-sitemap",
                order: 3
            )
        );

        // SECURITY & COMPLIANCE SECTION
        var securityCompliance = administration.AddItem(
            new ApplicationMenuItem(
                "SecurityCompliance",
                l["Menu:SecurityCompliance"],
                icon: "fas fa-shield-alt",
                order: 10
            )
        );

        securityCompliance.AddItem(
            new ApplicationMenuItem(
                "AuditLogs",
                l["Menu:AuditLogs"],
                "/AuditLogging/AuditLogs",
                icon: "fas fa-history"
            )
        );

        securityCompliance.AddItem(
            new ApplicationMenuItem(
                "SecurityLogs",
                l["Menu:SecurityLogs"],
                "/Identity/SecurityLogs",
                icon: "fas fa-user-shield"
            )
        );

        // PERMISSIONS MANAGEMENT
        administration.AddItem(
            new ApplicationMenuItem(
                "Permissions",
                l["Menu:Permissions"],
                "~/Permissions",
                icon: "fas fa-lock",
                order: 4
            ).RequirePermissions(GrcPermissions.Admin.Default)
        );

        // FEATURE MANAGEMENT (Critical for Multi-Tenancy)
        administration.AddItem(
            new ApplicationMenuItem(
                "FeatureManagement",
                l["Menu:FeatureManagement"],
                "/FeatureManagement",
                icon: "fas fa-toggle-on",
                order: 5
            )
        );

        // SYSTEM CONFIGURATION
        var systemConfig = administration.AddItem(
            new ApplicationMenuItem(
                "SystemConfiguration",
                l["Menu:SystemConfiguration"],
                icon: "fas fa-cogs",
                order: 20
            )
        );

        systemConfig.AddItem(
            new ApplicationMenuItem(
                "EmailTemplates",
                l["Menu:EmailTemplates"],
                "/TextTemplateManagement/TextTemplates",
                icon: "fas fa-envelope"
            )
        );

        systemConfig.AddItem(
            new ApplicationMenuItem(
                "Languages",
                l["Menu:Languages"],
                "/LanguageManagement/Languages",
                icon: "fas fa-globe"
            )
        );

        systemConfig.AddItem(
            new ApplicationMenuItem(
                "BackgroundJobs",
                l["Menu:BackgroundJobs"],
                "~/BackgroundJobs",
                icon: "fas fa-cog"
            ).RequirePermissions(GrcPermissions.Admin.BackgroundJobs)
        );

        systemConfig.AddItem(
            new ApplicationMenuItem(
                "SystemHealth",
                l["Menu:SystemHealth"],
                "~/SystemHealth",
                icon: "fas fa-heartbeat"
            ).RequirePermissions(GrcPermissions.Admin.SystemHealth)
        );

        // API MANAGEMENT
        administration.AddItem(
            new ApplicationMenuItem(
                "ApiManagement",
                l["Menu:ApiManagement"],
                "~/ApiManagement",
                icon: "fas fa-key",
                order: 25
            ).RequirePermissions(GrcPermissions.Admin.ApiManagement)
        );

        // HELP & SUPPORT
        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Help",
                l["Menu:Help"],
                icon: "fas fa-question-circle",
                order: 999
            )
            .AddItem(
                new ApplicationMenuItem(
                    "HelpCenter",
                    l["Menu:HelpCenter"],
                    "~/Help",
                    icon: "fas fa-book"
                )
            )
            .AddItem(
                new ApplicationMenuItem(
                    "Documentation",
                    l["Menu:Documentation"],
                    "~/Documentation",
                    icon: "fas fa-file-alt"
                )
            )
            .AddItem(
                new ApplicationMenuItem(
                    "Support",
                    l["Menu:Support"],
                    "~/Support",
                    icon: "fas fa-life-ring"
                )
            )
            .AddItem(
                new ApplicationMenuItem(
                    "WhatsNew",
                    l["Menu:WhatsNew"],
                    "~/WhatsNew",
                    icon: "fas fa-star"
                )
            )
        );

        return Task.CompletedTask;
    }
}

