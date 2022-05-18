using DSharpPlus.SlashCommands;

namespace Db.Classes;

public abstract class ContextMenuCommand : CommandBase
{
    protected readonly ContextMenuContext Ctx;

    protected ContextMenuCommand(ContextMenuContext ctx)
    {
        Ctx = ctx;
    }
}