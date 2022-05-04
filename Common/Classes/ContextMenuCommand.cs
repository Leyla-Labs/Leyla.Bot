using DSharpPlus.SlashCommands;

namespace Common.Classes;

public abstract class ContextMenuCommand
{
    protected readonly ContextMenuContext Ctx;

    protected ContextMenuCommand(ContextMenuContext ctx)
    {
        Ctx = ctx;
    }
    
    public abstract Task RunAsync();
}