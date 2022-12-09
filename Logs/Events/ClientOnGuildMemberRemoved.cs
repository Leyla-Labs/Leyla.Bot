using Common.Helper;
using Common.Interfaces;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;

namespace Logs.Events;

internal abstract class ClientOnGuildMemberRemoved : IEventHandler<GuildMemberRemoveEventArgs>
{
    public static async Task HandleEventAsync(DiscordClient sender, GuildMemberRemoveEventArgs e)
    {
        var channel = await ConfigHelper.Instance.GetChannel("Moderator Channel", e.Guild);
        if (channel == null)
        {
            return;
        }

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("User left");
        embed.WithColor(DiscordColor.IndianRed);
        embed.WithDescription(
            $"`{e.Member.Id}`{Environment.NewLine}{e.Member.Username}#{e.Member.Discriminator}");
        if (e.Member.JoinedAt != default)
        {
            embed.WithFooter($"Joined {e.Member.JoinedAt.Humanize()}");
        }

        embed.WithThumbnail(e.Member.AvatarUrl);
        await channel.SendMessageAsync(embed.Build());
    }
}