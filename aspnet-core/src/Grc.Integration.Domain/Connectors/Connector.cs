using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Integration.Domain.Connectors;

/// <summary>
/// External system connector
/// </summary>
public class Connector : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string ConnectorType { get; set; } // ActiveDirectory, ServiceNow, Jira, SharePoint
    public string Configuration { get; set; } // JSON configuration
    public bool IsActive { get; set; }
    public DateTime? LastSyncTime { get; set; }
    public string LastSyncStatus { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    
    protected Connector() { }
    
    public Connector(Guid id, string name, string connectorType, string configuration)
        : base(id)
    {
        Name = name;
        ConnectorType = connectorType;
        Configuration = configuration;
        IsActive = false;
        Metadata = new Dictionary<string, object>();
    }
    
    public void Activate()
    {
        IsActive = true;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
    
    public void UpdateLastSync(DateTime syncTime, string status)
    {
        LastSyncTime = syncTime;
        LastSyncStatus = status;
    }
}

