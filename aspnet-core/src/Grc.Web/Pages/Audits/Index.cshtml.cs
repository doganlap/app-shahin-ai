using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Enums;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Grc.Web.Pages.Audits;

[Authorize(GrcPermissions.Audits.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public List<AuditListItem> Audits { get; set; } = new();
    public int TotalCount { get; set; }
    public AuditSummary Summary { get; set; } = new();

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        // Query audits from database
        var audits = await _dbContext.Audits
            .Where(a => !a.IsDeleted)
            .Include(a => a.Findings)
            .OrderByDescending(a => a.CreationTime)
            .ToListAsync();

        Audits = audits.Select(a => new AuditListItem
        {
            Id = a.Id,
            Code = a.AuditId ?? $"AUD-{a.Id.ToString().Substring(0, 4).ToUpper()}",
            Title = a.Title ?? "Untitled Audit",
            Type = a.Type.ToString(),
            Status = a.Status.ToString(),
            StartDate = a.PlannedStartDate,
            EndDate = a.PlannedEndDate,
            FindingsCount = a.Findings?.Count ?? 0
        }).ToList();

        TotalCount = Audits.Count;

        // Calculate summary
        var allFindings = await _dbContext.AuditFindings
            .Where(f => f.Status != FindingStatus.Closed)
            .CountAsync();

        Summary = new AuditSummary
        {
            Total = audits.Count,
            Planned = audits.Count(a => a.Status == AuditStatus.Planned),
            InProgress = audits.Count(a => a.Status == AuditStatus.InProgress || a.Status == AuditStatus.FieldworkComplete),
            Completed = audits.Count(a => a.Status == AuditStatus.Completed || a.Status == AuditStatus.Closed),
            OpenFindings = allFindings
        };
    }
}

public class AuditListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int FindingsCount { get; set; }
}

public class AuditSummary
{
    public int Total { get; set; }
    public int Planned { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int OpenFindings { get; set; }
}
