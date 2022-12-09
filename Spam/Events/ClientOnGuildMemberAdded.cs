using Common.Helper;
using Common.Interfaces;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Spam.Helper;

namespace Spam.Events;

internal abstract class ClientOnGuildMemberAdded : IEventHandler<GuildMemberAddEventArgs>
{
    public static async Task HandleEventAsync(DiscordClient sender, GuildMemberAddEventArgs e)
    {
        await SilenceHelper.Instance.ProcessUserJoinedAsync(e.Guild, e.Member);

        var raidMode = await GuildConfigHelper.Instance.GetBoolAsync(Config.Raid.RaidMode.Name, e.Guild.Id);

        if (raidMode == true)
        {
            var raidRole = await GuildConfigHelper.Instance.GetRoleAsync(Config.Raid.RaidRole.Name, e.Guild);
            await e.Member.GrantRoleAsync(raidRole);

            await RaidHelper.MentionMembersInRaidChannelAsync(e.Guild, e.Member);
        }
        else
        {
            await RaidHelper.Instance.AddMemberAsync(sender, e.Member);
        }
    }
}