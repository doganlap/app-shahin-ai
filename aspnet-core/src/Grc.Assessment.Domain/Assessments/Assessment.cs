using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.Enums;
using Grc.Events;

namespace Grc.Assessments;

/// <summary>
/// Compliance assessment instance
/// </summary>
public class Assessment : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public AssessmentType Type { get; private set; }
    public AssessmentStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime TargetEndDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public Guid? OwnerUserId { get; private set; }
    public Dictionary<string, object> Scope { get; private set; }
    
    public ICollection<AssessmentFramework> Frameworks { get; private set; }
    public ICollection<ControlAssessment> ControlAssessments { get; private set; }
    
    public int TotalControls => ControlAssessments?.Count ?? 0;
    
    public int CompletedControls => ControlAssessments?.Count(c => c.IsComplete) ?? 0;
    
    public decimal CompletionPercentage => TotalControls > 0 
        ? Math.Round((CompletedControls * 100m / TotalControls), 2) : 0;
    
    public decimal OverallScore => CalculateOverallScore();
    
    protected Assessment() { }
    
    public Assessment(Guid id, string name, AssessmentType type, 
                     DateTime startDate, DateTime targetEndDate)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 200);
        Type = type;
        StartDate = startDate;
        TargetEndDate = targetEndDate;
        Status = AssessmentStatus.Draft;
        Frameworks = new Collection<AssessmentFramework>();
        ControlAssessments = new Collection<ControlAssessment>();
        Scope = new Dictionary<string, object>();
        
        AddDistributedEvent(new AssessmentCreatedEto 
        { 
            AssessmentId = id, 
            Name = name,
            TenantId = TenantId,
            CreationTime = DateTime.UtcNow
        });
    }
    
    public void Start()
    {
        if (Status != AssessmentStatus.Draft)
            throw new BusinessException("Assessment must be in Draft status to start");
        Status = AssessmentStatus.InProgress;
        AddDistributedEvent(new AssessmentStartedEto 
        { 
            AssessmentId = Id,
            TenantId = TenantId,
            StartTime = DateTime.UtcNow
        });
    }
    
    public void Complete()
    {
        Status = AssessmentStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        AddDistributedEvent(new AssessmentCompletedEto 
        { 
            AssessmentId = Id,
            TenantId = TenantId,
            OverallScore = OverallScore,
            CompletionTime = DateTime.UtcNow
        });
    }
    
    public ControlAssessment AddControlAssessment(Guid controlId)
    {
        var ca = new ControlAssessment(Guid.NewGuid(), Id, controlId)
        {
            TenantId = TenantId
        };
        ControlAssessments.Add(ca);
        return ca;
    }
    
    public void SetDescription(string description)
    {
        Description = description;
    }
    
    public void SetOwner(Guid? ownerUserId)
    {
        OwnerUserId = ownerUserId;
    }
    
    public void SetScope(Dictionary<string, object> scope)
    {
        Scope = scope ?? new Dictionary<string, object>();
    }

    public void AddFramework(Guid frameworkId)
    {
        if (Frameworks.Any(f => f.FrameworkId == frameworkId))
            return;

        var af = new AssessmentFramework(Guid.NewGuid(), Id, frameworkId);
        Frameworks.Add(af);
    }

    private decimal CalculateOverallScore()
    {
        var scored = ControlAssessments?.Where(c => c.VerifiedScore.HasValue).ToList();
        if (scored == null || !scored.Any()) return 0;
        return Math.Round(scored.Average(c => c.VerifiedScore!.Value), 2);
    }
}

