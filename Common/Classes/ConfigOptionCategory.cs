using Common.Enums;
using Common.Interfaces;

namespace Common.Classes;

public class ConfigOptionCategory : IIdentifiable
{
    public ConfigOptionCategory(int id, int sortId, DisplayString displayString, LeylaModule module)
    {
        Id = id;
        SortId = sortId;
        Name = displayString.Name;
        Description = displayString.Description;
        Module = module;
    }

    public LeylaModule Module { get; }

    public int Id { get; }
    public string Name { get; }
    public string Description { get; }
    public int SortId { get; }
}