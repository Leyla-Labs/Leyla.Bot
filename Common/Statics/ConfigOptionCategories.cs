using Common.Classes;
using Common.Enums;
using Common.Statics.BaseClasses;
using static Common.Strings.Config;

namespace Common.Statics;

public sealed class ConfigOptionCategories : StaticClass<ConfigOptionCategory>
{
    private ConfigOptionCategories() : base(new List<ConfigOptionCategory>
    {
        // id i reserved for general category later
        new(2, 2, Roles.Category, LeylaModule.Main),
        new(3, 3, Channels.Category, LeylaModule.Main),
        new(4, 4, Spam.Category, LeylaModule.Spam)
    })
    {
    }

    #region Singleton

    private static readonly Lazy<ConfigOptionCategories> Lazy = new(() => new ConfigOptionCategories());
    public static ConfigOptionCategories Instance => Lazy.Value;

    #endregion
}

public class ConfigOptionCategory : StaticField
{
    public readonly string Description;
    public readonly LeylaModule Module;

    public ConfigOptionCategory(int id, int sortId, DisplayString displayString, LeylaModule module)
        : base(id, sortId, displayString.Name)
    {
        Description = displayString.Description;
        Module = module;
    }
}