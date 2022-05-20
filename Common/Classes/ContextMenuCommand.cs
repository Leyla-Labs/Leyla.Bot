using DSharpPlus.SlashCommands;

namespace Common.Classes;

public abstract class ContextMenuCommand : CommandBase
{
    protected readonly ContextMenuContext Ctx;

    protected ContextMenuCommand(ContextMenuContext ctx)
    {
        Ctx = ctx;
    }
}