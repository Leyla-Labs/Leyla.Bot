using Db.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;

namespace Logs.Events;

public static class ClientOnGuildMemberRemoved
{
    public static async Task HandleEvent(DiscordClient sender, GuildMemberRemoveEventArgs e)
    {
        var channel = await ConfigHelper.Instance.GetChannel("Moderator Channel", e.Guild);
        if (channel == null) return;

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("User left");
        embed.WithColor(DiscordColor.IndianRed);
        embed.WithDescription(
            $"`{e.Member.Id}`{Environment.NewLine}{e.Member.Username}#{e.Member.Discriminator}");
        if (e.Member.JoinedAt != default) embed.WithFooter($"Joined {e.Member.JoinedAt.Humanize()}");
        embed.WithThumbnail(e.Member.AvatarUrl);
        await channel.SendMessageAsync(embed.Build());
    }
}