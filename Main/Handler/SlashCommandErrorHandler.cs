using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Main.Interfaces;

namespace Main.Handler;

public abstract class SlashCommandErrorHandler
{
    private protected readonly SlashCommandErrorEventArgs _args;

    protected SlashCommandErrorHandler(SlashCommandErrorEventArgs e)
    {
        _args = e;
    }

    public abstract Task HandleException();
}