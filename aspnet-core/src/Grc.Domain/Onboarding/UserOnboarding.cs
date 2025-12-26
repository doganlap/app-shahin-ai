using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Onboarding;

/// <summary>
/// Tracks user onboarding process and status
/// </summary>
public class UserOnboarding : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    /// <summary>
    /// User being onboarded
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// Current onboarding status
    /// </summary>
    public OnboardingStatus Status { get; private set; }
    
    /// <summary>
    /// Current step in the onboarding process
    /// </summary>
    public OnboardingStep CurrentStep { get; private set; }
    
    /// <summary>
    /// Steps completed by the user
    /// </summary>
    public List<OnboardingStep> CompletedSteps { get; private set; }
    
    /// <summary>
    /// When onboarding started
    /// </summary>
    public DateTime? StartedAt { get; private set; }
    
    /// <summary>
    /// When onboarding completed
    /// </summary>
    public DateTime? CompletedAt { get; private set; }
    
    /// <summary>
    /// Roles assigned during onboarding
    /// </summary>
    public List<string> AssignedRoles { get; private set; }
    
    /// <summary>
    /// Organization units assigned during onboarding
    /// </summary>
    public List<Guid> AssignedOrganizationUnits { get; private set; }
    
    /// <summary>
    /// Features enabled during onboarding
    /// </summary>
    public Dictionary<string, bool> EnabledFeatures { get; private set; }
    
    /// <summary>
    /// User preferences set during onboarding
    /// </summary>
    public Dictionary<string, string> UserPreferences { get; private set; }
    
    /// <summary>
    /// Notes about the onboarding process
    /// </summary>
    public string? Notes { get; set; }
    
    protected UserOnboarding()
    {
        CompletedSteps = new List<OnboardingStep>();
        AssignedRoles = new List<string>();
        AssignedOrganizationUnits = new List<Guid>();
        EnabledFeatures = new Dictionary<string, bool>();
        UserPreferences = new Dictionary<string, string>();
    }
    
    public UserOnboarding(Guid id, Guid userId, Guid? tenantId = null)
        : base(id)
    {
        UserId = userId;
        TenantId = tenantId;
        Status = OnboardingStatus.Pending;
        CurrentStep = OnboardingStep.Welcome;
        CompletedSteps = new List<OnboardingStep>();
        AssignedRoles = new List<string>();
        AssignedOrganizationUnits = new List<Guid>();
        EnabledFeatures = new Dictionary<string, bool>();
        UserPreferences = new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Start the onboarding process
    /// </summary>
    public void Start()
    {
        if (Status != OnboardingStatus.Pending)
        {
            throw new BusinessException("Onboarding can only be started when status is Pending");
        }
        
        Status = OnboardingStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        CurrentStep = OnboardingStep.Welcome;
    }
    
    /// <summary>
    /// Mark a step as completed and move to next
    /// </summary>
    public void CompleteStep(OnboardingStep step)
    {
        if (Status != OnboardingStatus.InProgress)
        {
            throw new BusinessException("Cannot complete steps when onboarding is not in progress");
        }
        
        if (!CompletedSteps.Contains(step))
        {
            CompletedSteps.Add(step);
        }
        
        // Move to next step
        if (step < OnboardingStep.Completion)
        {
            CurrentStep = step + 1;
        }
        else
        {
            Complete();
        }
    }
    
    /// <summary>
    /// Assign a role to the user
    /// </summary>
    public void AssignRole(string roleName)
    {
        Check.NotNullOrWhiteSpace(roleName, nameof(roleName));
        
        if (!AssignedRoles.Contains(roleName))
        {
            AssignedRoles.Add(roleName);
        }
    }
    
    /// <summary>
    /// Assign organization unit to the user
    /// </summary>
    public void AssignOrganizationUnit(Guid organizationUnitId)
    {
        if (!AssignedOrganizationUnits.Contains(organizationUnitId))
        {
            AssignedOrganizationUnits.Add(organizationUnitId);
        }
    }
    
    /// <summary>
    /// Enable/disable a feature for the user
    /// </summary>
    public void SetFeature(string featureName, bool isEnabled)
    {
        Check.NotNullOrWhiteSpace(featureName, nameof(featureName));
        EnabledFeatures[featureName] = isEnabled;
    }
    
    /// <summary>
    /// Set user preference
    /// </summary>
    public void SetPreference(string key, string value)
    {
        Check.NotNullOrWhiteSpace(key, nameof(key));
        UserPreferences[key] = value;
    }
    
    /// <summary>
    /// Complete the onboarding process
    /// </summary>
    public void Complete()
    {
        if (Status == OnboardingStatus.Completed)
        {
            return; // Already completed
        }
        
        Status = OnboardingStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CurrentStep = OnboardingStep.Completion;
    }
    
    /// <summary>
    /// Skip the onboarding process
    /// </summary>
    public void Skip(string reason)
    {
        Status = OnboardingStatus.Skipped;
        CompletedAt = DateTime.UtcNow;
        Notes = reason;
    }
    
    /// <summary>
    /// Mark onboarding as failed
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        Status = OnboardingStatus.Failed;
        Notes = reason;
    }
}
