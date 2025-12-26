using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Calendar;

/// <summary>
/// Calendar event for compliance deadlines and reminders
/// </summary>
public class CalendarEvent : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string EventType { get; set; } // Deadline, Reminder, Meeting, Assessment
    public Guid? RelatedEntityId { get; set; }
    public string RelatedEntityType { get; set; } // Assessment, ControlAssessment, Risk, Policy
    public List<Guid> AttendeeUserIds { get; set; }
    public bool IsAllDay { get; set; }
    public string Location { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    
    protected CalendarEvent() { }
    
    public CalendarEvent(Guid id, string title, DateTime startDate, string eventType)
        : base(id)
    {
        Title = title;
        StartDate = startDate;
        EventType = eventType;
        AttendeeUserIds = new List<Guid>();
        Metadata = new Dictionary<string, object>();
    }
    
    public bool IsOverdue => StartDate < DateTime.UtcNow && EndDate == null;
}

