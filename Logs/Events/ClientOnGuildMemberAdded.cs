using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;

namespace Logs.Events;

public static class ClientOnGuildMemberAdded
{
    public static async Task HandleEvent(DiscordClient sender, GuildMemberAddEventArgs e)
    {
        var channel = await ConfigHelper.Instance.GetChannel("Moderator Channel", e.Guild);
        if (channel == null)
        {
            return;
        }

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("User joined");
        embed.WithColor(DiscordColor.CornflowerBlue);
        embed.WithDescription(
            $"{e.Member.Mention}{Environment.NewLine}{e.Member.Username}#{e.Member.Discriminator}");
        embed.WithFooter($"Created {e.Member.CreationTimestamp.Humanize()}");
        embed.WithThumbnail(e.Member.AvatarUrl);
        await channel.SendMessageAsync(embed.Build());
    }
}