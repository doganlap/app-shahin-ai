using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Grc.Onboarding;
using Grc.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Web.Pages.Onboarding;

[Authorize]
public class IndexModel : GrcPageModel
{
    private readonly OnboardingAppService _onboardingService;
    
    public UserOnboarding Onboarding { get; set; } = null!;
    public OnboardingStep CurrentStep { get; set; }
    public int TotalSteps => 7;
    public int ProgressPercentage => ((int)CurrentStep * 100) / TotalSteps;
    public bool CanSkip => CurrentUser.IsInRole("admin");
    
    public IndexModel(OnboardingAppService onboardingService)
    {
        _onboardingService = onboardingService;
    }
    
    public async Task<IActionResult> OnGetAsync(int? step = null)
    {
        // Check if user is authenticated
        if (!CurrentUser.Id.HasValue)
        {
            return RedirectToPage("/Account/Login");
        }
        
        // Get or create onboarding
        Onboarding = await _onboardingService.GetMyOnboardingAsync() 
                     ?? await _onboardingService.StartOnboardingAsync();
        
        // Check if already completed
        if (Onboarding.Status == OnboardingStatus.Completed)
        {
            return RedirectToPage("/Dashboard");
        }
        
        // Set current step
        CurrentStep = step.HasValue 
            ? (OnboardingStep)step.Value 
            : Onboarding.CurrentStep;
        
        return Page();
    }
    
    public async Task<IActionResult> OnPostNextStepAsync(int currentStep)
    {
        if (!CurrentUser.Id.HasValue)
        {
            return RedirectToPage("/Account/Login");
        }
        
        // Mark current step as completed
        await _onboardingService.CompleteStepAsync((OnboardingStep)currentStep);
        
        // Redirect to next step
        int nextStep = currentStep + 1;
        return RedirectToPage(new { step = nextStep });
    }
    
    public async Task<IActionResult> OnPostSaveStepAsync(int step)
    {
        try
        {
            // Get form data from Request.Form
            switch (step)
            {
                case 2: // Profile Setup
                    if (Request.Form.TryGetValue("fullName", out var fullName))
                        await _onboardingService.SetPreferenceAsync("FullName", fullName.ToString());
                    if (Request.Form.TryGetValue("jobTitle", out var jobTitle))
                        await _onboardingService.SetPreferenceAsync("JobTitle", jobTitle.ToString());
                    if (Request.Form.TryGetValue("department", out var department))
                        await _onboardingService.SetPreferenceAsync("Department", department.ToString());
                    if (Request.Form.TryGetValue("phoneNumber", out var phoneNumber))
                        await _onboardingService.SetPreferenceAsync("PhoneNumber", phoneNumber.ToString());
                    if (Request.Form.TryGetValue("preferredLanguage", out var language))
                        await _onboardingService.SetPreferenceAsync("PreferredLanguage", language.ToString());
                    if (Request.Form.TryGetValue("timezone", out var timezone))
                        await _onboardingService.SetPreferenceAsync("Timezone", timezone.ToString());
                    if (Request.Form.TryGetValue("emailNotifications", out var emailNotif))
                        await _onboardingService.SetPreferenceAsync("EmailNotifications", emailNotif.ToString());
                    if (Request.Form.TryGetValue("smsNotifications", out var smsNotif))
                        await _onboardingService.SetPreferenceAsync("SmsNotifications", smsNotif.ToString());
                    if (Request.Form.TryGetValue("browserNotifications", out var browserNotif))
                        await _onboardingService.SetPreferenceAsync("BrowserNotifications", browserNotif.ToString());
                    break;
                    
                case 4: // Role Assignment
                    if (Request.Form.TryGetValue("roles", out var rolesJson))
                    {
                        var roles = System.Text.Json.JsonSerializer.Deserialize<List<string>>(rolesJson.ToString());
                        if (roles != null)
                        {
                            foreach (var role in roles)
                            {
                                await _onboardingService.SetPreferenceAsync($"Role_{role}", "true");
                            }
                        }
                    }
                    break;
                    
                case 5: // Feature Configuration
                    if (Request.Form.TryGetValue("features", out var featuresJson))
                    {
                        var features = System.Text.Json.JsonSerializer.Deserialize<List<string>>(featuresJson.ToString());
                        if (features != null)
                        {
                            foreach (var feature in features)
                            {
                                await _onboardingService.SetPreferenceAsync($"Feature_{feature}", "true");
                            }
                        }
                    }
                    break;
            }
            
            // Mark step as completed
            var stepEnum = (OnboardingStep)step;
            await _onboardingService.CompleteStepAsync(stepEnum);
            
            return new JsonResult(new { success = true, message = "Step data saved successfully" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
    
    public async Task<IActionResult> OnPostCompleteAsync()
    {
        if (!CurrentUser.Id.HasValue)
        {
            return RedirectToPage("/Account/Login");
        }
        
        // Mark last step as completed (will auto-complete onboarding)
        await _onboardingService.CompleteStepAsync(OnboardingStep.Completion);
        
        // Redirect to dashboard
        return RedirectToPage("/Dashboard");
    }
    
    public async Task<IActionResult> OnPostSkipAsync()
    {
        if (!CurrentUser.Id.HasValue || !CanSkip)
        {
            return Forbid();
        }
        
        await _onboardingService.SkipOnboardingAsync(CurrentUser.Id.Value, "User skipped during setup");
        
        return RedirectToPage("/Dashboard");
    }
}
