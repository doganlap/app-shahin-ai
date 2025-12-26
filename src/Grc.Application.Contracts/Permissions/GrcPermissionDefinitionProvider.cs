using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Grc.Localization;

namespace Grc.Permissions;

/// <summary>
/// Permission definition provider for GRC Platform
/// </summary>
public class GrcPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var grcGroup = context.AddGroup(GrcPermissions.GroupName, L("Permission:Grc"));

        // Frameworks permissions
        var frameworks = grcGroup.AddPermission(GrcPermissions.Frameworks.Default, L("Permission:Frameworks"));
        frameworks.AddChild(GrcPermissions.Frameworks.View, L("Permission:Frameworks.View"));
        frameworks.AddChild(GrcPermissions.Frameworks.Create, L("Permission:Frameworks.Create"));
        frameworks.AddChild(GrcPermissions.Frameworks.Edit, L("Permission:Frameworks.Edit"));
        frameworks.AddChild(GrcPermissions.Frameworks.Delete, L("Permission:Frameworks.Delete"));
        frameworks.AddChild(GrcPermissions.Frameworks.Import, L("Permission:Frameworks.Import"));

        // Assessments permissions
        var assessments = grcGroup.AddPermission(GrcPermissions.Assessments.Default, L("Permission:Assessments"));
        assessments.AddChild(GrcPermissions.Assessments.View, L("Permission:Assessments.View"));
        assessments.AddChild(GrcPermissions.Assessments.Create, L("Permission:Assessments.Create"));
        assessments.AddChild(GrcPermissions.Assessments.Edit, L("Permission:Assessments.Edit"));
        assessments.AddChild(GrcPermissions.Assessments.Delete, L("Permission:Assessments.Delete"));
        assessments.AddChild(GrcPermissions.Assessments.AssignControls, L("Permission:Assessments.AssignControls"));
        assessments.AddChild(GrcPermissions.Assessments.VerifyControls, L("Permission:Assessments.VerifyControls"));
        assessments.AddChild(GrcPermissions.Assessments.Generate, L("Permission:Assessments.Generate"));

        // Control Assessments permissions
        var controlAssessments = grcGroup.AddPermission(GrcPermissions.ControlAssessments.Default, L("Permission:ControlAssessments"));
        controlAssessments.AddChild(GrcPermissions.ControlAssessments.ViewOwn, L("Permission:ControlAssessments.ViewOwn"));
        controlAssessments.AddChild(GrcPermissions.ControlAssessments.ViewDepartment, L("Permission:ControlAssessments.ViewDepartment"));
        controlAssessments.AddChild(GrcPermissions.ControlAssessments.ViewAll, L("Permission:ControlAssessments.ViewAll"));
        controlAssessments.AddChild(GrcPermissions.ControlAssessments.UpdateOwn, L("Permission:ControlAssessments.UpdateOwn"));
        controlAssessments.AddChild(GrcPermissions.ControlAssessments.UploadEvidence, L("Permission:ControlAssessments.UploadEvidence"));
        controlAssessments.AddChild(GrcPermissions.ControlAssessments.SubmitForReview, L("Permission:ControlAssessments.SubmitForReview"));
        controlAssessments.AddChild(GrcPermissions.ControlAssessments.Verify, L("Permission:ControlAssessments.Verify"));
        controlAssessments.AddChild(GrcPermissions.ControlAssessments.Reject, L("Permission:ControlAssessments.Reject"));

        // Evidence permissions
        var evidence = grcGroup.AddPermission(GrcPermissions.Evidence.Default, L("Permission:Evidence"));
        evidence.AddChild(GrcPermissions.Evidence.View, L("Permission:Evidence.View"));
        evidence.AddChild(GrcPermissions.Evidence.Upload, L("Permission:Evidence.Upload"));
        evidence.AddChild(GrcPermissions.Evidence.Delete, L("Permission:Evidence.Delete"));
        evidence.AddChild(GrcPermissions.Evidence.Download, L("Permission:Evidence.Download"));

        // Risks permissions
        var risks = grcGroup.AddPermission(GrcPermissions.Risks.Default, L("Permission:Risks"));
        risks.AddChild(GrcPermissions.Risks.View, L("Permission:Risks.View"));
        risks.AddChild(GrcPermissions.Risks.Create, L("Permission:Risks.Create"));
        risks.AddChild(GrcPermissions.Risks.Edit, L("Permission:Risks.Edit"));
        risks.AddChild(GrcPermissions.Risks.Delete, L("Permission:Risks.Delete"));
        risks.AddChild(GrcPermissions.Risks.Assess, L("Permission:Risks.Assess"));
        risks.AddChild(GrcPermissions.Risks.Treat, L("Permission:Risks.Treat"));

        // Reports permissions
        var reports = grcGroup.AddPermission(GrcPermissions.Reports.Default, L("Permission:Reports"));
        reports.AddChild(GrcPermissions.Reports.View, L("Permission:Reports.View"));
        reports.AddChild(GrcPermissions.Reports.Generate, L("Permission:Reports.Generate"));
        reports.AddChild(GrcPermissions.Reports.Export, L("Permission:Reports.Export"));

        // Workflows permissions
        var workflows = grcGroup.AddPermission(GrcPermissions.Workflows.Default, L("Permission:Workflows"));
        workflows.AddChild(GrcPermissions.Workflows.View, L("Permission:Workflows.View"));
        workflows.AddChild(GrcPermissions.Workflows.Manage, L("Permission:Workflows.Manage"));
        workflows.AddChild(GrcPermissions.Workflows.Execute, L("Permission:Workflows.Execute"));

        // Admin permissions
        var admin = grcGroup.AddPermission(GrcPermissions.Admin.Default, L("Permission:Admin"));
        admin.AddChild(GrcPermissions.Admin.ManageUsers, L("Permission:Admin.ManageUsers"));
        admin.AddChild(GrcPermissions.Admin.ManageSettings, L("Permission:Admin.ManageSettings"));
        admin.AddChild(GrcPermissions.Admin.ViewAuditLog, L("Permission:Admin.ViewAuditLog"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<GrcResource>(name);
    }
}

