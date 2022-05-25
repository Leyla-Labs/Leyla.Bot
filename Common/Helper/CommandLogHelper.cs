using Common.Classes;
using Common.Db;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Common.Helper;

public class CommandLogHelper
{
    private readonly List<CommandLogLocal> _commandLogs = new();

    private CommandLogHelper()
    {
    }

    public void AddLog(InteractionContext ctx)
    {
        _commandLogs.Add(new CommandLogLocal(ctx.Guild.Id, ctx.User.Id, ctx.CommandName, DateTime.Now));
    }

    public List<CommandLogLocal> GetAndClear()
    {
        var list = _commandLogs.ToList();
        _commandLogs.Clear();
        return list;
    }

    public async Task<List<CommandLogLocal>> GetRecent(ulong guildId, int n)
    {
        var recentLocal = _commandLogs.Where(x => x.GuildId == guildId).OrderByDescending(x => x.RunAt).Take(n)
            .ToList();

        await using var context = new DatabaseContext();

        var recentDb = await context.CommandLogs.Where(x =>
                x.GuildId == guildId)
            .OrderByDescending(x => x.RunAt)
            .Take(n - recentLocal.Count)
            .Select(x => new CommandLogLocal(x))
            .ToListAsync();

        recentLocal.AddRange(recentDb);
        return recentLocal;
    }

    #region Singleton

    private static readonly Lazy<CommandLogHelper> Lazy = new(() => new CommandLogHelper());
    public static CommandLogHelper Instance => Lazy.Value;

    #endregion
}