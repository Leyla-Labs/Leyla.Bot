using Db.Enums;
using Db.Statics.BaseClasses;
using Db.Strings;

namespace Db.Statics;

public sealed class ConfigOptions : StaticClass<ConfigOption>
{
    private ConfigOptions() : base(new List<ConfigOption>
    {
        new(1, 1, "Moderator Role", "todo", ConfigType.Role, null, 2),
        new(2, 1, "Moderator Channel", "todo", ConfigType.Channel, null, 3),
        new(3, 2, "Log Channel", "todo", ConfigType.Channel, null, 3),
        new(4, 3, "Archive Channel", "todo", ConfigType.Channel, null, 3),
        new(5, 2, "Verification Role", "todo", ConfigType.Role, null, 2),
        new(6, 1, Spam.BasePressure, "todo", ConfigType.Decimal, "10", 4),
        new(7, 2, Spam.ImagePressure, "todo", ConfigType.Decimal, "8.3", 4),
        new(8, 3, Spam.LengthPressure, "todo", ConfigType.Decimal, "0.00625", 4),
        new(9, 4, Spam.LinePressure, "todo", ConfigType.Decimal, "0.714", 4),
        new(10, 5, Spam.PingPressure, "todo", ConfigType.Decimal, "2.5", 4),
        new(11, 6, Spam.RepeatPressure, "todo", ConfigType.Decimal, "10", 4),
        new(12, 7, Spam.MaxPressure, "todo", ConfigType.Decimal, "60", 4),
        new(13, 8, Spam.PressureDecay, "todo", ConfigType.Decimal, "2.5", 4),
        new(14, 3, "Silence Role", "todo", ConfigType.Role, null, 2),
        new(15, 9, Spam.DeleteMessages, "todo", ConfigType.Boolean, "0", 4),
        new(16, 4, "Silence Channel", "todo", ConfigType.Channel, null, 3),
        new(17, 10, Spam.SilenceMessage, "todo", ConfigType.String, string.Empty, 4),

        new(1000, 1, "String", "default", ConfigType.String, "hi", 1000),
        new(1001, 2, "StringNull", "null", ConfigType.String, null, 1000),
        new(1002, 3, "Boolean", "default", ConfigType.Boolean, "1", 1000),
        new(1003, 4, "BooleanNull", "null", ConfigType.Boolean, null, 1000),
        new(1004, 5, "Int", "default", ConfigType.Int, "7", 1000),
        new(1005, 6, "IntNull", "null", ConfigType.Int, null, 1000),
        new(1006, 7, "Char", "default", ConfigType.Char, "h", 1000),
        new(1007, 8, "CharNull", "null", ConfigType.Char, null, 1000),
        new(1008, 9, "Role", "null", ConfigType.Role, null, 1000),
        new(1009, 10, "Channel", "null", ConfigType.Channel, null, 1000)
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
    public readonly string? DefaultValue;
    public readonly string Description;
    public readonly ConfigType Type;

    public ConfigOption(int i, int s, string n, string d, ConfigType t, string? df, int ci) : base(i, s, n)
    {
        Description = d;
        Type = t;
        DefaultValue = df;
        ConfigOptionCategoryId = ci;
    }

    public ConfigOptionCategory ConfigOptionCategory => ConfigOptionCategories.Instance.Get(ConfigOptionCategoryId);
}