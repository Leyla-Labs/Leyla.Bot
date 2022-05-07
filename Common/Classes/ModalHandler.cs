using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Common.Classes;

public abstract class ModalHandler
{
    protected readonly ModalSubmitEventArgs EventArgs;
    protected readonly DiscordClient Sender;

    protected ModalHandler(DiscordClient sender, ModalSubmitEventArgs eventArgs)
    {
        EventArgs = eventArgs;
        Sender = sender;
    }

    public abstract Task RunAsync();
}