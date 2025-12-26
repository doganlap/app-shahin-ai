namespace Grc.Enums;

// Organization Enums
public enum OrganizationSize
{
    Micro = 0,      // 1-9 employees
    Small = 1,      // 10-49 employees
    Medium = 2,     // 50-249 employees
    Large = 3,      // 250-999 employees
    Enterprise = 4  // 1000+ employees
}

public enum ComplianceStatus
{
    NotStarted = 0,
    InProgress = 1,
    PartiallyCompliant = 2,
    Compliant = 3,
    NonCompliant = 4,
    Expired = 5
}

// Asset Enums
public enum AssetType
{
    Hardware = 0,
    Software = 1,
    Data = 2,
    People = 3,
    Process = 4,
    Service = 5,
    Facility = 6,
    Network = 7,
    Cloud = 8,
    ThirdParty = 9
}

public enum AssetCategory
{
    Server = 0,
    Workstation = 1,
    NetworkDevice = 2,
    SecurityDevice = 3,
    MobileDevice = 4,
    Database = 5,
    Application = 6,
    OperatingSystem = 7,
    CloudService = 8,
    DataStore = 9,
    Personnel = 10,
    Contractor = 11,
    BusinessProcess = 12,
    ITProcess = 13,
    Building = 14,
    DataCenter = 15,
    Other = 99
}

public enum AssetClassification
{
    Public = 0,
    Internal = 1,
    Confidential = 2,
    Restricted = 3,
    TopSecret = 4
}

public enum AssetCriticality
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3,
    MissionCritical = 4
}

public enum AssetStatus
{
    Active = 0,
    Inactive = 1,
    InMaintenance = 2,
    Decommissioned = 3,
    Disposed = 4,
    Lost = 5,
    Stolen = 6
}

// Gap Enums
public enum GapSeverity
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum GapPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Urgent = 3
}

public enum GapStatus
{
    Open = 0,
    InProgress = 1,
    PendingVerification = 2,
    Closed = 3,
    Reopened = 4,
    Accepted = 5,
    Deferred = 6
}

// Action Item Enums
public enum ActionItemType
{
    Remediation = 0,
    Implementation = 1,
    Documentation = 2,
    Training = 3,
    Review = 4,
    Assessment = 5,
    Monitoring = 6,
    Other = 99
}

public enum ActionItemPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum ActionItemStatus
{
    NotStarted = 0,
    InProgress = 1,
    OnHold = 2,
    PendingVerification = 3,
    Completed = 4,
    Cancelled = 5,
    Overdue = 6
}

public enum ActionItemSource
{
    Gap = 0,
    Risk = 1,
    Audit = 2,
    Assessment = 3,
    Incident = 4,
    ManagementReview = 5,
    ExternalRequirement = 6,
    Other = 99
}

// Audit Enums
public enum AuditType
{
    Internal = 0,
    External = 1,
    Regulatory = 2,
    Certification = 3,
    Surveillance = 4,
    Special = 5,
    FollowUp = 6
}

public enum AuditStatus
{
    Planned = 0,
    InProgress = 1,
    FieldworkComplete = 2,
    DraftReport = 3,
    Completed = 4,
    ReportIssued = 5,
    Closed = 6,
    Cancelled = 7
}

public enum FindingSeverity
{
    Observation = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum FindingStatus
{
    Open = 0,
    ActionPlanned = 1,
    InRemediation = 2,
    PendingVerification = 3,
    Closed = 4,
    Reopened = 5,
    Accepted = 6,
    Escalated = 7
}
