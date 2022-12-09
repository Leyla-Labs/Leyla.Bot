using DSharpPlus.SlashCommands.EventArgs;

namespace Common.Handler.BaseClasses;

public abstract class SlashCommandErrorHandler
{
    private protected readonly SlashCommandErrorEventArgs Args;

    protected SlashCommandErrorHandler(SlashCommandErrorEventArgs e)
    {
        Args = e;
    }

    public abstract Task HandleExceptionAsync();
}