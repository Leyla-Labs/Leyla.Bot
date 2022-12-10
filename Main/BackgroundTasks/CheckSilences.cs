using Common.Helper;
using Common.Strings;
using DNTCommon.Web.Core;
using DSharpPlus.Entities;

namespace Main.BackgroundTasks;

internal class CheckSilences : IScheduledTask
{
    public bool IsShuttingDown { get; set; }

    public async Task RunAsync()
    {
        if (IsShuttingDown)
        {
            return;
        }

        var membersToUnsilence = SilenceHelper.Instance.GetMembersToUnsilence();

        if (!membersToUnsilence.Any())
        {
            return;
        }

        foreach (var member in membersToUnsilence)
        {
            var role = await GuildConfigHelper.Instance.GetRoleAsync(Config.Roles.Silence.Name, member.Guild);

            var updatedMember = await member.Guild.GetMemberAsync(member.Id);

            if (role != null && updatedMember != null && updatedMember.Roles.Select(x => x.Id).Contains(role.Id))
            {
                await member.RevokeRoleAsync(role);
            }

            SilenceHelper.Instance.RemoveTimedSilence(member.Guild.Id, member.Id);

            if (updatedMember != null)
            {
                await SendModMessageAsync(updatedMember);
            }
        }
    }

    private static async Task SendModMessageAsync(DiscordMember member)
    {
        if (await GuildConfigHelper.Instance.GetChannelAsync(Config.Channels.Mod.Name, member.Guild) is { } modChannel)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithTitle("Silence Expired");
            embed.WithDescription($"The silence for {member.Mention} expired. The silence role was removed.");
            embed.WithColor(DiscordColor.Blurple);

            await modChannel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed.Build()));
        }
    }
}