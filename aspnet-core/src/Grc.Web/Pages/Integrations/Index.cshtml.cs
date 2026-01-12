using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using IntegrationConnectorEntity = Grc.Integration.IntegrationConnector;
using IntegrationType = Grc.Integration.IntegrationType;
using IntegrationStatus = Grc.Integration.IntegrationStatus;

namespace Grc.Web.Pages.Integrations;

[Authorize(GrcPermissions.Integrations.Default)]
public class IndexModel : GrcPageModel
{
    private readonly IRepository<IntegrationConnectorEntity, Guid> _integrationRepository;

    public List<IntegrationConnectorDto> Connectors { get; set; } = new();
    public List<SyncHistory> RecentSyncs { get; set; } = new();
    public IntegrationSummary Summary { get; set; } = new();

    public IndexModel(IRepository<IntegrationConnectorEntity, Guid> integrationRepository)
    {
        _integrationRepository = integrationRepository;
    }

    public async Task OnGetAsync()
    {
        var queryable = await _integrationRepository.GetQueryableAsync();

        // Get integrations from database
        var integrations = await queryable
            .OrderBy(i => i.Name)
            .ToListAsync();

        Connectors = integrations.Select(i => new IntegrationConnectorDto
        {
            Id = i.Id,
            Name = i.Name,
            NameAr = i.NameAr,
            Description = i.Description,
            DescriptionAr = i.DescriptionAr,
            Type = GetTypeString(i.Type),
            TypeEnum = i.Type,
            Status = GetStatusString(i.Status),
            StatusEnum = i.Status,
            LastSync = i.LastSyncAt,
            NextSync = i.NextSyncAt,
            SyncIntervalMinutes = i.SyncIntervalMinutes,
            AutoSyncEnabled = i.AutoSyncEnabled,
            SuccessfulSyncs = i.SuccessfulSyncs,
            FailedSyncs = i.FailedSyncs,
            LastError = i.LastError,
            Icon = i.Icon ?? GetDefaultIcon(i.Type),
            Color = i.Color ?? GetDefaultColor(i.Status),
            BaseUrl = i.BaseUrl
        }).ToList();

        // Build sync history from successful/failed sync counts
        RecentSyncs = integrations
            .Where(i => i.LastSyncAt.HasValue)
            .OrderByDescending(i => i.LastSyncAt)
            .Take(10)
            .Select(i => new SyncHistory
            {
                Id = i.Id,
                Connector = i.Name,
                ConnectorAr = i.NameAr,
                Type = GetTypeString(i.Type),
                RecordsProcessed = i.SuccessfulSyncs,
                Status = string.IsNullOrEmpty(i.LastError) ? "Success" : "Error",
                SyncedAt = i.LastSyncAt ?? DateTime.UtcNow
            }).ToList();

        // Calculate summary from actual data
        Summary = new IntegrationSummary
        {
            TotalConnectors = await queryable.CountAsync(),
            Connected = await queryable.CountAsync(i => i.Status == IntegrationStatus.Connected),
            Configured = await queryable.CountAsync(i => i.Status == IntegrationStatus.Configured),
            Disconnected = await queryable.CountAsync(i => i.Status == IntegrationStatus.Disconnected),
            NotConfigured = await queryable.CountAsync(i => i.Status == IntegrationStatus.NotConfigured),
            Error = await queryable.CountAsync(i => i.Status == IntegrationStatus.Error),
            TotalSuccessfulSyncs = integrations.Sum(i => i.SuccessfulSyncs),
            TotalFailedSyncs = integrations.Sum(i => i.FailedSyncs)
        };
    }

    private static string GetTypeString(IntegrationType type)
    {
        return type switch
        {
            IntegrationType.Identity => "Identity",
            IntegrationType.IssueTracking => "Issue Tracking",
            IntegrationType.ITSM => "ITSM",
            IntegrationType.DocumentManagement => "Document Management",
            IntegrationType.Notifications => "Notifications",
            IntegrationType.SIEM => "SIEM",
            IntegrationType.Vulnerability => "Vulnerability Management",
            IntegrationType.CloudProvider => "Cloud Provider",
            IntegrationType.Email => "Email",
            IntegrationType.Calendar => "Calendar",
            IntegrationType.AI => "AI",
            IntegrationType.Workflow => "Workflow",
            _ => "Other"
        };
    }

    private static string GetStatusString(IntegrationStatus status)
    {
        return status switch
        {
            IntegrationStatus.NotConfigured => "Not Configured",
            IntegrationStatus.Configured => "Configured",
            IntegrationStatus.Connected => "Connected",
            IntegrationStatus.Disconnected => "Disconnected",
            IntegrationStatus.Error => "Error",
            IntegrationStatus.Syncing => "Syncing",
            _ => "Unknown"
        };
    }

    private static string GetDefaultIcon(IntegrationType type)
    {
        return type switch
        {
            IntegrationType.Identity => "fa-users",
            IntegrationType.IssueTracking => "fa-bug",
            IntegrationType.ITSM => "fa-headset",
            IntegrationType.DocumentManagement => "fa-folder-open",
            IntegrationType.Notifications => "fa-bell",
            IntegrationType.SIEM => "fa-shield-alt",
            IntegrationType.Vulnerability => "fa-search",
            IntegrationType.CloudProvider => "fa-cloud",
            IntegrationType.Email => "fa-envelope",
            IntegrationType.Calendar => "fa-calendar",
            IntegrationType.AI => "fa-brain",
            IntegrationType.Workflow => "fa-project-diagram",
            _ => "fa-plug"
        };
    }

    private static string GetDefaultColor(IntegrationStatus status)
    {
        return status switch
        {
            IntegrationStatus.Connected => "#28a745",
            IntegrationStatus.Configured => "#17a2b8",
            IntegrationStatus.Disconnected => "#6c757d",
            IntegrationStatus.NotConfigured => "#ffc107",
            IntegrationStatus.Error => "#dc3545",
            IntegrationStatus.Syncing => "#007bff",
            _ => "#6c757d"
        };
    }
}

public class IntegrationConnectorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public IntegrationType TypeEnum { get; set; }
    public string Status { get; set; } = string.Empty;
    public IntegrationStatus StatusEnum { get; set; }
    public DateTime? LastSync { get; set; }
    public DateTime? NextSync { get; set; }
    public int SyncIntervalMinutes { get; set; }
    public bool AutoSyncEnabled { get; set; }
    public int SuccessfulSyncs { get; set; }
    public int FailedSyncs { get; set; }
    public string LastError { get; set; } = string.Empty;
    public string Icon { get; set; } = "fa-plug";
    public string Color { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}

public class SyncHistory
{
    public Guid Id { get; set; }
    public string Connector { get; set; } = string.Empty;
    public string ConnectorAr { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int RecordsProcessed { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SyncedAt { get; set; }
}

public class IntegrationSummary
{
    public int TotalConnectors { get; set; }
    public int Connected { get; set; }
    public int Configured { get; set; }
    public int Disconnected { get; set; }
    public int NotConfigured { get; set; }
    public int Error { get; set; }
    public int TotalSuccessfulSyncs { get; set; }
    public int TotalFailedSyncs { get; set; }
}
