using GrcMvc.Messaging.Messages;
using GrcMvc.Data;
using Microsoft.EntityFrameworkCore;

namespace GrcMvc.Services.Analytics
{
    /// <summary>
    /// Stub implementation of ClickHouse service when ClickHouse is disabled
    /// Falls back to PostgreSQL queries for analytics data
    /// </summary>
    public class StubClickHouseService : IClickHouseService
    {
        private readonly ILogger<StubClickHouseService> _logger;
        private readonly GrcDbContext _dbContext;

        public StubClickHouseService(
            ILogger<StubClickHouseService> logger,
            GrcDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _logger.LogInformation("Using stub ClickHouse service with PostgreSQL fallback - ClickHouse is disabled");
        }

        public async Task<DashboardSnapshotDto?> GetLatestSnapshotAsync(Guid tenantId)
        {
            try
            {
                // Query PostgreSQL for current snapshot data
                var requirements = await _dbContext.AssessmentRequirements
                    .Include(r => r.Assessment)
                    .Where(r => r.Assessment != null && r.Assessment.TenantId == tenantId && !r.IsDeleted)
                    .ToListAsync();

                var risks = await _dbContext.Risks
                    .Where(r => r.TenantId == tenantId && !r.IsDeleted)
                    .ToListAsync();

                var evidences = await _dbContext.Evidences
                    .Where(e => e.TenantId == tenantId && !e.IsDeleted)
                    .ToListAsync();

                var now = DateTime.UtcNow;
                var snapshotHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);

                return new DashboardSnapshotDto
                {
                    TenantId = tenantId,
                    SnapshotDate = snapshotHour.Date,
                    SnapshotHour = snapshotHour,
                    TotalControls = requirements.Count,
                    CompliantControls = requirements.Count(r => r.Status == "Compliant"),
                    PartialControls = requirements.Count(r => r.Status == "PartiallyCompliant"),
                    NonCompliantControls = requirements.Count(r => r.Status == "NonCompliant"),
                    TotalRisks = risks.Count,
                    HighRisks = risks.Count(r => r.Impact >= 4 || r.RiskScore >= 16), // High impact (4-5) or high score
                    TotalEvidence = evidences.Count,
                    CreatedAt = now,
                    UpdatedAt = now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying PostgreSQL for dashboard snapshot");
                return null;
            }
        }

        public Task<List<DashboardSnapshotDto>> GetSnapshotHistoryAsync(Guid tenantId, DateTime from, DateTime to)
        {
            // For history, return current snapshot only (full history would require aggregation)
            var snapshot = GetLatestSnapshotAsync(tenantId).Result;
            return Task.FromResult(snapshot != null ? new List<DashboardSnapshotDto> { snapshot } : new List<DashboardSnapshotDto>());
        }

        public Task UpsertSnapshotAsync(DashboardSnapshotDto snapshot) => Task.CompletedTask;

        public async Task<List<ComplianceTrendDto>> GetComplianceTrendsAsync(Guid tenantId, int months = 12)
        {
            try
            {
                var fromDate = DateTime.UtcNow.AddMonths(-months);
                var requirements = await _dbContext.AssessmentRequirements
                    .Include(r => r.Assessment)
                    .Where(r => r.Assessment != null && 
                               r.Assessment.TenantId == tenantId && 
                               !r.IsDeleted &&
                               r.Assessment.CreatedDate >= fromDate)
                    .ToListAsync();

                // Group by month and framework
                var trends = requirements
                    .GroupBy(r => new
                    {
                        Year = r.Assessment!.CreatedDate.Year,
                        Month = r.Assessment.CreatedDate.Month,
                        Framework = r.Assessment.FrameworkCode ?? "Unknown"
                    })
                    .Select(g => new ComplianceTrendDto
                    {
                        TenantId = tenantId,
                        FrameworkCode = g.Key.Framework,
                        BaselineCode = g.Key.Framework,
                        MeasureDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                        MeasureHour = new DateTime(g.Key.Year, g.Key.Month, 1),
                        TotalControls = g.Count(),
                        CompliantControls = g.Count(r => r.Status == "Compliant"),
                        PartialControls = g.Count(r => r.Status == "PartiallyCompliant"),
                        NonCompliantControls = g.Count(r => r.Status == "NonCompliant"),
                        ComplianceScore = g.Any() ? (decimal)g.Average(r => r.Score ?? 0) : 0,
                        DeltaFromPrevious = 0 // Would need previous month data to calculate
                    })
                    .OrderBy(t => t.MeasureDate)
                    .ToList();

                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying PostgreSQL for compliance trends");
                return new List<ComplianceTrendDto>();
            }
        }

        public async Task<List<ComplianceTrendDto>> GetComplianceTrendsByFrameworkAsync(Guid tenantId, string frameworkCode, int months = 12)
        {
            var allTrends = await GetComplianceTrendsAsync(tenantId, months);
            return allTrends.Where(t => t.FrameworkCode == frameworkCode).ToList();
        }

        public Task UpsertComplianceTrendAsync(ComplianceTrendDto trend) => Task.CompletedTask;

        public async Task<List<RiskHeatmapCell>> GetRiskHeatmapAsync(Guid tenantId)
        {
            try
            {
                var risks = await _dbContext.Risks
                    .Where(r => r.TenantId == tenantId && !r.IsDeleted)
                    .ToListAsync();

                // Group by likelihood and impact (Risk.Likelihood and Risk.Impact are already int: 1-5)
                var heatmap = risks
                    .GroupBy(r => new 
                    { 
                        Likelihood = r.Likelihood > 0 ? r.Likelihood : 3, // Default to medium if 0
                        Impact = r.Impact > 0 ? r.Impact : 3 // Default to medium if 0
                    })
                    .Select(g => new RiskHeatmapCell
                    {
                        Likelihood = g.Key.Likelihood,
                        Impact = g.Key.Impact,
                        RiskCount = g.Count(),
                        RiskIds = g.Select(r => r.Id).ToList()
                    })
                    .ToList();

                return heatmap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying PostgreSQL for risk heatmap");
                return new List<RiskHeatmapCell>();
            }
        }


        public Task UpsertRiskHeatmapAsync(Guid tenantId, List<RiskHeatmapCell> cells) => Task.CompletedTask;

        public async Task<List<FrameworkComparisonDto>> GetFrameworkComparisonAsync(Guid tenantId)
        {
            try
            {
                var requirements = await _dbContext.AssessmentRequirements
                    .Include(r => r.Assessment)
                    .Where(r => r.Assessment != null && r.Assessment.TenantId == tenantId && !r.IsDeleted)
                    .ToListAsync();

                var comparison = requirements
                    .GroupBy(r => r.Assessment!.FrameworkCode ?? "Unknown")
                    .Select(g => new FrameworkComparisonDto
                    {
                        TenantId = tenantId,
                        SnapshotDate = DateTime.UtcNow.Date,
                        FrameworkCode = g.Key,
                        FrameworkName = g.Key, // Would need to lookup from catalog
                        TotalRequirements = g.Count(),
                        CompliantCount = g.Count(r => r.Status == "Compliant"),
                        PartialCount = g.Count(r => r.Status == "PartiallyCompliant"),
                        NonCompliantCount = g.Count(r => r.Status == "NonCompliant"),
                        NotAssessedCount = g.Count(r => r.Status == "NotStarted" || string.IsNullOrEmpty(r.Status)),
                        ComplianceScore = g.Any() ? (decimal)g.Average(r => r.Score ?? 0) : 0,
                        MaturityLevel = 0, // Would need maturity calculation
                        Trend7d = 0,
                        Trend30d = 0
                    })
                    .ToList();

                return comparison;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying PostgreSQL for framework comparison");
                return new List<FrameworkComparisonDto>();
            }
        }

        public Task UpsertFrameworkComparisonAsync(FrameworkComparisonDto comparison) => Task.CompletedTask;

        public async Task<List<TaskMetricsByRoleDto>> GetTaskMetricsByRoleAsync(Guid tenantId)
        {
            try
            {
                var tasks = await _dbContext.WorkflowTasks
                    .Include(t => t.WorkflowInstance)
                    .Where(t => t.WorkflowInstance != null && 
                               t.WorkflowInstance.TenantId == tenantId && 
                               !t.IsDeleted)
                    .ToListAsync();

                // Group by assigned role (would need to get from user/team membership)
                // For now, group by assigned user and map to role
                var metrics = tasks
                    .GroupBy(t => "DefaultRole") // TODO: Get actual role from user/team
                    .Select(g => new TaskMetricsByRoleDto
                    {
                        TenantId = tenantId,
                        SnapshotDate = DateTime.UtcNow.Date,
                        RoleCode = g.Key,
                        TeamId = Guid.Empty,
                        TotalTasks = g.Count(),
                        CompletedTasks = g.Count(t => t.Status == "Completed"),
                        PendingTasks = g.Count(t => t.Status == "Pending"),
                        InProgressTasks = g.Count(t => t.Status == "InProgress"),
                        OverdueTasks = g.Count(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && t.Status != "Completed"),
                        AvgCompletionDays = 0,
                        SlaComplianceRate = 0
                    })
                    .ToList();

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying PostgreSQL for task metrics");
                return new List<TaskMetricsByRoleDto>();
            }
        }

        public Task UpsertTaskMetricsByRoleAsync(TaskMetricsByRoleDto metrics) => Task.CompletedTask;

        public async Task<List<EvidenceMetricsDto>> GetEvidenceMetricsAsync(Guid tenantId)
        {
            try
            {
                var evidences = await _dbContext.Evidences
                    .Where(e => e.TenantId == tenantId && !e.IsDeleted)
                    .ToListAsync();

                var metrics = new List<EvidenceMetricsDto>
                {
                    new EvidenceMetricsDto
                    {
                        TenantId = tenantId,
                        SnapshotDate = DateTime.UtcNow.Date,
                        EvidenceType = "All",
                        ControlDomain = "All",
                        TotalRequired = 0, // Would need to query requirements
                        TotalCollected = evidences.Count,
                        TotalApproved = evidences.Count(e => e.VerificationStatus == "Approved"),
                        TotalRejected = evidences.Count(e => e.VerificationStatus == "Rejected"),
                        TotalExpired = 0, // Would need expiry date logic
                        CollectionRate = 0,
                        ApprovalRate = evidences.Any() ? (decimal)evidences.Count(e => e.VerificationStatus == "Approved") / evidences.Count * 100 : 0,
                        AvgReviewDays = 0
                    }
                };

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying PostgreSQL for evidence metrics");
                return new List<EvidenceMetricsDto>();
            }
        }

        public Task UpsertEvidenceMetricsAsync(EvidenceMetricsDto metrics) => Task.CompletedTask;

        public async Task<List<TopActionDto>> GetTopActionsAsync(Guid tenantId, int limit = 10)
        {
            try
            {
                var tasks = await _dbContext.WorkflowTasks
                    .Include(t => t.WorkflowInstance)
                    .Where(t => t.WorkflowInstance != null && 
                               t.WorkflowInstance.TenantId == tenantId && 
                               !t.IsDeleted &&
                               (t.Status == "Pending" || t.Status == "InProgress"))
                    .OrderBy(t => t.Priority) // Priority is int: 1=High, 2=Medium, 3=Low
                    .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                    .Take(limit)
                    .ToListAsync();

                return tasks.Select((t, index) => new TopActionDto
                {
                    TenantId = tenantId,
                    SnapshotDate = DateTime.UtcNow.Date,
                    ActionRank = index + 1,
                    ActionType = "WorkflowTask",
                    ActionTitle = t.TaskName ?? "Untitled Task",
                    ActionDescription = t.Description ?? string.Empty,
                    EntityType = "WorkflowTask",
                    EntityId = t.Id,
                    Urgency = t.Priority == 1 ? "High" : t.Priority == 2 ? "Medium" : "Low",
                    DueDate = t.DueDate,
                    AssignedTo = t.AssignedToUserName ?? "Unassigned"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying PostgreSQL for top actions");
                return new List<TopActionDto>();
            }
        }

        public Task UpsertTopActionsAsync(Guid tenantId, List<TopActionDto> actions) => Task.CompletedTask;

        public Task<List<UserActivityDto>> GetUserActivityAsync(Guid tenantId, DateTime from, DateTime to)
        {
            // User activity would require audit log queries - return empty for now
            return Task.FromResult(new List<UserActivityDto>());
        }

        public Task UpsertUserActivityAsync(UserActivityDto activity) => Task.CompletedTask;

        public Task InsertEventAsync(AnalyticsEventDto analyticsEvent) => Task.CompletedTask;

        public Task<List<AnalyticsEventDto>> GetRecentEventsAsync(Guid tenantId, int limit = 100)
        {
            return Task.FromResult(new List<AnalyticsEventDto>());
        }

        public Task<bool> IsHealthyAsync() => Task.FromResult(true); // PostgreSQL is available
    }

    /// <summary>
    /// Stub implementation of Dashboard Projector when ClickHouse is disabled
    /// Does nothing - dashboards will query PostgreSQL directly
    /// </summary>
    public class StubDashboardProjector : IDashboardProjector
    {
        private readonly ILogger<StubDashboardProjector> _logger;

        public StubDashboardProjector(ILogger<StubDashboardProjector> logger)
        {
            _logger = logger;
            _logger.LogInformation("Using stub dashboard projector - ClickHouse is disabled");
        }

        public Task ProjectSnapshotAsync(Guid tenantId) => Task.CompletedTask;
        public Task ProjectComplianceTrendsAsync(Guid tenantId) => Task.CompletedTask;
        public Task ProjectRiskHeatmapAsync(Guid tenantId) => Task.CompletedTask;
        public Task ProjectFrameworkComparisonAsync(Guid tenantId) => Task.CompletedTask;
        public Task ProjectTaskMetricsAsync(Guid tenantId) => Task.CompletedTask;
        public Task ProjectEvidenceMetricsAsync(Guid tenantId) => Task.CompletedTask;
        public Task ProjectTopActionsAsync(Guid tenantId) => Task.CompletedTask;
        public Task ProjectAllAsync(Guid tenantId) => Task.CompletedTask;
        public Task HandleEventAsync(IGrcEvent domainEvent) => Task.CompletedTask;
    }
}
