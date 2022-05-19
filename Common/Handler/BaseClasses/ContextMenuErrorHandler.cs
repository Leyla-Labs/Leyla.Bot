using DSharpPlus.SlashCommands.EventArgs;

namespace Common.Handler.BaseClasses;

public abstract class ContextMenuErrorHandler
{
    private protected readonly ContextMenuErrorEventArgs Args;

    protected ContextMenuErrorHandler(ContextMenuErrorEventArgs e)
    {
        Args = e;
    }

    public abstract Task HandleException();
}