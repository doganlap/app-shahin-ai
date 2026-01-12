using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Evidence;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Grc.Web.Pages.Evidence;

[Authorize(GrcPermissions.Evidence.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;
    private readonly IEvidenceAppService _evidenceAppService;

    public List<EvidenceListItem> Evidences { get; set; } = new();
    public int TotalCount { get; set; }
    public EvidenceSummary Summary { get; set; } = new();

    public IndexModel(GrcDbContext dbContext, IEvidenceAppService evidenceAppService)
    {
        _dbContext = dbContext;
        _evidenceAppService = evidenceAppService;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var evidences = await _dbContext.Evidences
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.CreationTime)
                .Take(50)
                .ToListAsync();

            TotalCount = await _dbContext.Evidences.CountAsync(e => !e.IsDeleted);

            // Calculate summary
            Summary.TotalFiles = TotalCount;
            Summary.TotalSizeMB = await _dbContext.Evidences
                .Where(e => !e.IsDeleted)
                .SumAsync(e => e.FileSize) / (1024.0 * 1024.0);

            foreach (var evidence in evidences)
            {
                var extension = System.IO.Path.GetExtension(evidence.FileName)?.ToLower() ?? "unknown";
                Evidences.Add(new EvidenceListItem
                {
                    Id = evidence.Id,
                    FileName = evidence.FileName,
                    FileSize = evidence.FileSize / (1024.0m * 1024.0m),
                    FileType = extension,
                    Description = evidence.Description ?? "",
                    UploadedBy = "User",
                    UploadDate = evidence.CreationTime,
                    LinkedTo = evidence.ControlAssessmentId.HasValue ? "Control Assessment" : "Unlinked",
                    Tags = new[] { evidence.EvidenceType.ToString() }
                });
            }
        }
        catch (Exception)
        {
            // Fallback to empty list
            Evidences = new List<EvidenceListItem>();
            TotalCount = 0;
        }
    }

    public async Task<IActionResult> OnPostUploadAsync(IFormFile file, string description, Guid? controlAssessmentId)
    {
        // Check upload permission manually (MVC1001: can't use [Authorize] on handler methods)
        if (!await AuthorizationService.IsGrantedAsync(GrcPermissions.Evidence.Upload))
        {
            return new ForbidResult();
        }
        
        try
        {
            var evidence = await _evidenceAppService.UploadAsync(controlAssessmentId, file, description);
            return new JsonResult(new { success = true, message = L["Evidence:UploadSuccess"].Value });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to upload evidence");
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }
}

public class EvidenceListItem
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public decimal FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string LinkedTo { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
}

public class EvidenceSummary
{
    public int TotalFiles { get; set; }
    public double TotalSizeMB { get; set; }
}
