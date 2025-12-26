using Grc.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Grc.Permissions;

public class GrcPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(GrcPermissions.GroupName);

        // Admin permissions
        var adminPermission = myGroup.AddPermission(GrcPermissions.Admin.Default, L("Permission:Admin"));
        adminPermission.AddChild(GrcPermissions.Admin.SeedData, L("Permission:Admin.SeedData"));
        adminPermission.AddChild(GrcPermissions.Admin.SystemHealth, L("Permission:Admin.SystemHealth"));
        adminPermission.AddChild(GrcPermissions.Admin.BackgroundJobs, L("Permission:Admin.BackgroundJobs"));
        adminPermission.AddChild(GrcPermissions.Admin.ApiManagement, L("Permission:Admin.ApiManagement"));

        // Dashboard permissions
        var dashboardPermission = myGroup.AddPermission(GrcPermissions.Dashboard.Default, L("Permission:Dashboard"));
        dashboardPermission.AddChild(GrcPermissions.Dashboard.View, L("Permission:Dashboard.View"));

        // Frameworks permissions
        var frameworksPermission = myGroup.AddPermission(GrcPermissions.Frameworks.Default, L("Permission:Frameworks"));
        frameworksPermission.AddChild(GrcPermissions.Frameworks.View, L("Permission:Frameworks.View"));
        frameworksPermission.AddChild(GrcPermissions.Frameworks.Create, L("Permission:Frameworks.Create"));
        frameworksPermission.AddChild(GrcPermissions.Frameworks.Edit, L("Permission:Frameworks.Edit"));
        frameworksPermission.AddChild(GrcPermissions.Frameworks.Delete, L("Permission:Frameworks.Delete"));

        // Regulators permissions
        var regulatorsPermission = myGroup.AddPermission(GrcPermissions.Regulators.Default, L("Permission:Regulators"));
        regulatorsPermission.AddChild(GrcPermissions.Regulators.View, L("Permission:Regulators.View"));
        regulatorsPermission.AddChild(GrcPermissions.Regulators.Create, L("Permission:Regulators.Create"));
        regulatorsPermission.AddChild(GrcPermissions.Regulators.Edit, L("Permission:Regulators.Edit"));
        regulatorsPermission.AddChild(GrcPermissions.Regulators.Delete, L("Permission:Regulators.Delete"));

        // Assessments permissions
        var assessmentsPermission = myGroup.AddPermission(GrcPermissions.Assessments.Default, L("Permission:Assessments"));
        assessmentsPermission.AddChild(GrcPermissions.Assessments.View, L("Permission:Assessments.View"));
        assessmentsPermission.AddChild(GrcPermissions.Assessments.Create, L("Permission:Assessments.Create"));
        assessmentsPermission.AddChild(GrcPermissions.Assessments.Edit, L("Permission:Assessments.Edit"));
        assessmentsPermission.AddChild(GrcPermissions.Assessments.Delete, L("Permission:Assessments.Delete"));
        assessmentsPermission.AddChild(GrcPermissions.Assessments.Start, L("Permission:Assessments.Start"));
        assessmentsPermission.AddChild(GrcPermissions.Assessments.Complete, L("Permission:Assessments.Complete"));
        assessmentsPermission.AddChild(GrcPermissions.Assessments.AssignControls, L("Permission:Assessments.AssignControls"));

        // ControlAssessments permissions
        var controlAssessmentsPermission = myGroup.AddPermission(GrcPermissions.ControlAssessments.Default, L("Permission:ControlAssessments"));
        controlAssessmentsPermission.AddChild(GrcPermissions.ControlAssessments.View, L("Permission:ControlAssessments.View"));
        controlAssessmentsPermission.AddChild(GrcPermissions.ControlAssessments.ViewOwn, L("Permission:ControlAssessments.ViewOwn"));
        controlAssessmentsPermission.AddChild(GrcPermissions.ControlAssessments.Update, L("Permission:ControlAssessments.Update"));
        controlAssessmentsPermission.AddChild(GrcPermissions.ControlAssessments.UpdateOwn, L("Permission:ControlAssessments.UpdateOwn"));
        controlAssessmentsPermission.AddChild(GrcPermissions.ControlAssessments.Verify, L("Permission:ControlAssessments.Verify"));
        controlAssessmentsPermission.AddChild(GrcPermissions.ControlAssessments.Reject, L("Permission:ControlAssessments.Reject"));

        // Evidence permissions
        var evidencePermission = myGroup.AddPermission(GrcPermissions.Evidence.Default, L("Permission:Evidence"));
        evidencePermission.AddChild(GrcPermissions.Evidence.View, L("Permission:Evidence.View"));
        evidencePermission.AddChild(GrcPermissions.Evidence.Upload, L("Permission:Evidence.Upload"));
        evidencePermission.AddChild(GrcPermissions.Evidence.Download, L("Permission:Evidence.Download"));
        evidencePermission.AddChild(GrcPermissions.Evidence.Delete, L("Permission:Evidence.Delete"));

        // Risks permissions
        var risksPermission = myGroup.AddPermission(GrcPermissions.Risks.Default, L("Permission:Risks"));
        risksPermission.AddChild(GrcPermissions.Risks.View, L("Permission:Risks.View"));
        risksPermission.AddChild(GrcPermissions.Risks.Create, L("Permission:Risks.Create"));
        risksPermission.AddChild(GrcPermissions.Risks.Edit, L("Permission:Risks.Edit"));
        risksPermission.AddChild(GrcPermissions.Risks.Delete, L("Permission:Risks.Delete"));
        risksPermission.AddChild(GrcPermissions.Risks.Assess, L("Permission:Risks.Assess"));
        risksPermission.AddChild(GrcPermissions.Risks.Treat, L("Permission:Risks.Treat"));

        // Audits permissions
        var auditsPermission = myGroup.AddPermission(GrcPermissions.Audits.Default, L("Permission:Audits"));
        auditsPermission.AddChild(GrcPermissions.Audits.View, L("Permission:Audits.View"));
        auditsPermission.AddChild(GrcPermissions.Audits.Create, L("Permission:Audits.Create"));
        auditsPermission.AddChild(GrcPermissions.Audits.Edit, L("Permission:Audits.Edit"));
        auditsPermission.AddChild(GrcPermissions.Audits.Delete, L("Permission:Audits.Delete"));
        auditsPermission.AddChild(GrcPermissions.Audits.AddFinding, L("Permission:Audits.AddFinding"));

        // ActionPlans permissions
        var actionPlansPermission = myGroup.AddPermission(GrcPermissions.ActionPlans.Default, L("Permission:ActionPlans"));
        actionPlansPermission.AddChild(GrcPermissions.ActionPlans.View, L("Permission:ActionPlans.View"));
        actionPlansPermission.AddChild(GrcPermissions.ActionPlans.Create, L("Permission:ActionPlans.Create"));
        actionPlansPermission.AddChild(GrcPermissions.ActionPlans.Edit, L("Permission:ActionPlans.Edit"));
        actionPlansPermission.AddChild(GrcPermissions.ActionPlans.Delete, L("Permission:ActionPlans.Delete"));
        actionPlansPermission.AddChild(GrcPermissions.ActionPlans.AssignTasks, L("Permission:ActionPlans.AssignTasks"));

        // Policies permissions
        var policiesPermission = myGroup.AddPermission(GrcPermissions.Policies.Default, L("Permission:Policies"));
        policiesPermission.AddChild(GrcPermissions.Policies.View, L("Permission:Policies.View"));
        policiesPermission.AddChild(GrcPermissions.Policies.Create, L("Permission:Policies.Create"));
        policiesPermission.AddChild(GrcPermissions.Policies.Edit, L("Permission:Policies.Edit"));
        policiesPermission.AddChild(GrcPermissions.Policies.Delete, L("Permission:Policies.Delete"));
        policiesPermission.AddChild(GrcPermissions.Policies.Publish, L("Permission:Policies.Publish"));
        policiesPermission.AddChild(GrcPermissions.Policies.Attest, L("Permission:Policies.Attest"));

        // Calendar permissions
        var calendarPermission = myGroup.AddPermission(GrcPermissions.Calendar.Default, L("Permission:Calendar"));
        calendarPermission.AddChild(GrcPermissions.Calendar.View, L("Permission:Calendar.View"));
        calendarPermission.AddChild(GrcPermissions.Calendar.Create, L("Permission:Calendar.Create"));
        calendarPermission.AddChild(GrcPermissions.Calendar.Edit, L("Permission:Calendar.Edit"));
        calendarPermission.AddChild(GrcPermissions.Calendar.Delete, L("Permission:Calendar.Delete"));

        // Workflows permissions
        var workflowsPermission = myGroup.AddPermission(GrcPermissions.Workflows.Default, L("Permission:Workflows"));
        workflowsPermission.AddChild(GrcPermissions.Workflows.View, L("Permission:Workflows.View"));
        workflowsPermission.AddChild(GrcPermissions.Workflows.Create, L("Permission:Workflows.Create"));
        workflowsPermission.AddChild(GrcPermissions.Workflows.Edit, L("Permission:Workflows.Edit"));
        workflowsPermission.AddChild(GrcPermissions.Workflows.Delete, L("Permission:Workflows.Delete"));
        workflowsPermission.AddChild(GrcPermissions.Workflows.Execute, L("Permission:Workflows.Execute"));

        // Notifications permissions
        var notificationsPermission = myGroup.AddPermission(GrcPermissions.Notifications.Default, L("Permission:Notifications"));
        notificationsPermission.AddChild(GrcPermissions.Notifications.View, L("Permission:Notifications.View"));
        notificationsPermission.AddChild(GrcPermissions.Notifications.Send, L("Permission:Notifications.Send"));
        notificationsPermission.AddChild(GrcPermissions.Notifications.Manage, L("Permission:Notifications.Manage"));

        // Vendors permissions
        var vendorsPermission = myGroup.AddPermission(GrcPermissions.Vendors.Default, L("Permission:Vendors"));
        vendorsPermission.AddChild(GrcPermissions.Vendors.View, L("Permission:Vendors.View"));
        vendorsPermission.AddChild(GrcPermissions.Vendors.Create, L("Permission:Vendors.Create"));
        vendorsPermission.AddChild(GrcPermissions.Vendors.Edit, L("Permission:Vendors.Edit"));
        vendorsPermission.AddChild(GrcPermissions.Vendors.Delete, L("Permission:Vendors.Delete"));
        vendorsPermission.AddChild(GrcPermissions.Vendors.Assess, L("Permission:Vendors.Assess"));

        // Reports permissions
        var reportsPermission = myGroup.AddPermission(GrcPermissions.Reports.Default, L("Permission:Reports"));
        reportsPermission.AddChild(GrcPermissions.Reports.View, L("Permission:Reports.View"));
        reportsPermission.AddChild(GrcPermissions.Reports.Generate, L("Permission:Reports.Generate"));
        reportsPermission.AddChild(GrcPermissions.Reports.Export, L("Permission:Reports.Export"));
        reportsPermission.AddChild(GrcPermissions.Reports.Schedule, L("Permission:Reports.Schedule"));

        // Integrations permissions
        var integrationsPermission = myGroup.AddPermission(GrcPermissions.Integrations.Default, L("Permission:Integrations"));
        integrationsPermission.AddChild(GrcPermissions.Integrations.View, L("Permission:Integrations.View"));
        integrationsPermission.AddChild(GrcPermissions.Integrations.Configure, L("Permission:Integrations.Configure"));
        integrationsPermission.AddChild(GrcPermissions.Integrations.Sync, L("Permission:Integrations.Sync"));

        // AI permissions
        var aiPermission = myGroup.AddPermission(GrcPermissions.AI.Default, L("Permission:AI"));
        aiPermission.AddChild(GrcPermissions.AI.View, L("Permission:AI.View"));
        aiPermission.AddChild(GrcPermissions.AI.UseRecommendations, L("Permission:AI.UseRecommendations"));
        aiPermission.AddChild(GrcPermissions.AI.TrainModel, L("Permission:AI.TrainModel"));

        // Subscriptions permissions
        var subscriptionsPermission = myGroup.AddPermission(GrcPermissions.Subscriptions.Default, L("Permission:Subscriptions"));
        subscriptionsPermission.AddChild(GrcPermissions.Subscriptions.View, L("Permission:Subscriptions.View"));
        subscriptionsPermission.AddChild(GrcPermissions.Subscriptions.Manage, L("Permission:Subscriptions.Manage"));

        // MyWorkspace permissions
        var myWorkspacePermission = myGroup.AddPermission(GrcPermissions.MyWorkspace.Default, L("Permission:MyWorkspace"));
        myWorkspacePermission.AddChild(GrcPermissions.MyWorkspace.ViewTasks, L("Permission:MyWorkspace.ViewTasks"));
        myWorkspacePermission.AddChild(GrcPermissions.MyWorkspace.ViewNotifications, L("Permission:MyWorkspace.ViewNotifications"));
        myWorkspacePermission.AddChild(GrcPermissions.MyWorkspace.ManageSettings, L("Permission:MyWorkspace.ManageSettings"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<GrcResource>(name);
    }
}
