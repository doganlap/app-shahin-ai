namespace Grc.Onboarding;

/// <summary>
/// User onboarding process status
/// </summary>
public enum OnboardingStatus
{
    /// <summary>
    /// User created but onboarding not started
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// User is going through onboarding steps
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// Onboarding completed successfully
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Onboarding skipped by admin
    /// </summary>
    Skipped = 3,
    
    /// <summary>
    /// Onboarding failed and needs review
    /// </summary>
    Failed = 4
}
