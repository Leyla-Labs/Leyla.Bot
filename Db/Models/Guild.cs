namespace Db.Models;

public class Guild
{
    public ulong Id { get; set; }

    public ICollection<Config> Configs { get; set; } = null!;
    public ICollection<DiscordEntity> DiscordEntities { get; set; } = null!;
    public ICollection<Member> Members { get; set; } = null!;
    public ICollection<SelfAssignMenu> SelfAssignMenus { get; set; } = null!;
    public ICollection<Stash> Stashes { get; set; } = null!;
}