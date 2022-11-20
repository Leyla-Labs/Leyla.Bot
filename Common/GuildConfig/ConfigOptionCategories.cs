using Common.Classes;
using Common.Comparer;
using Common.Enums;
using Common.Interfaces;
using static Common.Strings.Config;

namespace Common.GuildConfig;

public class ConfigOptionCategories : IdentifiableSetProvider<ConfigOptionCategory>, ISetProvider<ConfigOptionCategory>
{
    private ConfigOptionCategories() : base(CreateSet())
    {
    }

    public static SortedSet<ConfigOptionCategory> CreateSet()
    {
        return new SortedSet<ConfigOptionCategory>(new IdentifiableComparer())
        {
            new(2, 2, Roles.Category, LeylaModule.Main),
            new(3, 3, Channels.Category, LeylaModule.Main),
            new(4, 4, Spam.Category, LeylaModule.Spam),
            new(5, 5, Raid.Category, LeylaModule.Spam)
        };
    }

    #region Singleton

    private static readonly Lazy<ConfigOptionCategories> Lazy = new(() => new ConfigOptionCategories());
    public static ConfigOptionCategories Instance => Lazy.Value;

    #endregion
}