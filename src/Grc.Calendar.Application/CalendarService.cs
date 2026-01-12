using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Calendar;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace Grc.Calendar.Application;

/// <summary>
/// Calendar service for deadline and reminder management
/// </summary>
public class CalendarService
{
    private readonly IRepository<CalendarEvent, Guid> _calendarRepository;
    private readonly ILogger<CalendarService> _logger;

    public CalendarService(
        IRepository<CalendarEvent, Guid> calendarRepository,
        ILogger<CalendarService> logger)
    {
        _calendarRepository = calendarRepository;
        _logger = logger;
    }

    /// <summary>
    /// Create calendar event
    /// </summary>
    public async Task<CalendarEvent> CreateEventAsync(
        string title,
        DateTime startDate,
        string eventType,
        Guid? tenantId = null,
        DateTime? endDate = null,
        string description = null)
    {
        var calendarEvent = new CalendarEvent(
            Guid.NewGuid(),
            title,
            startDate,
            eventType)
        {
            TenantId = tenantId,
            EndDate = endDate,
            Description = description
        };
        
        await _calendarRepository.InsertAsync(calendarEvent);
        return calendarEvent;
    }

    /// <summary>
    /// Get upcoming events for user
    /// </summary>
    public async Task<List<CalendarEvent>> GetUpcomingEventsAsync(Guid userId, int daysAhead = 30)
    {
        var fromDate = DateTime.UtcNow;
        var toDate = fromDate.AddDays(daysAhead);
        
        var events = await _calendarRepository.GetListAsync(e => 
            (e.StartDate >= fromDate && e.StartDate <= toDate) &&
            (e.AttendeeUserIds.Contains(userId) || e.AttendeeUserIds.Count == 0));
        
        return events.OrderBy(e => e.StartDate).ToList();
    }

    /// <summary>
    /// Get overdue events
    /// </summary>
    public async Task<List<CalendarEvent>> GetOverdueEventsAsync(Guid? tenantId = null)
    {
        var events = await _calendarRepository.GetListAsync(e => 
            e.StartDate < DateTime.UtcNow &&
            (tenantId == null || e.TenantId == tenantId));
        
        return events.Where(e => e.IsOverdue).ToList();
    }

    /// <summary>
    /// Create reminder for a deadline
    /// </summary>
    public async Task CreateReminderAsync(
        string title,
        DateTime deadline,
        Guid userId,
        int reminderDaysBefore = 7,
        Guid? tenantId = null)
    {
        var reminderDate = deadline.AddDays(-reminderDaysBefore);
        
        if (reminderDate > DateTime.UtcNow)
        {
            var reminder = new CalendarEvent(
                Guid.NewGuid(),
                $"Reminder: {title}",
                reminderDate,
                "Reminder")
            {
                TenantId = tenantId,
                EndDate = deadline,
                Description = title,
                AttendeeUserIds = new List<Guid> { userId }
            };
            
            await _calendarRepository.InsertAsync(reminder);
            _logger.LogInformation("Created reminder for {Title} on {ReminderDate}", title, reminderDate);
        }
    }
}

