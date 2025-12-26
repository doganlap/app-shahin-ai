using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Grc.Web.Pages.ApiManagement;

[Authorize(GrcPermissions.Admin.ApiManagement)]
public class IndexModel : GrcPageModel
{
    public List<ApiKeyInfo> ApiKeys { get; set; } = new();
    public ApiUsageStats UsageStats { get; set; } = null!;
    public List<ApiEndpointInfo> Endpoints { get; set; } = new();

    public async Task OnGetAsync()
    {
        // API Keys
        ApiKeys = new List<ApiKeyInfo>
        {
            new ApiKeyInfo
            {
                Id = Guid.NewGuid(),
                Name = "Production API Key",
                KeyPrefix = "grc_prod_***",
                CreatedAt = DateTime.Now.AddMonths(-3),
                LastUsed = DateTime.Now.AddHours(-2),
                Status = "Active",
                Scopes = new[] { "read", "write" }
            },
            new ApiKeyInfo
            {
                Id = Guid.NewGuid(),
                Name = "Development API Key",
                KeyPrefix = "grc_dev_***",
                CreatedAt = DateTime.Now.AddMonths(-1),
                LastUsed = DateTime.Now.AddDays(-5),
                Status = "Active",
                Scopes = new[] { "read" }
            }
        };

        // Usage stats
        UsageStats = new ApiUsageStats
        {
            TotalRequests = 125000,
            SuccessfulRequests = 123500,
            FailedRequests = 1500,
            AverageResponseTimeMs = 145,
            RequestsToday = 2500,
            RequestsThisMonth = 45000
        };

        // API Endpoints
        Endpoints = new List<ApiEndpointInfo>
        {
            new() { Path = "/api/app/framework", Method = "GET", Description = "List all frameworks", CallsToday = 450 },
            new() { Path = "/api/app/framework/{id}", Method = "GET", Description = "Get framework by ID", CallsToday = 280 },
            new() { Path = "/api/app/assessment", Method = "GET", Description = "List assessments", CallsToday = 320 },
            new() { Path = "/api/app/assessment", Method = "POST", Description = "Create assessment", CallsToday = 45 },
            new() { Path = "/api/app/risk", Method = "GET", Description = "List risks", CallsToday = 180 },
            new() { Path = "/api/app/evidence", Method = "POST", Description = "Upload evidence", CallsToday = 95 }
        };

        await Task.CompletedTask;
    }
}

public class ApiKeyInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsed { get; set; }
    public string Status { get; set; } = "Active";
    public string[] Scopes { get; set; } = Array.Empty<string>();
}

public class ApiUsageStats
{
    public long TotalRequests { get; set; }
    public long SuccessfulRequests { get; set; }
    public long FailedRequests { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public long RequestsToday { get; set; }
    public long RequestsThisMonth { get; set; }
    public double SuccessRate => TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests * 100 : 0;
}

public class ApiEndpointInfo
{
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CallsToday { get; set; }
}
