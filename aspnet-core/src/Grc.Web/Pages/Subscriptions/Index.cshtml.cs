using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Grc.Web.Pages.Subscriptions;

[Authorize(GrcPermissions.Subscriptions.Default)]
public class IndexModel : GrcPageModel
{
    public SubscriptionInfo CurrentSubscription { get; set; } = null!;
    public List<PlanInfo> AvailablePlans { get; set; } = new();
    public UsageInfo CurrentUsage { get; set; } = null!;

    public async Task OnGetAsync()
    {
        // Current subscription
        CurrentSubscription = new SubscriptionInfo
        {
            PlanName = "Professional",
            Status = "Active",
            StartDate = DateTime.Now.AddMonths(-6),
            NextBillingDate = DateTime.Now.AddMonths(1),
            MonthlyPrice = 999.00m,
            Currency = "SAR"
        };

        // Usage data
        CurrentUsage = new UsageInfo
        {
            AssessmentsUsed = 15,
            AssessmentsLimit = 50,
            UsersUsed = 8,
            UsersLimit = 25,
            StorageUsedMB = 2500,
            StorageLimitMB = 10000,
            ApiCallsUsed = 45000,
            ApiCallsLimit = 100000
        };

        // Available plans
        AvailablePlans = new List<PlanInfo>
        {
            new PlanInfo
            {
                Name = "Starter",
                Description = "For small teams getting started",
                MonthlyPrice = 299.00m,
                Features = new[] { "10 Assessments", "5 Users", "1 GB Storage", "Email Support" }
            },
            new PlanInfo
            {
                Name = "Professional",
                Description = "For growing organizations",
                MonthlyPrice = 999.00m,
                Features = new[] { "50 Assessments", "25 Users", "10 GB Storage", "Priority Support", "API Access" },
                IsCurrentPlan = true
            },
            new PlanInfo
            {
                Name = "Enterprise",
                Description = "For large enterprises",
                MonthlyPrice = 2999.00m,
                Features = new[] { "Unlimited Assessments", "Unlimited Users", "100 GB Storage", "24/7 Support", "Custom Integrations", "SSO" }
            }
        };

        await Task.CompletedTask;
    }
}

public class SubscriptionInfo
{
    public string PlanName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public decimal MonthlyPrice { get; set; }
    public string Currency { get; set; } = "SAR";
}

public class UsageInfo
{
    public int AssessmentsUsed { get; set; }
    public int AssessmentsLimit { get; set; }
    public int UsersUsed { get; set; }
    public int UsersLimit { get; set; }
    public long StorageUsedMB { get; set; }
    public long StorageLimitMB { get; set; }
    public long ApiCallsUsed { get; set; }
    public long ApiCallsLimit { get; set; }

    public int AssessmentsPercentage => AssessmentsLimit > 0 ? (int)(AssessmentsUsed * 100.0 / AssessmentsLimit) : 0;
    public int UsersPercentage => UsersLimit > 0 ? (int)(UsersUsed * 100.0 / UsersLimit) : 0;
    public int StoragePercentage => StorageLimitMB > 0 ? (int)(StorageUsedMB * 100.0 / StorageLimitMB) : 0;
    public int ApiCallsPercentage => ApiCallsLimit > 0 ? (int)(ApiCallsUsed * 100.0 / ApiCallsLimit) : 0;
}

public class PlanInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public string[] Features { get; set; } = Array.Empty<string>();
    public bool IsCurrentPlan { get; set; }
}
