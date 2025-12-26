using Microsoft.Extensions.Localization;
using Grc.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Grc;

[Dependency(ReplaceServices = true)]
public class GrcBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<GrcResource> _localizer;

    public GrcBrandingProvider(IStringLocalizer<GrcResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
