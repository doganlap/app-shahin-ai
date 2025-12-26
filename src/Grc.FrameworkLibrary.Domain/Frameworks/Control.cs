using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.FrameworkLibrary.Frameworks;

/// <summary>
/// Individual compliance control/requirement
/// </summary>
public class Control : FullAuditedEntity<Guid>
{
    public Guid FrameworkId { get; private set; }
    public Guid? ParentControlId { get; private set; }
    public string ControlNumber { get; private set; }
    public string DomainCode { get; private set; }
    public LocalizedString Title { get; private set; }
    public LocalizedString Requirement { get; private set; }
    public LocalizedString ImplementationGuidance { get; private set; }
    public ControlType Type { get; private set; }
    public ControlCategory? Category { get; private set; }
    public int MaturityLevel { get; private set; }
    public Priority Priority { get; private set; }
    public List<string> EvidenceTypes { get; private set; }
    public int EstimatedEffortHours { get; private set; }
    public string MappingISO27001 { get; private set; }
    public string MappingNIST { get; private set; }
    public string MappingCOBIT { get; private set; }
    public List<string> Tags { get; private set; }
    public FrameworkStatus Status { get; private set; }
    
    public ICollection<ControlMapping> Mappings { get; private set; }
    public ICollection<Control> ChildControls { get; private set; }
    
    protected Control() { }
    
    public Control(Guid id, Guid frameworkId, string controlNumber, string domainCode,
                  LocalizedString title, LocalizedString requirement, ControlType type)
        : base(id)
    {
        FrameworkId = frameworkId;
        ControlNumber = Check.NotNullOrWhiteSpace(controlNumber, nameof(controlNumber));
        DomainCode = Check.NotNullOrWhiteSpace(domainCode, nameof(domainCode));
        Title = Check.NotNull(title, nameof(title));
        Requirement = Check.NotNull(requirement, nameof(requirement));
        Type = type;
        MaturityLevel = 1;
        Priority = Priority.Medium;
        Status = FrameworkStatus.Active;
        EvidenceTypes = new List<string>();
        Tags = new List<string>();
        Mappings = new Collection<ControlMapping>();
        ChildControls = new Collection<Control>();
    }
    
    public void SetMaturityLevel(int level)
    {
        if (level < 1 || level > 5)
            throw new ArgumentOutOfRangeException(nameof(level), "Must be between 1 and 5");
        MaturityLevel = level;
    }
    
    public void AddMapping(string framework, string controlNumber, string strength)
    {
        var mapping = new ControlMapping(Id, framework, controlNumber, strength);
        Mappings.Add(mapping);
    }
    
    public void SetParentControl(Guid? parentControlId)
    {
        ParentControlId = parentControlId;
    }
    
    public void SetImplementationGuidance(LocalizedString guidance)
    {
        ImplementationGuidance = guidance;
    }
    
    public void SetCategory(ControlCategory category)
    {
        Category = category;
    }
    
    public void SetPriority(Priority priority)
    {
        Priority = priority;
    }
    
    public void AddEvidenceType(string evidenceType)
    {
        if (!string.IsNullOrWhiteSpace(evidenceType) && !EvidenceTypes.Contains(evidenceType))
        {
            EvidenceTypes.Add(evidenceType);
        }
    }
    
    public void SetEstimatedEffortHours(int hours)
    {
        EstimatedEffortHours = hours;
    }
    
    public void SetMappingISO27001(string mapping)
    {
        MappingISO27001 = mapping;
    }
    
    public void SetMappingNIST(string mapping)
    {
        MappingNIST = mapping;
    }
    
    public void SetMappingCOBIT(string mapping)
    {
        MappingCOBIT = mapping;
    }
    
    public void AddTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag) && !Tags.Contains(tag))
        {
            Tags.Add(tag);
        }
    }
}

