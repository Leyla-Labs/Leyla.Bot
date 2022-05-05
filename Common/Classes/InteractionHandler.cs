using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Common.Classes;

public abstract class InteractionHandler
{
    protected readonly ComponentInteractionCreateEventArgs EventArgs;
    protected readonly DiscordClient Sender;

    protected InteractionHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        Sender = sender;
        EventArgs = e;
    }

    public abstract Task RunAsync();
}