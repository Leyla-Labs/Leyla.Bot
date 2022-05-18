using DSharpPlus;
using DSharpPlus.EventArgs;
using Spam.Helper;

namespace Spam.Events;

public class ClientOnMessageCreated
{
    public static async Task HandleEvent(DiscordClient sender, MessageCreateEventArgs e)
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

        await SpamHelper.Instance.ProcessMessage(sender, e.Message);
    }
}