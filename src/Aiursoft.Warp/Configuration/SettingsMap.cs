using Aiursoft.Warp.Models;

namespace Aiursoft.Warp.Configuration;

public static class SettingsMap
{
    public const string AllowUserAdjustNickname = "Allow_User_Adjust_Nickname";

    public static readonly List<GlobalSettingDefinition> Definitions = new()
    {
        new GlobalSettingDefinition
        {
            Key = AllowUserAdjustNickname,
            Description = "Allow users to adjust their nickname in the profile management page.",
            Type = SettingType.Bool,
            DefaultValue = "True"
        }
    };
}
