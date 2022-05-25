using Common.Classes;
using DSharpPlus.SlashCommands;

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

    #region Singleton

    private static readonly Lazy<CommandLogHelper> Lazy = new(() => new CommandLogHelper());
    public static CommandLogHelper Instance => Lazy.Value;

    #endregion
}