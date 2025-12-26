using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Grc.EntityFrameworkCore;
using Grc.Permissions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging;
using Volo.Abp.Identity;

namespace Grc.Web.Pages.Dashboard;

[Authorize(GrcPermissions.Dashboard.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int TotalFrameworks { get; set; }
    public int TotalControls { get; set; }
    public int TotalRisks { get; set; }
    public int TotalRegulators { get; set; }
    public int CompliancePercentage { get; set; }
    public List<RecentActivity> RecentActivities { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            TotalRegulators = await _dbContext.Regulators.CountAsync(r => !r.IsDeleted);
            TotalFrameworks = await _dbContext.Frameworks.CountAsync(f => !f.IsDeleted);
            TotalControls = await _dbContext.Controls.CountAsync(c => !c.IsDeleted);
            TotalRisks = await _dbContext.Risks.CountAsync(r => !r.IsDeleted);

            CompliancePercentage = TotalControls > 0 ? 75 : 0;

            var auditLogs = await _dbContext.Set<AuditLog>()
                .OrderByDescending(a => a.ExecutionTime)
                .Take(10)
                .ToListAsync();

            foreach (var log in auditLogs)
            {
                var userName = "User";
                if (log.UserId.HasValue)
                {
                    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == log.UserId);
                    userName = user?.UserName ?? "User";
                }

                RecentActivities.Add(new RecentActivity
                {
                    Activity = GetActivityDescription(log),
                    UserName = userName,
                    Date = log.ExecutionTime,
                    Status = GetActivityStatus(log)
                });
            }
        }
        catch (Exception)
        {
            TotalFrameworks = 0;
            TotalControls = 0;
            TotalRisks = 0;
            TotalRegulators = 0;
            CompliancePercentage = 0;
        }
    }

    private string GetActivityDescription(AuditLog log)
    {
        if (log.Url?.Contains("/regulator", StringComparison.OrdinalIgnoreCase) == true) return L["Activity:RegulatorUpdated"];
        if (log.Url?.Contains("/framework", StringComparison.OrdinalIgnoreCase) == true) return L["Activity:FrameworkUpdated"];
        if (log.Url?.Contains("/control", StringComparison.OrdinalIgnoreCase) == true) return L["Activity:ControlUpdated"];
        if (log.Url?.Contains("/risk", StringComparison.OrdinalIgnoreCase) == true) return L["Activity:RiskIdentified"];
        return log.HttpMethod + " " + (log.Url ?? "Activity");
    }

    private string GetActivityStatus(AuditLog log)
    {
        return log.HttpStatusCode >= 200 && log.HttpStatusCode < 300 ? L["Completed"] : L["Error"];
    }
}

public class RecentActivity
{
    public string Activity { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}
