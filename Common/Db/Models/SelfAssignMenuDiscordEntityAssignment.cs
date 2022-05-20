namespace Common.Db.Models;

public class SelfAssignMenuDiscordEntityAssignment
{
    public int Id { get; set; }

    public int SelfAssignMenuId { get; set; }
    public SelfAssignMenu SelfAssignMenu { get; set; } = null!;

    public ulong DiscordEntityId { get; set; }
    public DiscordEntity DiscordEntity { get; set; } = null!;
}