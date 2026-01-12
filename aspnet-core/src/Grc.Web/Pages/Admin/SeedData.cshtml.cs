using Microsoft.AspNetCore.Authorization;
using Grc.Permissions;

namespace Grc.Web.Pages.Admin;

[Authorize(GrcPermissions.Admin.Default)]
public class SeedDataModel : GrcPageModel
{
    public void OnGet()
    {
        // Page initialization
    }
}

