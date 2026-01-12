namespace Grc.Onboarding;

/// <summary>
/// Onboarding process steps
/// </summary>
public enum OnboardingStep
{
    /// <summary>
    /// Welcome and account activation
    /// </summary>
    Welcome = 1,
    
    /// <summary>
    /// Profile setup (name, contact info, language preference)
    /// </summary>
    ProfileSetup = 2,
    
    /// <summary>
    /// Organization/department selection
    /// </summary>
    OrganizationAssignment = 3,
    
    /// <summary>
    /// Role and permissions configuration
    /// </summary>
    RoleAssignment = 4,
    
    /// <summary>
    /// Feature access configuration
    /// </summary>
    FeatureConfiguration = 5,
    
    /// <summary>
    /// Quick tour of the application
    /// </summary>
    ApplicationTour = 6,
    
    /// <summary>
    /// Final confirmation and completion
    /// </summary>
    Completion = 7
}
