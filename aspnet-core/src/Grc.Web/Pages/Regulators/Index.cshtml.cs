using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Grc.Web.Pages.Regulators;

[Authorize(GrcPermissions.Regulators.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public List<RegulatorListItem> Regulators { get; set; } = new();
    public int TotalCount { get; set; }

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var regulators = await _dbContext.Regulators
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Code)
                .Take(100)
                .ToListAsync();

            TotalCount = await _dbContext.Regulators.CountAsync(r => !r.IsDeleted);

            foreach (var regulator in regulators)
            {
                var frameworkCount = await _dbContext.Frameworks.CountAsync(f => f.RegulatorId == regulator.Id && !f.IsDeleted);

                Regulators.Add(new RegulatorListItem
                {
                    Id = regulator.Id,
                    Code = regulator.Code,
                    NameEn = regulator.Name?.En ?? "",
                    NameAr = regulator.Name?.Ar ?? "",
                    Jurisdiction = regulator.Jurisdiction?.En ?? "Saudi Arabia",
                    Category = regulator.Category.ToString(),
                    FrameworkCount = frameworkCount,
                    Website = regulator.Website
                });
            }
        }
        catch (Exception)
        {
            Regulators = new List<RegulatorListItem>();
            TotalCount = 0;
        }
    }
}

public class RegulatorListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Jurisdiction { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int FrameworkCount { get; set; }
    public string? Website { get; set; }
}
