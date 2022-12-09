using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Spam.Helper;

namespace Spam.Events;

internal static class ClientOnGuildMemberAdded
{
    public static async Task HandleEvent(DiscordClient sender, GuildMemberAddEventArgs e)
    {
        await SilenceHelper.Instance.ProcessUserJoined(e.Guild, e.Member);

        var raidMode = await GuildConfigHelper.Instance.GetBoolAsync(Config.Raid.RaidMode.Name, e.Guild.Id);

        if (raidMode == true)
        {
            var raidRole = await GuildConfigHelper.Instance.GetRoleAsync(Config.Raid.RaidRole.Name, e.Guild);
            await e.Member.GrantRoleAsync(raidRole);

            await RaidHelper.MentionMembersInRaidChannel(e.Guild, e.Member);
        }
        else
        {
            await RaidHelper.Instance.AddMember(sender, e.Member);
        }
    }
}