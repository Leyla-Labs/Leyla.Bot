using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Spam.Helper;

namespace Spam.Events;

public static class ClientOnGuildMemberAdded
{
    public static async Task HandleEvent(DiscordClient sender, GuildMemberAddEventArgs e)
    {
        await SilenceHelper.Instance.ProcessUserJoined(e.Guild, e.Member);

        var raidMode = await ConfigHelper.Instance.GetBool(Config.Raid.RaidMode.Name, e.Guild.Id);

        if (raidMode == true)
        {
            var raidRole = await ConfigHelper.Instance.GetRole(Config.Raid.RaidRole.Name, e.Guild);
            await e.Member.GrantRoleAsync(raidRole);

            await RaidHelper.MentionMembersInRaidChannel(e.Guild, e.Member);
        }
        else
        {
            await RaidHelper.Instance.AddMember(sender, e.Member);
        }
    }
}