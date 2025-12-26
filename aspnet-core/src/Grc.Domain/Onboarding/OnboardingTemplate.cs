using System;
using System.Collections.Generic;
using Grc.ValueObjects;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Grc.Onboarding;

/// <summary>
/// Template for onboarding process configuration per role/organization
/// </summary>
public class OnboardingTemplate : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Template name
    /// </summary>
    public LocalizedString Name { get; private set; }
    
    /// <summary>
    /// Template description
    /// </summary>
    public LocalizedString Description { get; private set; }
    
    /// <summary>
    /// Role this template applies to
    /// </summary>
    public string? TargetRole { get; set; }
    
    /// <summary>
    /// Steps included in this template
    /// </summary>
    public List<OnboardingStep> RequiredSteps { get; private set; }
    
    /// <summary>
    /// Default roles to assign
    /// </summary>
    public List<string> DefaultRoles { get; private set; }
    
    /// <summary>
    /// Default features to enable
    /// </summary>
    public Dictionary<string, bool> DefaultFeatures { get; private set; }
    
    /// <summary>
    /// Is this template active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }
    
    protected OnboardingTemplate()
    {
        RequiredSteps = new List<OnboardingStep>();
        DefaultRoles = new List<string>();
        DefaultFeatures = new Dictionary<string, bool>();
    }
    
    public OnboardingTemplate(
        Guid id,
        LocalizedString name,
        LocalizedString description,
        string? targetRole = null)
        : base(id)
    {
        Name = Check.NotNull(name, nameof(name));
        Description = Check.NotNull(description, nameof(description));
        TargetRole = targetRole;
        RequiredSteps = new List<OnboardingStep>();
        DefaultRoles = new List<string>();
        DefaultFeatures = new Dictionary<string, bool>();
        IsActive = true;
        DisplayOrder = 0;
    }
    
    public void AddRequiredStep(OnboardingStep step)
    {
        if (!RequiredSteps.Contains(step))
        {
            RequiredSteps.Add(step);
        }
    }
    
    public void AddDefaultRole(string roleName)
    {
        Check.NotNullOrWhiteSpace(roleName, nameof(roleName));
        if (!DefaultRoles.Contains(roleName))
        {
            DefaultRoles.Add(roleName);
        }
    }
    
    public void SetDefaultFeature(string featureName, bool isEnabled)
    {
        Check.NotNullOrWhiteSpace(featureName, nameof(featureName));
        DefaultFeatures[featureName] = isEnabled;
    }
}
