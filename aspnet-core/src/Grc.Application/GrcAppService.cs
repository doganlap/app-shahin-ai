using System;
using System.Collections.Generic;
using System.Text;
using Grc.Localization;
using Volo.Abp.Application.Services;

namespace Grc;

/* Inherit your application services from this class.
 */
public abstract class GrcAppService : ApplicationService
{
    protected GrcAppService()
    {
        LocalizationResource = typeof(GrcResource);
    }
}
