using Common.Enums;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Main.Events;

internal static class ClientOnGuildRoleDeleted
{
    public static async Task HandleEvent(DiscordClient sender, GuildRoleDeleteEventArgs e)
    {
        await DiscordEntityHelper.DeleteIfExists(DiscordEntityType.Role, e.Role.Id, e.Guild.Id);
    }
}