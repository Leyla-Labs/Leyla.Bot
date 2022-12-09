namespace Common.Classes;

// skipcq CS-R1078
public abstract class CommandLogBase
{
    public string Command { get; set; } = null!;

    public DateTime RunAt { get; set; }

    public ulong UserId { get; set; }
    public ulong GuildId { get; set; }
}