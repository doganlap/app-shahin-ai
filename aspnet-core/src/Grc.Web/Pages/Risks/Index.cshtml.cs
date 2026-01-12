using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Enums;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Grc.Web.Pages.Risks;

[Authorize(GrcPermissions.Risks.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public List<RiskListItem> Risks { get; set; } = new();
    public int TotalCount { get; set; }
    public RiskSummary Summary { get; set; } = new();

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var risks = await _dbContext.Risks
                .Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.CreationTime)
                .Take(100)
                .ToListAsync();

            TotalCount = await _dbContext.Risks.CountAsync(r => !r.IsDeleted);

            Summary.TotalRisks = TotalCount;
            Summary.CriticalRisks = await _dbContext.Risks.CountAsync(r => !r.IsDeleted && r.InherentRiskLevel == RiskLevel.Critical);
            Summary.HighRisks = await _dbContext.Risks.CountAsync(r => !r.IsDeleted && r.InherentRiskLevel == RiskLevel.High);
            Summary.MediumRisks = await _dbContext.Risks.CountAsync(r => !r.IsDeleted && r.InherentRiskLevel == RiskLevel.Medium);
            Summary.LowRisks = await _dbContext.Risks.CountAsync(r => !r.IsDeleted && (r.InherentRiskLevel == RiskLevel.Low || r.InherentRiskLevel == RiskLevel.VeryLow));

            foreach (var risk in risks)
            {
                Risks.Add(new RiskListItem
                {
                    Id = risk.Id,
                    Code = risk.RiskCode,
                    TitleEn = risk.Title?.En ?? "",
                    TitleAr = risk.Title?.Ar ?? "",
                    Category = risk.Category.ToString(),
                    InherentProbability = risk.InherentProbability,
                    InherentImpact = risk.InherentImpact,
                    InherentRiskLevel = risk.InherentRiskLevel.ToString(),
                    ResidualRiskLevel = risk.ResidualRiskLevel?.ToString() ?? "Not Assessed",
                    Status = risk.Status.ToString()
                });
            }
        }
        catch (Exception)
        {
            Risks = new List<RiskListItem>();
            TotalCount = 0;
        }
    }
}

public class RiskListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int InherentProbability { get; set; }
    public int InherentImpact { get; set; }
    public string InherentRiskLevel { get; set; } = string.Empty;
    public string ResidualRiskLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class RiskSummary
{
    public int TotalRisks { get; set; }
    public int CriticalRisks { get; set; }
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
}
