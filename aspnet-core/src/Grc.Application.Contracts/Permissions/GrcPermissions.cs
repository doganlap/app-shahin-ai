namespace Grc.Permissions;

public static class GrcPermissions
{
    public const string GroupName = "Grc";

    public static class Admin
    {
        public const string Default = GroupName + ".Admin";
        public const string SeedData = Default + ".SeedData";
        public const string SystemHealth = Default + ".SystemHealth";
        public const string BackgroundJobs = Default + ".BackgroundJobs";
        public const string ApiManagement = Default + ".ApiManagement";
    }

    public static class Dashboard
    {
        public const string Default = GroupName + ".Dashboard";
        public const string View = Default + ".View";
    }

    public static class Frameworks
    {
        public const string Default = GroupName + ".Frameworks";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Regulators
    {
        public const string Default = GroupName + ".Regulators";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Assessments
    {
        public const string Default = GroupName + ".Assessments";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Start = Default + ".Start";
        public const string Complete = Default + ".Complete";
        public const string AssignControls = Default + ".AssignControls";
    }

    public static class ControlAssessments
    {
        public const string Default = GroupName + ".ControlAssessments";
        public const string View = Default + ".View";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string Update = Default + ".Update";
        public const string UpdateOwn = Default + ".UpdateOwn";
        public const string Verify = Default + ".Verify";
        public const string Reject = Default + ".Reject";
    }

    public static class Evidence
    {
        public const string Default = GroupName + ".Evidence";
        public const string View = Default + ".View";
        public const string Upload = Default + ".Upload";
        public const string Download = Default + ".Download";
        public const string Delete = Default + ".Delete";
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

    public static class Audits
    {
        public const string Default = GroupName + ".Audits";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string AddFinding = Default + ".AddFinding";
    }

    public static class ActionPlans
    {
        public const string Default = GroupName + ".ActionPlans";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string AssignTasks = Default + ".AssignTasks";
    }

    public static class Policies
    {
        public const string Default = GroupName + ".Policies";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Publish = Default + ".Publish";
        public const string Attest = Default + ".Attest";
    }

    public static class Calendar
    {
        public const string Default = GroupName + ".Calendar";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Workflows
    {
        public const string Default = GroupName + ".Workflows";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Execute = Default + ".Execute";
    }

    public static class Notifications
    {
        public const string Default = GroupName + ".Notifications";
        public const string View = Default + ".View";
        public const string Send = Default + ".Send";
        public const string Manage = Default + ".Manage";
    }

    public static class Vendors
    {
        public const string Default = GroupName + ".Vendors";
        public const string View = Default + ".View";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Assess = Default + ".Assess";
    }

    public static class Reports
    {
        public const string Default = GroupName + ".Reports";
        public const string View = Default + ".View";
        public const string Generate = Default + ".Generate";
        public const string Export = Default + ".Export";
        public const string Schedule = Default + ".Schedule";
    }

    public static class Integrations
    {
        public const string Default = GroupName + ".Integrations";
        public const string View = Default + ".View";
        public const string Configure = Default + ".Configure";
        public const string Sync = Default + ".Sync";
    }

    public static class AI
    {
        public const string Default = GroupName + ".AI";
        public const string View = Default + ".View";
        public const string UseRecommendations = Default + ".UseRecommendations";
        public const string TrainModel = Default + ".TrainModel";
    }

    public static class Subscriptions
    {
        public const string Default = GroupName + ".Subscriptions";
        public const string View = Default + ".View";
        public const string Manage = Default + ".Manage";
    }

    public static class MyWorkspace
    {
        public const string Default = GroupName + ".MyWorkspace";
        public const string ViewTasks = Default + ".ViewTasks";
        public const string ViewNotifications = Default + ".ViewNotifications";
        public const string ManageSettings = Default + ".ManageSettings";
    }
}
