using Volo.Abp.Settings;

namespace Grc.Settings;

public class GrcSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(GrcSettings.MySetting1));
    }
}
