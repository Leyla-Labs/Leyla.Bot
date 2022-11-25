using Common.Classes;
using Common.Comparer;
using Common.Enums;
using Common.Interfaces;
using Common.Records;
using static Common.Strings.Config;

namespace Common.GuildConfig;

public class GuildConfigOptionCategories : IdentifiableSetProvider<ConfigOptionCategory>,
    ISetProvider<ConfigOptionCategory>
{
    private GuildConfigOptionCategories() : base(CreateSet())
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

    private static readonly Lazy<GuildConfigOptionCategories> Lazy = new(() => new GuildConfigOptionCategories());
    public static GuildConfigOptionCategories Instance => Lazy.Value;

    #endregion
}