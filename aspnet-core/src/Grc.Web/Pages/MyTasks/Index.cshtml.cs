using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grc.EntityFrameworkCore;
using Grc.Permissions;

namespace Grc.Web.Pages.MyTasks;

[Authorize(GrcPermissions.MyWorkspace.ViewTasks)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;
    private readonly Volo.Abp.Users.ICurrentUser _currentUser;

    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<TaskDto> Tasks { get; set; } = new();

    public IndexModel(GrcDbContext dbContext, Volo.Abp.Users.ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task OnGetAsync()
    {
        // Get tasks from audit logs of current user
        if (!_currentUser.Id.HasValue)
        {
            return;
        }

        var userId = _currentUser.Id.Value;
        var auditLogs = await _dbContext.Set<Volo.Abp.AuditLogging.AuditLog>()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.ExecutionTime)
            .Take(50)
            .ToListAsync();

        PendingTasks = auditLogs.Count(a => a.ExecutionTime > DateTime.UtcNow.AddDays(-7));
        InProgressTasks = auditLogs.Count(a => a.ExecutionTime > DateTime.UtcNow.AddDays(-3) && a.ExecutionTime <= DateTime.UtcNow.AddDays(-7));
        CompletedTasks = auditLogs.Count(a => a.ExecutionTime <= DateTime.UtcNow.AddDays(-7));
        OverdueTasks = 0;
    }
}

public class TaskDto
{
    public string Title { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "";
    public string Status { get; set; } = "";
    public string PriorityColor { get; set; } = "";
    public string StatusColor { get; set; } = "";
}

