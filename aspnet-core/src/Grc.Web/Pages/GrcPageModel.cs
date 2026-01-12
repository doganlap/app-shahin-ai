using Grc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Grc.Web.Pages;

public abstract class GrcPageModel : AbpPageModel
{
    protected GrcPageModel()
    {
        LocalizationResourceType = typeof(GrcResource);
    }
}

