using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.Enums;
using Grc.Events;

namespace Grc.Assessments;

/// <summary>
/// Assessment of a single control
/// </summary>
public class ControlAssessment : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid AssessmentId { get; private set; }
    public Guid ControlId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public Guid? AssignedToDepartmentId { get; private set; }
    public ControlAssessmentStatus Status { get; private set; }
    public decimal? SelfScore { get; private set; }
    public decimal? VerifiedScore { get; private set; }
    public Guid? VerifiedByUserId { get; private set; }
    public DateTime? VerificationDate { get; private set; }
    public string ImplementationNotes { get; private set; }
    public string RejectionReason { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Priority Priority { get; private set; }
    
    public ICollection<Evidence.Evidence> Evidences { get; private set; }
    public ICollection<ControlAssessmentComment> Comments { get; private set; }
    public ICollection<ControlAssessmentHistory> History { get; private set; }
    
    public bool IsComplete => Status == ControlAssessmentStatus.Verified;
    
    public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && !IsComplete;
    
    protected ControlAssessment() { }
    
    public ControlAssessment(Guid id, Guid assessmentId, Guid controlId)
        : base(id)
    {
        AssessmentId = assessmentId;
        ControlId = controlId;
        Status = ControlAssessmentStatus.NotStarted;
        Priority = Priority.Medium;
        Evidences = new Collection<Evidence.Evidence>();
        Comments = new Collection<ControlAssessmentComment>();
        History = new Collection<ControlAssessmentHistory>();
    }
    
    public void AssignTo(Guid userId, DateTime? dueDate = null)
    {
        AssignedToUserId = userId;
        DueDate = dueDate;
        AddHistory("Assigned", $"Assigned to user {userId}");
        AddDistributedEvent(new ControlAssignedEto 
        { 
            ControlAssessmentId = Id, 
            UserId = userId,
            TenantId = TenantId,
            DueDate = dueDate,
            AssignmentTime = DateTime.UtcNow
        });
    }
    
    public void SubmitSelfScore(decimal score, string notes)
    {
        if (score < 0 || score > 100)
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 0 and 100");
        SelfScore = score;
        ImplementationNotes = notes;
        Status = ControlAssessmentStatus.PendingReview;
        AddHistory("SelfScored", $"Self-score: {score}");
        AddDistributedEvent(new SelfScoreSubmittedEto 
        { 
            ControlAssessmentId = Id, 
            Score = score,
            TenantId = TenantId,
            SubmissionTime = DateTime.UtcNow
        });
    }
    
    public void Verify(Guid verifierId, decimal score)
    {
        if (score < 0 || score > 100)
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 0 and 100");
        VerifiedScore = score;
        VerifiedByUserId = verifierId;
        VerificationDate = DateTime.UtcNow;
        Status = ControlAssessmentStatus.Verified;
        RejectionReason = null;
        AddHistory("Verified", $"Verified with score: {score}");
        AddDistributedEvent(new ControlVerifiedEto 
        { 
            ControlAssessmentId = Id, 
            VerifierId = verifierId, 
            Score = score,
            TenantId = TenantId,
            VerificationTime = DateTime.UtcNow
        });
    }
    
    public void Reject(Guid verifierId, string reason)
    {
        VerifiedByUserId = verifierId;
        RejectionReason = Check.NotNullOrWhiteSpace(reason, nameof(reason));
        Status = ControlAssessmentStatus.Rejected;
        AddHistory("Rejected", reason);
        AddDistributedEvent(new ControlRejectedEto 
        { 
            ControlAssessmentId = Id,
            VerifierId = verifierId,
            Reason = reason,
            TenantId = TenantId,
            RejectionTime = DateTime.UtcNow
        });
    }
    
    public void SetPriority(Priority priority)
    {
        Priority = priority;
    }
    
    public void SetAssignedToDepartment(Guid? departmentId)
    {
        AssignedToDepartmentId = departmentId;
    }
    
    private void AddHistory(string action, string details)
    {
        History.Add(new ControlAssessmentHistory(Id, action, details));
    }
}

