using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Enums;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Grc.Web.Pages.ActionPlans;

[Authorize(GrcPermissions.ActionPlans.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public List<ActionPlanListItem> ActionPlans { get; set; } = new();
    public int TotalCount { get; set; }
    public ActionPlanSummary Summary { get; set; } = new();

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        // Query action plans from database
        var actionPlans = await _dbContext.ActionPlans
            .Where(ap => !ap.IsDeleted)
            .Include(ap => ap.ActionItems)
            .OrderByDescending(ap => ap.CreationTime)
            .ToListAsync();

        ActionPlans = actionPlans.Select(ap => {
            var completedItems = ap.ActionItems?.Count(i => i.Status == WorkflowStatus.Approved) ?? 0;
            var totalItems = ap.ActionItems?.Count ?? 0;
            var progress = totalItems > 0 ? (completedItems * 100 / totalItems) : 0;
            var isOverdue = ap.TargetEndDate < DateTime.UtcNow && ap.Status != WorkflowStatus.Approved;

            return new ActionPlanListItem
            {
                Id = ap.Id,
                Code = $"AP-{ap.Id.ToString().Substring(0, 4).ToUpper()}",
                Title = ap.Name,
                Status = isOverdue ? "Overdue" : ap.Status.ToString(),
                Priority = GetPriorityFromItems(ap.ActionItems),
                DueDate = ap.TargetEndDate,
                Progress = progress,
                AssignedTo = "Team"
            };
        }).ToList();

        TotalCount = ActionPlans.Count;

        // Calculate summary
        var now = DateTime.UtcNow;
        Summary = new ActionPlanSummary
        {
            Total = actionPlans.Count,
            Open = actionPlans.Count(ap => ap.Status == WorkflowStatus.Pending),
            InProgress = actionPlans.Count(ap => ap.Status == WorkflowStatus.InProgress),
            Completed = actionPlans.Count(ap => ap.Status == WorkflowStatus.Approved),
            Overdue = actionPlans.Count(ap => ap.TargetEndDate < now && ap.Status != WorkflowStatus.Approved)
        };
    }

    private string GetPriorityFromItems(ICollection<ActionPlan.ActionItem>? items)
    {
        if (items == null || !items.Any()) return "Medium";
        var maxPriority = items.Max(i => (int)i.Priority);
        return maxPriority switch
        {
            >= 2 => "High",
            >= 1 => "Medium",
            _ => "Low"
        };
    }
}

public class ActionPlanListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int Progress { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
}

public class ActionPlanSummary
{
    public int Total { get; set; }
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Overdue { get; set; }
}
