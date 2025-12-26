using Grc.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Grc.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class GrcController : AbpControllerBase
{
    protected GrcController()
    {
        LocalizationResource = typeof(GrcResource);
    }
}
