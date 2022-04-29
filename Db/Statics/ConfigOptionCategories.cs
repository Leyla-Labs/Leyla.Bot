using Db.Statics.BaseClasses;

namespace Db.Statics;

public class ConfigOptionCategories : StaticClass<ConfigOptionCategory>
{
    public ConfigOptionCategories() : base(new List<ConfigOptionCategory>
    {
        new(1, 1, "General", "todo"),
        new(2, 2, "Roles", "todo"),
        new(3, 3, "Channels", "todo")
    })
    {
    }
}

public class ConfigOptionCategory : StaticField
{
    public readonly string Description;

    public ConfigOptionCategory(int i, int s, string n, string d) : base(i, s, n)
    {
        Description = d;
    }
}