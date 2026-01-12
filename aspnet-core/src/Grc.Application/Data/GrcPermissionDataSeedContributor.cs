using System.Threading.Tasks;
using Grc.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;

namespace Grc.Application.Data;

/// <summary>
/// Seeds all GRC permissions to the admin role
/// </summary>
public class GrcPermissionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IPermissionManager _permissionManager;

    public GrcPermissionDataSeedContributor(IPermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // Grant all GRC permissions to admin role
        var adminPermissions = new[]
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

            // ControlAssessments
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

            // ActionPlans
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

            // MyWorkspace
            GrcPermissions.MyWorkspace.Default,
            GrcPermissions.MyWorkspace.ViewTasks,
            GrcPermissions.MyWorkspace.ViewNotifications,
            GrcPermissions.MyWorkspace.ManageSettings,
        };

        foreach (var permission in adminPermissions)
        {
            await _permissionManager.SetForRoleAsync("admin", permission, true);
        }
    }
}
