using System.Threading.Tasks;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace Grc.Web.Pages.MySettings;

[Authorize]
public class IndexModel : GrcPageModel
{
    private readonly IIdentityUserAppService _userAppService;

    public IdentityUserDto CurrentUser { get; set; } = null!;
    public string TimeZone { get; set; } = "Asia/Riyadh";
    public string Language { get; set; } = "ar";
    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public bool InAppNotifications { get; set; } = true;
    public string Theme { get; set; } = "light";

    public IndexModel(IIdentityUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    public async Task OnGetAsync()
    {
        if (CurrentUser != null)
        {
            var user = await _userAppService.GetAsync(CurrentUser.Id);
            CurrentUser = user;
        }
    }

    public async Task<IActionResult> OnPostSaveAsync(string timezone, string language, bool emailNotifications, bool smsNotifications, bool inAppNotifications, string theme)
    {
        TimeZone = timezone;
        Language = language;
        EmailNotifications = emailNotifications;
        SmsNotifications = smsNotifications;
        InAppNotifications = inAppNotifications;
        Theme = theme;

        await Task.CompletedTask;
        return new JsonResult(new { success = true, message = L["SettingsSaved"] });
    }
}
