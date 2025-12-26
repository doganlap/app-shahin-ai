using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Grc.Enums;

namespace Grc.Workflow;

/// <summary>
/// Workflow definition (BPMN-style)
/// </summary>
public class WorkflowDefinition : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Version { get; private set; }
    public string BpmnXml { get; private set; }
    public WorkflowStatus Status { get; private set; }
    public string Category { get; private set; }
    public Dictionary<string, object> Variables { get; private set; }
    
    public ICollection<WorkflowInstance> Instances { get; private set; }
    
    protected WorkflowDefinition() { }
    
    public WorkflowDefinition(Guid id, string name, string version, string bpmnXml)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        Version = Check.NotNullOrWhiteSpace(version, nameof(version));
        BpmnXml = Check.NotNullOrWhiteSpace(bpmnXml, nameof(bpmnXml));
        Status = WorkflowStatus.Pending;
        Variables = new Dictionary<string, object>();
        Instances = new Collection<WorkflowInstance>();
    }
    
    public void Activate()
    {
        Status = WorkflowStatus.InProgress;
    }
    
    public void Deactivate()
    {
        Status = WorkflowStatus.Cancelled;
    }
    
    public WorkflowInstance CreateInstance(Guid? tenantId, Dictionary<string, object> inputVariables = null)
    {
        var instance = new WorkflowInstance(
            Guid.NewGuid(),
            Id,
            tenantId,
            inputVariables ?? new Dictionary<string, object>());
        
        Instances.Add(instance);
        return instance;
    }
}

