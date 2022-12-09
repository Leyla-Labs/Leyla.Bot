using Common.Enums;
using Common.Helper;
using Common.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Main.Events;

internal abstract class ClientOnGuildRoleDeleted : IEventHandler<GuildRoleDeleteEventArgs>
{
    public static async Task HandleEventAsync(DiscordClient sender, GuildRoleDeleteEventArgs e)
    {
        await DiscordEntityHelper.DeleteIfExists(DiscordEntityType.Role, e.Role.Id, e.Guild.Id);
    }
}