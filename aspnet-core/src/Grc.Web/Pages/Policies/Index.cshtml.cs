using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Grc.Web.Pages.Policies;

[Authorize(GrcPermissions.Policies.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public List<PolicyListItem> Policies { get; set; } = new();
    public int TotalCount { get; set; }
    public PolicySummary Summary { get; set; } = new();

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        // Query policies from database
        var policies = await _dbContext.Policies
            .Where(p => !p.IsDeleted)
            .Include(p => p.Versions)
            .OrderByDescending(p => p.CreationTime)
            .ToListAsync();

        Policies = policies.Select(p => new PolicyListItem
        {
            Id = p.Id,
            Code = p.PolicyCode,
            TitleEn = p.Title?.En ?? "",
            TitleAr = p.Title?.Ar ?? "",
            Owner = "System",
            EffectiveDate = p.EffectiveDate,
            IsActive = p.IsActive,
            CurrentVersion = p.Versions?.FirstOrDefault(v => v.IsCurrentVersion)?.VersionNumber ?? "1.0"
        }).ToList();

        TotalCount = Policies.Count;

        // Calculate summary
        Summary = new PolicySummary
        {
            Total = TotalCount,
            Active = policies.Count(p => p.IsActive),
            Draft = policies.Count(p => !p.IsActive && p.EffectiveDate > DateTime.UtcNow),
            UnderReview = policies.Count(p => !p.IsActive && p.EffectiveDate <= DateTime.UtcNow)
        };
    }
}

public class PolicyListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public DateTime? EffectiveDate { get; set; }
    public bool IsActive { get; set; }
    public string CurrentVersion { get; set; } = string.Empty;
}

public class PolicySummary
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Draft { get; set; }
    public int UnderReview { get; set; }
}
