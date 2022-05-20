namespace Common.Db.Models;

public class Stash
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;

    public ulong? RequiredRoleId { get; set; }
    public DiscordEntity? RequiredRole { get; set; }

    public ICollection<StashEntry> StashEntries { get; set; } = null!;
}