namespace Grc.Permissions;

/// <summary>
/// GRC Platform permissions
/// </summary>
public static class GrcPermissions
{
    public const string GroupName = "Grc";
    
    public static class Frameworks
    {
        public const string Default = GroupName + ".Frameworks";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Import = Default + ".Import";
    }
    
    public static class Assessments
    {
        public const string Default = GroupName + ".Assessments";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string AssignControls = Default + ".AssignControls";
        public const string VerifyControls = Default + ".VerifyControls";
        public const string Generate = Default + ".Generate";
    }
    
    public static class ControlAssessments
    {
        public const string Default = GroupName + ".ControlAssessments";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string ViewDepartment = Default + ".ViewDepartment";
        public const string ViewAll = Default + ".ViewAll";
        public const string UpdateOwn = Default + ".UpdateOwn";
        public const string UploadEvidence = Default + ".UploadEvidence";
        public const string SubmitForReview = Default + ".SubmitForReview";
        public const string Verify = Default + ".Verify";
        public const string Reject = Default + ".Reject";
    }
    
    public static class Evidence
    {
        public const string Default = GroupName + ".Evidence";
        public const string View = Default + ".View";
        public const string Upload = Default + ".Upload";
        public const string Delete = Default + ".Delete";
        public const string Download = Default + ".Download";
    }
    
    public static class Risks
    {
        public const string Default = GroupName + ".Risks";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Assess = Default + ".Assess";
        public const string Treat = Default + ".Treat";
    }
    
    public static class Reports
    {
        public const string Default = GroupName + ".Reports";
        public const string View = Default + ".View";
        public const string Generate = Default + ".Generate";
        public const string Export = Default + ".Export";
    }
    
    public static class Workflows
    {
        public const string Default = GroupName + ".Workflows";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
        public const string Execute = Default + ".Execute";
    }
    
    public static class Admin
    {
        public const string Default = GroupName + ".Admin";
        public const string ManageUsers = Default + ".ManageUsers";
        public const string ManageSettings = Default + ".ManageSettings";
        public const string ViewAuditLog = Default + ".ViewAuditLog";
    }
}

