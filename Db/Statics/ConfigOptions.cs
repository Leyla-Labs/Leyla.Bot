using Common.Classes;
using Common.Enums;
using Db.Enums;
using Db.Statics.BaseClasses;
using static Common.Strings.Config;

namespace Db.Statics;

public sealed class ConfigOptions : StaticClass<ConfigOption>
{
    private ConfigOptions() : base(new List<ConfigOption>
    {
        new(1, 1, 2, Roles.Mod, ConfigType.Role, null),
        new(2, 1, 3, Channels.Mod, ConfigType.Channel, null),
        new(3, 2, 3, Channels.Log, ConfigType.Channel, null, LeylaModule.Logs),
        new(4, 3, 3, Channels.Archive, ConfigType.Channel, null, LeylaModule.Logs),
        new(5, 2, 2, Roles.Verification, ConfigType.Role, null),
        new(6, 1, 4, Spam.BasePressure, ConfigType.Decimal, "10"),
        new(7, 2, 4, Spam.ImagePressure, ConfigType.Decimal, "8.3"),
        new(8, 3, 4, Spam.LengthPressure, ConfigType.Decimal, "0.00625"),
        new(9, 4, 4, Spam.LinePressure, ConfigType.Decimal, "0.714"),
        new(10, 5, 4, Spam.PingPressure, ConfigType.Decimal, "2.5"),
        new(11, 6, 4, Spam.RepeatPressure, ConfigType.Decimal, "10"),
        new(12, 7, 4, Spam.MaxPressure, ConfigType.Decimal, "60"),
        new(13, 8, 4, Spam.PressureDecay, ConfigType.Decimal, "2.5"),
        new(14, 3, 2, Roles.Silence, ConfigType.Role, null, LeylaModule.Spam),
        new(15, 9, 4, Spam.DeleteMessages, ConfigType.Boolean, "0"),
        new(16, 4, 3, Channels.Silence, ConfigType.Channel, null, LeylaModule.Spam),
        new(17, 10, 4, Spam.SilenceMessage, ConfigType.String, string.Empty),
        new(18, 11, 4, Spam.Timeout, typeof(TimeoutDuration), "0")
    })
    {
    }

    #region Singleton

    private static readonly Lazy<ConfigOptions> Lazy = new(() => new ConfigOptions());
    public static ConfigOptions Instance => Lazy.Value;

    #endregion
}

public class ConfigOption : StaticField
{
    public readonly int ConfigOptionCategoryId;
    public readonly ConfigType ConfigType;
    public readonly string? DefaultValue;
    public readonly string Description;
    public readonly Type? EnumType;
    public readonly LeylaModule? Module;

    public ConfigOption(int id,
        int sortId,
        int categoryId,
        DisplayString displayString,
        ConfigType configType,
        string? defaultValue,
        LeylaModule? module = null)
        : base(id, sortId, displayString.Name)
    {
        Description = displayString.Description;
        ConfigType = configType;
        DefaultValue = defaultValue;
        ConfigOptionCategoryId = categoryId;
        Module = module;
    }

    public ConfigOption(int id,
        int sortId,
        int categoryId,
        DisplayString displayString,
        Type enumType,
        string? defaultValue,
        LeylaModule? module = null)
        : base(id, sortId, displayString.Name)
    {
        Description = displayString.Description;
        ConfigType = ConfigType.Enum;
        EnumType = enumType;
        DefaultValue = defaultValue;
        ConfigOptionCategoryId = categoryId;
        Module = module;
    }

    public ConfigOptionCategory ConfigOptionCategory => ConfigOptionCategories.Instance.Get(ConfigOptionCategoryId);
}