using Common.Classes;
using Common.Enums;
using Common.Statics.BaseClasses;
using static Common.Strings.Config;

namespace Common.Statics;

public sealed class ConfigOptions : StaticClass<ConfigOption>
{
    private ConfigOptions() : base(new List<ConfigOption>
    {
        new(1, 1, 2, Roles.Mod, ConfigType.Role, null, null, false),
        new(2, 1, 3, Channels.Mod, ConfigType.Channel, null, null, false),
        new(3, 2, 3, Channels.Log, ConfigType.Channel, null, LeylaModule.Logs, true),
        new(4, 3, 3, Channels.Archive, ConfigType.Channel, null, LeylaModule.Logs, true),
        new(5, 2, 2, Roles.Verification, ConfigType.Role, null, null, true),
        new(6, 1, 4, Spam.BasePressure, ConfigType.Decimal, "10", null, false),
        new(7, 2, 4, Spam.ImagePressure, ConfigType.Decimal, "8.3", null, false),
        new(8, 3, 4, Spam.LengthPressure, ConfigType.Decimal, "0.00625", null, false),
        new(9, 4, 4, Spam.LinePressure, ConfigType.Decimal, "0.714", null, false),
        new(10, 5, 4, Spam.PingPressure, ConfigType.Decimal, "2.5", null, false),
        new(11, 6, 4, Spam.RepeatPressure, ConfigType.Decimal, "10", null, false),
        new(12, 7, 4, Spam.MaxPressure, ConfigType.Decimal, "60", null, false),
        new(13, 8, 4, Spam.PressureDecay, ConfigType.Decimal, "2.5", null, false),
        new(14, 3, 2, Roles.Silence, ConfigType.Role, null, LeylaModule.Spam, true),
        new(15, 9, 4, Spam.DeleteMessages, ConfigType.Boolean, "0", null, false),
        new(16, 4, 3, Channels.Silence, ConfigType.Channel, null, LeylaModule.Spam, true),
        new(17, 10, 4, Spam.SilenceMessage, ConfigType.String, string.Empty, null, true),
        new(18, 11, 4, Spam.Timeout, typeof(TimeoutDuration), "0", null, false),
        new(19, 1, 5, Raid.RaidMode, ConfigType.Boolean, "false", null, false),
        new(20, 2, 5, Raid.RaidSize, ConfigType.Int, null, null, false),
        new(21, 3, 5, Raid.RaidTime, ConfigType.Int, null, null, false),
        new(22, 5, 3, Raid.RaidChannel, ConfigType.Channel, null, LeylaModule.Spam, true),
        new(23, 3, 2, Raid.RaidRole, ConfigType.Role, null, LeylaModule.Spam, true),
        new(24, 4, 5, Raid.RaidMessage, ConfigType.String, null, null, true),
        new(25, 5, 5, Raid.LockdownDuration, ConfigType.Int, "15", null, true),
        new(26, 6, 5, Raid.NotifyModerators, ConfigType.Boolean, "1", null, false)
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
    public readonly bool Nullable;

    internal ConfigOption(int id,
        int sortId,
        int categoryId,
        DisplayString displayString,
        ConfigType configType,
        string? defaultValue,
        LeylaModule? module,
        bool nullable)
        : base(id, sortId, displayString.Name)
    {
        Description = displayString.Description;
        ConfigType = configType;
        DefaultValue = defaultValue;
        ConfigOptionCategoryId = categoryId;
        Module = module;
        Nullable = nullable;
    }

    internal ConfigOption(int id,
        int sortId,
        int categoryId,
        DisplayString displayString,
        Type enumType,
        string? defaultValue,
        LeylaModule? module,
        bool nullable)
        : base(id, sortId, displayString.Name)
    {
        Description = displayString.Description;
        ConfigType = ConfigType.Enum;
        EnumType = enumType;
        DefaultValue = defaultValue;
        ConfigOptionCategoryId = categoryId;
        Module = module;
        Nullable = nullable;
    }

    public ConfigOptionCategory ConfigOptionCategory => ConfigOptionCategories.Instance.Get(ConfigOptionCategoryId);
}