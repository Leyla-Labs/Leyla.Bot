using Common.Classes;

namespace Common.Comparer;

public class ConfigOptionComparer : IComparer<ConfigOption>
{
#pragma warning disable CS8767
    public int Compare(ConfigOption x, ConfigOption y)
#pragma warning restore CS8767
    {
        return x.ConfigOptionCategory.SortId == y.ConfigOptionCategory.SortId
            ? new IdentifiableComparer().Compare(x, y)
            : x.ConfigOptionCategory.Id > y.ConfigOptionCategory.Id
                ? 1
                : 0;
    }
}