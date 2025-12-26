using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Grc.Web.Pages.FrameworkLibrary;

[Authorize(GrcPermissions.Frameworks.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public List<FrameworkListItem> Frameworks { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalRegulators { get; set; }
    public int TotalControls { get; set; }
    public int ActiveFrameworks { get; set; }

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        try
        {
            // Load statistics
            TotalCount = await _dbContext.Frameworks.CountAsync(f => !f.IsDeleted);
            TotalRegulators = await _dbContext.Regulators.CountAsync(r => !r.IsDeleted);
            TotalControls = await _dbContext.Controls.CountAsync(c => !c.IsDeleted);
            ActiveFrameworks = await _dbContext.Frameworks.CountAsync(f => !f.IsDeleted && f.Status == Grc.Enums.FrameworkStatus.Active);

            // Use raw SQL to avoid owned type navigation issues
            var frameworkData = await _dbContext.Database
                .SqlQuery<FrameworkQueryResult>($@"
                    SELECT f.""Id"", f.""Code"", f.""TitleEn"", f.""TitleAr"",
                           f.""Category"", f.""IsMandatory"", f.""EffectiveDate"", f.""Status"",
                           r.""NameEn"" as ""RegulatorNameEn"",
                           (SELECT COUNT(*) FROM ""Controls"" c WHERE c.""FrameworkId"" = f.""Id"" AND c.""IsDeleted"" = false) as ""ControlCount""
                    FROM ""Frameworks"" f
                    LEFT JOIN ""Regulators"" r ON f.""RegulatorId"" = r.""Id""
                    WHERE f.""IsDeleted"" = false
                    ORDER BY f.""Code""
                    LIMIT 100")
                .ToListAsync();

            foreach (var data in frameworkData)
            {
                Frameworks.Add(new FrameworkListItem
                {
                    Id = data.Id,
                    Code = data.Code,
                    TitleEn = data.TitleEn ?? "",
                    TitleAr = data.TitleAr ?? "",
                    RegulatorName = data.RegulatorNameEn ?? "N/A",
                    Category = ((Grc.Enums.FrameworkCategory)data.Category).ToString(),
                    ControlCount = data.ControlCount,
                    IsMandatory = data.IsMandatory,
                    EffectiveDate = data.EffectiveDate,
                    Status = ((Grc.Enums.FrameworkStatus)data.Status).ToString()
                });
            }
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            System.Diagnostics.Debug.WriteLine($"Error loading frameworks: {ex.Message}");
            Frameworks = new List<FrameworkListItem>();
            TotalCount = 0;
        }
    }
}

public class FrameworkQueryResult
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public int Category { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime EffectiveDate { get; set; }
    public int Status { get; set; }
    public string? RegulatorNameEn { get; set; }
    public int ControlCount { get; set; }
}

public class FrameworkListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string RegulatorName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ControlCount { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
