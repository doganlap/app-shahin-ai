using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Calendar;

/// <summary>
/// Calendar event entity for compliance calendar
/// </summary>
public class CalendarEvent : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Title { get; private set; }
    public string TitleAr { get; private set; }
    public string Description { get; private set; }
    public string DescriptionAr { get; private set; }
    public CalendarEventType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public CalendarEventPriority Priority { get; private set; }
    public CalendarEventStatus Status { get; private set; }
    public string Color { get; private set; }
    public string Icon { get; private set; }
    public bool IsAllDay { get; private set; }
    public bool IsRecurring { get; private set; }
    public string RecurrenceRule { get; private set; }
    public Guid? AssessmentId { get; private set; }
    public Guid? FrameworkId { get; private set; }
    public Guid? ControlId { get; private set; }
    public Guid? AuditId { get; private set; }
    public string AssignedTo { get; private set; }
    public string AssignedToEmail { get; private set; }
    public int ReminderDays { get; private set; }
    public bool ReminderSent { get; private set; }
    public string Notes { get; private set; }

    protected CalendarEvent() { }

    public CalendarEvent(
        Guid id,
        string title,
        CalendarEventType type,
        DateTime startDate,
        CalendarEventPriority priority = CalendarEventPriority.Medium,
        Guid? tenantId = null)
        : base(id)
    {
        Title = title;
        Type = type;
        StartDate = startDate;
        Priority = priority;
        Status = CalendarEventStatus.Upcoming;
        TenantId = tenantId;
        ReminderDays = 7;
    }

    public void SetArabicDetails(string titleAr, string descriptionAr)
    {
        TitleAr = titleAr;
        DescriptionAr = descriptionAr;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void SetDates(DateTime startDate, DateTime? endDate, DateTime? dueDate)
    {
        StartDate = startDate;
        EndDate = endDate;
        DueDate = dueDate;
    }

    public void SetAppearance(string color, string icon)
    {
        Color = color;
        Icon = icon;
    }

    public void SetRecurrence(bool isRecurring, string recurrenceRule)
    {
        IsRecurring = isRecurring;
        RecurrenceRule = recurrenceRule;
    }

    public void LinkToAssessment(Guid assessmentId)
    {
        AssessmentId = assessmentId;
    }

    public void LinkToFramework(Guid frameworkId)
    {
        FrameworkId = frameworkId;
    }

    public void LinkToControl(Guid controlId)
    {
        ControlId = controlId;
    }

    public void LinkToAudit(Guid auditId)
    {
        AuditId = auditId;
    }

    public void AssignTo(string name, string email)
    {
        AssignedTo = name;
        AssignedToEmail = email;
    }

    public void SetReminder(int days)
    {
        ReminderDays = days;
        ReminderSent = false;
    }

    public void MarkReminderSent()
    {
        ReminderSent = true;
    }

    public void UpdateStatus(CalendarEventStatus status)
    {
        Status = status;
    }

    public void Complete()
    {
        Status = CalendarEventStatus.Completed;
    }

    public void Cancel()
    {
        Status = CalendarEventStatus.Cancelled;
    }

    public void SetNotes(string notes)
    {
        Notes = notes;
    }
}

public enum CalendarEventType
{
    Assessment = 0,
    AuditDue = 1,
    Review = 2,
    Training = 3,
    Deadline = 4,
    Meeting = 5,
    EvidenceCollection = 6,
    PolicyReview = 7,
    RiskAssessment = 8,
    VendorAssessment = 9,
    CertificationExpiry = 10,
    RegulatoryDeadline = 11,
    Other = 99
}

public enum CalendarEventPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum CalendarEventStatus
{
    Upcoming = 0,
    InProgress = 1,
    Completed = 2,
    Overdue = 3,
    Cancelled = 4
}
