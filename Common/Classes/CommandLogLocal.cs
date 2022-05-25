using Common.Db.Models;

namespace Common.Classes;

public class CommandLogLocal : CommandLogBase
{
    public CommandLogLocal(CommandLog log)
    {
        Command = log.Command;
        RunAt = log.RunAt.ToUniversalTime();
        UserId = log.UserId;
        GuildId = log.Member.GuildId;
    }

    public CommandLogLocal(ulong guildId, ulong userId, string command, DateTime runAt)
    {
        UserId = userId;
        GuildId = guildId;
        Command = command;
        RunAt = runAt.ToUniversalTime();
    }
}