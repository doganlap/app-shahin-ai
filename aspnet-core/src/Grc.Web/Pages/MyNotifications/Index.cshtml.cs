using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grc.EntityFrameworkCore;
using Grc.Permissions;
using Volo.Abp.Users;

namespace Grc.Web.Pages.MyNotifications;

[Authorize(GrcPermissions.MyWorkspace.ViewNotifications)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public List<NotificationDto> Notifications { get; set; } = new();

    public IndexModel(GrcDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task OnGetAsync()
    {
        // Get audit logs for current user as "notifications"
        var userId = _currentUser.Id;
        
        if (!userId.HasValue)
        {
            return;
        }
        
        var auditLogs = await _dbContext.Set<Volo.Abp.AuditLogging.AuditLog>()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.ExecutionTime)
            .Take(50)
            .ToListAsync();

        foreach (var log in auditLogs)
        {
            Notifications.Add(new NotificationDto
            {
                Id = log.Id,
                Message = GetNotificationMessage(log),
                CreatedDate = log.ExecutionTime,
                IsRead = log.ExecutionTime < DateTime.UtcNow.AddDays(-7)
            });
        }
    }

    private string GetNotificationMessage(Volo.Abp.AuditLogging.AuditLog log)
    {
        var method = log.HttpMethod;
        var url = log.Url ?? "";

        if (url.Contains("/framework")) return $"Framework activity: {method} - {log.HttpStatusCode}";
        if (url.Contains("/regulator")) return $"Regulator activity: {method} - {log.HttpStatusCode}";
        if (url.Contains("/risk")) return $"Risk activity: {method} - {log.HttpStatusCode}";
        if (url.Contains("/control")) return $"Control activity: {method} - {log.HttpStatusCode}";
        if (url.Contains("/vendor")) return $"Vendor activity: {method} - {log.HttpStatusCode}";

        return $"System activity: {method} {url}";
    }
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = "";
    public DateTime CreatedDate { get; set; }
    public bool IsRead { get; set; }
}

