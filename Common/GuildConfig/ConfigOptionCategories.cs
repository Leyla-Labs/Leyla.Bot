using Common.Classes;
using Common.Comparer;
using Common.Enums;
using Common.Interfaces;
using Common.Records;
using static Common.Strings.Config;

namespace Common.GuildConfig;

public class ConfigOptionCategories : IdentifiableSetProvider<GuildConfigOptionCategory>,
    ISetProvider<GuildConfigOptionCategory>
{
    private ConfigOptionCategories() : base(CreateSet())
    {
    }

    public static SortedSet<GuildConfigOptionCategory> CreateSet()
    {
        return new SortedSet<GuildConfigOptionCategory>(new IdentifiableComparer())
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