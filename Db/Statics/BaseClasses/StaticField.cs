namespace Db.Statics.BaseClasses;

public class StaticField
{
    public readonly int Id;
    public readonly string Name;
    public readonly int SortId;

    protected StaticField(int i, int s, string n)
    {
        Id = i;
        SortId = s;
        Name = n;
    }
}