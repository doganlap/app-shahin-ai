using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Grc.Web.Pages.Error;

public class IndexModel : PageModel
{
    public int StatusCode { get; set; }
    public string ErrorId { get; set; } = string.Empty;
    
    public void OnGet(int? statusCode = null)
    {
        StatusCode = statusCode ?? 500;
        ErrorId = Guid.NewGuid().ToString("N");
    }
}
