using Db.Enums;

namespace Db.Models;

public class DiscordEntity
{
    public ulong Id { get; set; }

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;

    public DiscordEntityType DiscordEntityType { get; set; }

    public ICollection<SelfAssignMenu> SelfAssignMenus { get; set; } = null!;

    public ICollection<SelfAssignMenuDiscordEntityAssignment> SelfAssignMenuDiscordEntityAssignments { get; set; } =
        null!;
}