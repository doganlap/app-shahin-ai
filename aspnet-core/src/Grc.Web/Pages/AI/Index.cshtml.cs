using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Grc.Permissions;

namespace Grc.Web.Pages.AI;

[Authorize(GrcPermissions.AI.Default)]
public class IndexModel : GrcPageModel
{
    public async Task OnGetAsync()
    {
        await Task.CompletedTask;
    }
}

