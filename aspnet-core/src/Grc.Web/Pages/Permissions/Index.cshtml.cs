using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grc.EntityFrameworkCore;
using Grc.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Grc.Web.Pages.Permissions;

/// <summary>
/// Permission management page - dynamically loads all registered permissions from ABP.
/// Secured with admin role requirement.
/// </summary>
[Authorize(Roles = "admin")]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;
    private readonly IPermissionManager _permissionManager;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;

    public List<RoleDto> Roles { get; set; } = new();
    public List<PermissionDto> Permissions { get; set; } = new();
    public Dictionary<string, PermissionGroupDto> GroupedPermissions { get; set; } = new();
    public int TotalPermissionCount => Permissions.Count;
    public int GrantedPermissionCount => Permissions.Count(p => p.IsGranted);

    [BindProperty(SupportsGet = true)]
    public Guid? SelectedRoleId { get; set; }
    
    public string? SelectedRoleName { get; set; }

    public IndexModel(
        GrcDbContext dbContext,
        IPermissionManager permissionManager,
        IPermissionDefinitionManager permissionDefinitionManager)
    {
        _dbContext = dbContext;
        _permissionManager = permissionManager;
        _permissionDefinitionManager = permissionDefinitionManager;
    }

    public async Task OnGetAsync()
    {
        var roles = await _dbContext.Set<IdentityRole>().OrderBy(r => r.Name).ToListAsync();

        foreach (var role in roles)
        {
            var permissionCount = await _dbContext.Set<PermissionGrant>()
                .CountAsync(p => p.ProviderName == "R" && p.ProviderKey == role.Id.ToString());

            Roles.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name ?? "Unknown",
                PermissionCount = permissionCount
            });
        }

        if (SelectedRoleId.HasValue)
        {
            SelectedRoleName = roles.FirstOrDefault(r => r.Id == SelectedRoleId.Value)?.Name;
            await LoadPermissionsForRole(SelectedRoleId.Value);
        }
    }

    public async Task<IActionResult> OnPostUpdatePermissionsAsync(Guid roleId, List<string> grantedPermissions)
    {
        try
        {
            var allPermissions = await GetAllPermissionNamesAsync();
            var updatedCount = 0;

            // Update each permission
            foreach (var permissionName in allPermissions)
            {
                var shouldGrant = grantedPermissions.Contains(permissionName);
                
                try
                {
                    var currentGrant = await _permissionManager.GetAsync(permissionName, "R", roleId.ToString());

                    if (currentGrant?.IsGranted != shouldGrant)
                    {
                        await _permissionManager.SetAsync(permissionName, "R", roleId.ToString(), shouldGrant);
                        updatedCount++;
                    }
                }
                catch (Exception)
                {
                    // Permission might not exist in definition - skip it
                    continue;
                }
            }

            return new JsonResult(new { 
                success = true, 
                message = $"Permissions updated successfully. {updatedCount} permission(s) changed.",
                updatedCount
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }

    private async Task LoadPermissionsForRole(Guid roleId)
    {
        var roleKey = roleId.ToString();
        var allPermissions = await GetAllPermissionNamesAsync();

        Permissions = new List<PermissionDto>();
        GroupedPermissions = new Dictionary<string, PermissionGroupDto>();

        foreach (var permissionName in allPermissions)
        {
            try
            {
                var grant = await _permissionManager.GetAsync(permissionName, "R", roleKey);
                var groupName = GetPermissionGroup(permissionName);
                var permission = new PermissionDto
                {
                    Name = permissionName,
                    DisplayName = await GetPermissionDisplayNameAsync(permissionName),
                    Description = GetPermissionDescription(permissionName),
                    IsGranted = grant?.IsGranted ?? false,
                    Group = groupName,
                    IsParent = IsParentPermission(permissionName)
                };
                
                Permissions.Add(permission);
                
                // Group permissions by category
                if (!GroupedPermissions.ContainsKey(groupName))
                {
                    GroupedPermissions[groupName] = new PermissionGroupDto
                    {
                        Name = groupName,
                        DisplayName = GetGroupDisplayName(groupName),
                        Icon = GetGroupIcon(groupName),
                        Permissions = new List<PermissionDto>()
                    };
                }
                GroupedPermissions[groupName].Permissions.Add(permission);
            }
            catch
            {
                // Skip permissions that can't be loaded
            }
        }

        // Update group statistics
        foreach (var group in GroupedPermissions.Values)
        {
            group.TotalCount = group.Permissions.Count;
            group.GrantedCount = group.Permissions.Count(p => p.IsGranted);
        }
    }

    private bool IsParentPermission(string permissionName)
    {
        // Parent permissions are the ones without a final action suffix
        var parts = permissionName.Split('.');
        if (parts.Length <= 2) return true;
        
        var actions = new[] { "View", "Create", "Edit", "Delete", "Upload", "Download", 
            "Start", "Complete", "Assess", "Treat", "Verify", "Reject", "Send", "Manage",
            "Execute", "Generate", "Export", "Schedule", "Configure", "Sync", "Publish", 
            "Attest", "AssignControls", "AssignTasks", "AddFinding", "ViewOwn", "UpdateOwn",
            "UseRecommendations", "TrainModel", "SeedData", "SystemHealth", "BackgroundJobs",
            "ApiManagement", "ViewTasks", "ViewNotifications", "ManageSettings", "Update" };
        
        return !actions.Contains(parts.Last());
    }

    private string GetPermissionGroup(string permissionName)
    {
        if (permissionName.StartsWith("Grc."))
        {
            var parts = permissionName.Replace("Grc.", "").Split('.');
            return parts.Length > 0 ? parts[0] : "General";
        }
        if (permissionName.StartsWith("AbpIdentity")) return "Identity";
        if (permissionName.StartsWith("AbpTenantManagement")) return "Tenants";
        if (permissionName.StartsWith("AbpAuditLogging")) return "AuditLogs";
        if (permissionName.StartsWith("AbpSettingManagement")) return "Settings";
        return "Other";
    }

    private string GetGroupDisplayName(string groupName)
    {
        return groupName switch
        {
            "Admin" => "🔧 Administration",
            "Dashboard" => "📊 Dashboard",
            "Frameworks" => "📋 Frameworks",
            "Regulators" => "🏛️ Regulators",
            "Assessments" => "✅ Assessments",
            "ControlAssessments" => "🎯 Control Assessments",
            "Evidence" => "📎 Evidence",
            "Risks" => "⚠️ Risks",
            "Audits" => "🔍 Audits",
            "ActionPlans" => "📝 Action Plans",
            "Policies" => "📜 Policies",
            "Calendar" => "📅 Calendar",
            "Workflows" => "🔄 Workflows",
            "Notifications" => "🔔 Notifications",
            "Vendors" => "🏢 Vendors",
            "Reports" => "📈 Reports",
            "Integrations" => "🔗 Integrations",
            "AI" => "🤖 AI Features",
            "Subscriptions" => "💳 Subscriptions",
            "MyWorkspace" => "👤 My Workspace",
            "Identity" => "👥 Identity Management",
            "Tenants" => "🏠 Tenant Management",
            "AuditLogs" => "📋 Audit Logs",
            "Settings" => "⚙️ Settings",
            _ => $"📁 {groupName}"
        };
    }

    private string GetGroupIcon(string groupName)
    {
        return groupName switch
        {
            "Admin" => "fas fa-cog",
            "Dashboard" => "fas fa-tachometer-alt",
            "Frameworks" => "fas fa-sitemap",
            "Regulators" => "fas fa-building",
            "Assessments" => "fas fa-clipboard-check",
            "ControlAssessments" => "fas fa-tasks",
            "Evidence" => "fas fa-file-alt",
            "Risks" => "fas fa-exclamation-triangle",
            "Audits" => "fas fa-search",
            "ActionPlans" => "fas fa-list-check",
            "Policies" => "fas fa-file-contract",
            "Calendar" => "fas fa-calendar",
            "Workflows" => "fas fa-project-diagram",
            "Notifications" => "fas fa-bell",
            "Vendors" => "fas fa-handshake",
            "Reports" => "fas fa-chart-bar",
            "Integrations" => "fas fa-plug",
            "AI" => "fas fa-robot",
            "Subscriptions" => "fas fa-credit-card",
            "MyWorkspace" => "fas fa-user",
            "Identity" => "fas fa-users",
            "Tenants" => "fas fa-building",
            "AuditLogs" => "fas fa-history",
            "Settings" => "fas fa-sliders-h",
            _ => "fas fa-folder"
        };
    }

    private string GetPermissionDescription(string permissionName)
    {
        var parts = permissionName.Split('.');
        if (parts.Length < 2) return "";
        
        var action = parts.Last();
        return action switch
        {
            "View" => "View items",
            "Create" => "Create new items",
            "Edit" => "Edit existing items",
            "Delete" => "Delete items",
            "Upload" => "Upload files",
            "Download" => "Download files",
            "Start" => "Start workflow",
            "Complete" => "Complete workflow",
            "Assess" => "Perform assessments",
            "Treat" => "Apply treatments",
            "Verify" => "Verify items",
            "Reject" => "Reject items",
            "Manage" => "Full management access",
            "Execute" => "Execute operations",
            _ => ""
        };
    }

    /// <summary>
    /// Dynamically loads all permission names from ABP's permission definition system.
    /// This ensures the UI always shows all registered permissions.
    /// </summary>
    private async Task<List<string>> GetAllPermissionNamesAsync()
    {
        var permissions = new List<string>();

        try
        {
            // Get all permissions from ABP's permission definition manager (async API in ABP 8.x)
            var allPermissions = await _permissionDefinitionManager.GetPermissionsAsync();
            
            foreach (var permission in allPermissions)
            {
                permissions.Add(permission.Name);
            }
        }
        catch
        {
            // Fallback to static list if dynamic loading fails
            permissions = GetStaticPermissionNames();
        }

        return permissions.Distinct().OrderBy(p => p).ToList();
    }

    /// <summary>
    /// Fallback static permission list matching GrcPermissions.cs
    /// </summary>
    private List<string> GetStaticPermissionNames()
    {
        return new List<string>
        {
            // Admin
            GrcPermissions.Admin.Default,
            GrcPermissions.Admin.SeedData,
            GrcPermissions.Admin.SystemHealth,
            GrcPermissions.Admin.BackgroundJobs,
            GrcPermissions.Admin.ApiManagement,

            // Dashboard
            GrcPermissions.Dashboard.Default,
            GrcPermissions.Dashboard.View,

            // Frameworks
            GrcPermissions.Frameworks.Default,
            GrcPermissions.Frameworks.View,
            GrcPermissions.Frameworks.Create,
            GrcPermissions.Frameworks.Edit,
            GrcPermissions.Frameworks.Delete,

            // Regulators
            GrcPermissions.Regulators.Default,
            GrcPermissions.Regulators.View,
            GrcPermissions.Regulators.Create,
            GrcPermissions.Regulators.Edit,
            GrcPermissions.Regulators.Delete,

            // Assessments
            GrcPermissions.Assessments.Default,
            GrcPermissions.Assessments.View,
            GrcPermissions.Assessments.Create,
            GrcPermissions.Assessments.Edit,
            GrcPermissions.Assessments.Delete,
            GrcPermissions.Assessments.Start,
            GrcPermissions.Assessments.Complete,
            GrcPermissions.Assessments.AssignControls,

            // Control Assessments
            GrcPermissions.ControlAssessments.Default,
            GrcPermissions.ControlAssessments.View,
            GrcPermissions.ControlAssessments.ViewOwn,
            GrcPermissions.ControlAssessments.Update,
            GrcPermissions.ControlAssessments.UpdateOwn,
            GrcPermissions.ControlAssessments.Verify,
            GrcPermissions.ControlAssessments.Reject,

            // Evidence
            GrcPermissions.Evidence.Default,
            GrcPermissions.Evidence.View,
            GrcPermissions.Evidence.Upload,
            GrcPermissions.Evidence.Download,
            GrcPermissions.Evidence.Delete,

            // Risks
            GrcPermissions.Risks.Default,
            GrcPermissions.Risks.View,
            GrcPermissions.Risks.Create,
            GrcPermissions.Risks.Edit,
            GrcPermissions.Risks.Delete,
            GrcPermissions.Risks.Assess,
            GrcPermissions.Risks.Treat,

            // Audits
            GrcPermissions.Audits.Default,
            GrcPermissions.Audits.View,
            GrcPermissions.Audits.Create,
            GrcPermissions.Audits.Edit,
            GrcPermissions.Audits.Delete,
            GrcPermissions.Audits.AddFinding,

            // Action Plans
            GrcPermissions.ActionPlans.Default,
            GrcPermissions.ActionPlans.View,
            GrcPermissions.ActionPlans.Create,
            GrcPermissions.ActionPlans.Edit,
            GrcPermissions.ActionPlans.Delete,
            GrcPermissions.ActionPlans.AssignTasks,

            // Policies
            GrcPermissions.Policies.Default,
            GrcPermissions.Policies.View,
            GrcPermissions.Policies.Create,
            GrcPermissions.Policies.Edit,
            GrcPermissions.Policies.Delete,
            GrcPermissions.Policies.Publish,
            GrcPermissions.Policies.Attest,

            // Calendar
            GrcPermissions.Calendar.Default,
            GrcPermissions.Calendar.View,
            GrcPermissions.Calendar.Create,
            GrcPermissions.Calendar.Edit,
            GrcPermissions.Calendar.Delete,

            // Workflows
            GrcPermissions.Workflows.Default,
            GrcPermissions.Workflows.View,
            GrcPermissions.Workflows.Create,
            GrcPermissions.Workflows.Edit,
            GrcPermissions.Workflows.Delete,
            GrcPermissions.Workflows.Execute,

            // Notifications
            GrcPermissions.Notifications.Default,
            GrcPermissions.Notifications.View,
            GrcPermissions.Notifications.Send,
            GrcPermissions.Notifications.Manage,

            // Vendors
            GrcPermissions.Vendors.Default,
            GrcPermissions.Vendors.View,
            GrcPermissions.Vendors.Create,
            GrcPermissions.Vendors.Edit,
            GrcPermissions.Vendors.Delete,
            GrcPermissions.Vendors.Assess,

            // Reports
            GrcPermissions.Reports.Default,
            GrcPermissions.Reports.View,
            GrcPermissions.Reports.Generate,
            GrcPermissions.Reports.Export,
            GrcPermissions.Reports.Schedule,

            // Integrations
            GrcPermissions.Integrations.Default,
            GrcPermissions.Integrations.View,
            GrcPermissions.Integrations.Configure,
            GrcPermissions.Integrations.Sync,

            // AI
            GrcPermissions.AI.Default,
            GrcPermissions.AI.View,
            GrcPermissions.AI.UseRecommendations,
            GrcPermissions.AI.TrainModel,

            // Subscriptions
            GrcPermissions.Subscriptions.Default,
            GrcPermissions.Subscriptions.View,
            GrcPermissions.Subscriptions.Manage,

            // My Workspace
            GrcPermissions.MyWorkspace.Default,
            GrcPermissions.MyWorkspace.ViewTasks,
            GrcPermissions.MyWorkspace.ViewNotifications,
            GrcPermissions.MyWorkspace.ManageSettings,

            // ABP Identity
            "AbpIdentity.Users",
            "AbpIdentity.Users.Create",
            "AbpIdentity.Users.Update",
            "AbpIdentity.Users.Delete",
            "AbpIdentity.Roles",
            "AbpIdentity.Roles.Create",
            "AbpIdentity.Roles.Update",
            "AbpIdentity.Roles.Delete",

            // ABP Tenant Management
            "AbpTenantManagement.Tenants",
            "AbpTenantManagement.Tenants.Create",
            "AbpTenantManagement.Tenants.Update",
            "AbpTenantManagement.Tenants.Delete",

            // ABP Other
            "AbpAuditLogging.AuditLogs",
            "AbpSettingManagement.Settings"
        };
    }

    private async Task<string> GetPermissionDisplayNameAsync(string permissionName)
    {
        // Try to get localized name from ABP
        try
        {
            var permission = await _permissionDefinitionManager.GetOrNullAsync(permissionName);
            if (permission != null)
            {
                return permission.DisplayName?.Localize(StringLocalizerFactory)?.Value ?? FormatPermissionName(permissionName);
            }
        }
        catch
        {
            // Fallback to formatting if permission not found
        }

        return FormatPermissionName(permissionName);
    }

    private string FormatPermissionName(string permissionName)
    {
        var parts = permissionName.Split('.');
        if (parts.Length == 0) return permissionName;
        
        // Get the last meaningful part
        var lastPart = parts.Last();
        
        // Add spaces before capital letters
        var formatted = string.Concat(lastPart.Select((c, i) => 
            i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        
        return formatted;
    }
}

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int PermissionCount { get; set; }
}

public class PermissionDto
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsGranted { get; set; }
    public string Group { get; set; } = "General";
    public bool IsParent { get; set; }
}

public class PermissionGroupDto
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Icon { get; set; } = "fas fa-folder";
    public List<PermissionDto> Permissions { get; set; } = new();
    public int TotalCount { get; set; }
    public int GrantedCount { get; set; }
}


