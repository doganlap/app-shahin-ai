using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Grc.FrameworkLibrary.Application.Contracts.Frameworks;

namespace Grc.Web.Pages.FrameworkLibrary;

public class DetailsModel : GrcPageModel
{
    private readonly IFrameworkAppService _frameworkAppService;

    public FrameworkDto Framework { get; set; } = new();

    public DetailsModel(IFrameworkAppService frameworkAppService)
    {
        _frameworkAppService = frameworkAppService;
    }

    public async Task OnGetAsync(Guid id)
    {
        try
        {
            Framework = await _frameworkAppService.GetAsync(id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load framework {Id}", id);
            throw;
        }
    }
}

