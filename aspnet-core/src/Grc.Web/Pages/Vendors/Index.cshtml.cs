using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using VendorEntity = Grc.Vendor.Domain.Vendors.Vendor;

namespace Grc.Web.Pages.Vendors;

[Authorize(GrcPermissions.Vendors.Default)]
public class IndexModel : GrcPageModel
{
    private readonly IRepository<VendorEntity, Guid> _vendorRepository;

    public List<VendorListItem> Vendors { get; set; } = new();
    public int TotalCount { get; set; }
    public VendorSummary Summary { get; set; } = new();

    public IndexModel(IRepository<VendorEntity, Guid> vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task OnGetAsync()
    {
        var queryable = await _vendorRepository.GetQueryableAsync();

        // Get vendors from database
        var vendors = await queryable
            .OrderBy(v => v.VendorName)
            .Take(50)
            .ToListAsync();

        Vendors = vendors.Select(v => new VendorListItem
        {
            Id = v.Id,
            Name = v.Name?.En ?? v.VendorName,
            NameAr = v.Name?.Ar ?? v.VendorName,
            Category = v.Category ?? "Uncategorized",
            RiskLevel = GetRiskLevel(v.RiskScore),
            RiskScore = v.RiskScore,
            Status = v.Status,
            LastAssessment = v.LastModificationTime ?? v.CreationTime,
            NextReview = (v.LastModificationTime ?? v.CreationTime).AddMonths(6)
        }).ToList();

        TotalCount = await queryable.CountAsync();

        // Calculate summary from actual data
        Summary = new VendorSummary
        {
            Total = TotalCount,
            Active = await queryable.CountAsync(v => v.Status == "Active"),
            Inactive = await queryable.CountAsync(v => v.Status == "Inactive"),
            PendingReview = await queryable.CountAsync(v => v.Status == "PendingReview"),
            HighRisk = await queryable.CountAsync(v => v.RiskScore >= 4)
        };
    }

    private static string GetRiskLevel(int riskScore)
    {
        return riskScore switch
        {
            <= 1 => "Low",
            <= 2 => "Medium",
            <= 3 => "High",
            _ => "Critical"
        };
    }
}

public class VendorListItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? LastAssessment { get; set; }
    public DateTime? NextReview { get; set; }
}

public class VendorSummary
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public int PendingReview { get; set; }
    public int HighRisk { get; set; }
}
