using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Grc.Web.Pages.Error;

public class AccessDeniedModel : PageModel
{
    public string RequestedPath { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    
    public void OnGet(string? returnUrl = null)
    {
        RequestedPath = Request.Path;
        ReturnUrl = returnUrl ?? "/";
    }
}
