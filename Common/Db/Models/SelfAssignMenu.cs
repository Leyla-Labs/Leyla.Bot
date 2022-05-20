namespace Common.Db.Models;

public class SelfAssignMenu
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;

    public ulong? RequiredRoleId { get; set; }
    public DiscordEntity? RequiredRole { get; set; }

    public ICollection<SelfAssignMenuDiscordEntityAssignment> SelfAssignMenuDiscordEntityAssignments { get; set; } =
        null!;
}