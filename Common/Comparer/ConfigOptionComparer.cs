using Common.Records;

namespace Common.Comparer;

public class ConfigOptionComparer : IComparer<GuildConfigOption>
{
#pragma warning disable CS8767
    public int Compare(GuildConfigOption x, GuildConfigOption y)
#pragma warning restore CS8767
    {
        return x.GuildConfigOptionCategory.SortId == y.GuildConfigOptionCategory.SortId
            ? new IdentifiableComparer().Compare(x, y)
            : x.GuildConfigOptionCategory.Id > y.GuildConfigOptionCategory.Id
                ? 1
                : 0;
    }
}