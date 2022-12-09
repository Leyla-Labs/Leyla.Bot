using Common.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Spam.Helper;

namespace Spam.Events;

internal abstract class ClientOnMessageCreated : IEventHandler<MessageCreateEventArgs>
{
    public static async Task HandleEventAsync(DiscordClient sender, MessageCreateEventArgs e)
    {
        if (e.Channel?.GuildId == null)
        {
            // TODO handle this
            return;
        }

        if (e.Author.IsBot)
        {
            return;
        }

        await SpamHelper.Instance.ProcessMessageAsync(sender, e.Message);
    }
}