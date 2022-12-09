using Common.Helper;
using Common.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Spam.Events;

internal abstract class ClientOnGuildMemberRemoved : IEventHandler<GuildMemberRemoveEventArgs>
{
    public static async Task HandleEventAsync(DiscordClient sender, GuildMemberRemoveEventArgs e)
    {
        await SilenceHelper.Instance.ProcessUserLeftAsync(e.Guild, e.Member);
    }
}