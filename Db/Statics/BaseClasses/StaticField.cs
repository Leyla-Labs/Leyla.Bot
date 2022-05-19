namespace Db.Statics.BaseClasses;

public class StaticField
{
    public readonly int Id;
    public readonly string Name;
    public readonly int SortId;

    protected StaticField(int id, int sortId, string name)
    {
        Id = id;
        SortId = sortId;
        Name = name;
    }
}