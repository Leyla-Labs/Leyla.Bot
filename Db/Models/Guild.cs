namespace Db.Models;

public class Guild
{
    public ulong Id { get; set; }

    public ICollection<DiscordEntity> DiscordEntities { get; set; } = null!;
    public ICollection<Config> Configs { get; set; } = null!;
}