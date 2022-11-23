using Common.Classes;
using Common.Enums;
using Common.Interfaces;

namespace Common.Records;

public record GuildConfigOptionCategory
    (int Id, int SortId, ConfigStrings ConfigStrings, LeylaModule Module) : IIdentifiable
{
    public string Name => ConfigStrings.Name;
}