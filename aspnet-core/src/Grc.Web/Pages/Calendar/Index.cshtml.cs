using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using CalendarEventEntity = Grc.Calendar.CalendarEvent;
using CalendarEventType = Grc.Calendar.CalendarEventType;
using CalendarEventPriority = Grc.Calendar.CalendarEventPriority;
using CalendarEventStatus = Grc.Calendar.CalendarEventStatus;

namespace Grc.Web.Pages.Calendar;

[Authorize(GrcPermissions.Calendar.Default)]
public class IndexModel : GrcPageModel
{
    private readonly IRepository<CalendarEventEntity, Guid> _calendarEventRepository;

    public List<CalendarEventDto> Events { get; set; } = new();
    public CalendarSummary Summary { get; set; } = new();

    public IndexModel(IRepository<CalendarEventEntity, Guid> calendarEventRepository)
    {
        _calendarEventRepository = calendarEventRepository;
    }

    public async Task OnGetAsync()
    {
        var queryable = await _calendarEventRepository.GetQueryableAsync();

        // Get calendar events from database
        var events = await queryable
            .OrderBy(e => e.StartDate)
            .Take(100)
            .ToListAsync();

        Events = events.Select(e => new CalendarEventDto
        {
            Id = e.Id,
            Title = e.Title,
            TitleAr = e.TitleAr,
            Description = e.Description,
            DescriptionAr = e.DescriptionAr,
            Type = e.Type.ToString(),
            TypeEnum = e.Type,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            DueDate = e.DueDate ?? e.StartDate,
            Priority = e.Priority.ToString(),
            PriorityEnum = e.Priority,
            Status = e.Status.ToString(),
            StatusEnum = e.Status,
            Color = e.Color ?? GetDefaultColor(e.Type),
            Icon = e.Icon ?? GetDefaultIcon(e.Type),
            IsAllDay = e.IsAllDay,
            AssignedTo = e.AssignedTo
        }).ToList();

        // Calculate summary from actual data
        var now = DateTime.UtcNow;
        Summary = new CalendarSummary
        {
            TotalEvents = await queryable.CountAsync(),
            UpcomingEvents = await queryable.CountAsync(e => e.Status == CalendarEventStatus.Upcoming),
            OverdueEvents = await queryable.CountAsync(e => e.Status == CalendarEventStatus.Overdue || (e.DueDate.HasValue && e.DueDate < now && e.Status != CalendarEventStatus.Completed)),
            CompletedEvents = await queryable.CountAsync(e => e.Status == CalendarEventStatus.Completed),
            HighPriorityEvents = await queryable.CountAsync(e => e.Priority >= CalendarEventPriority.High)
        };
    }

    private static string GetDefaultColor(CalendarEventType type)
    {
        return type switch
        {
            CalendarEventType.Assessment => "#007bff",
            CalendarEventType.AuditDue => "#dc3545",
            CalendarEventType.Review => "#17a2b8",
            CalendarEventType.Training => "#28a745",
            CalendarEventType.Deadline => "#ffc107",
            CalendarEventType.Meeting => "#6c757d",
            CalendarEventType.EvidenceCollection => "#fd7e14",
            CalendarEventType.PolicyReview => "#20c997",
            CalendarEventType.RiskAssessment => "#e83e8c",
            CalendarEventType.VendorAssessment => "#6610f2",
            CalendarEventType.CertificationExpiry => "#dc3545",
            CalendarEventType.RegulatoryDeadline => "#fd7e14",
            _ => "#6c757d"
        };
    }

    private static string GetDefaultIcon(CalendarEventType type)
    {
        return type switch
        {
            CalendarEventType.Assessment => "fa-clipboard-check",
            CalendarEventType.AuditDue => "fa-search",
            CalendarEventType.Review => "fa-eye",
            CalendarEventType.Training => "fa-graduation-cap",
            CalendarEventType.Deadline => "fa-clock",
            CalendarEventType.Meeting => "fa-users",
            CalendarEventType.EvidenceCollection => "fa-folder-open",
            CalendarEventType.PolicyReview => "fa-file-alt",
            CalendarEventType.RiskAssessment => "fa-exclamation-triangle",
            CalendarEventType.VendorAssessment => "fa-building",
            CalendarEventType.CertificationExpiry => "fa-certificate",
            CalendarEventType.RegulatoryDeadline => "fa-gavel",
            _ => "fa-calendar"
        };
    }
}

public class CalendarEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public CalendarEventType TypeEnum { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public CalendarEventPriority PriorityEnum { get; set; }
    public string Status { get; set; } = string.Empty;
    public CalendarEventStatus StatusEnum { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsAllDay { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
}

public class CalendarSummary
{
    public int TotalEvents { get; set; }
    public int UpcomingEvents { get; set; }
    public int OverdueEvents { get; set; }
    public int CompletedEvents { get; set; }
    public int HighPriorityEvents { get; set; }
}
