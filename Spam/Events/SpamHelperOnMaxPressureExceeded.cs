using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;

namespace Spam.Events;

public static class SpamHelperOnMaxPressureExceeded
{
    public static async void HandleEvent(DiscordClient sender, MaxPressureExceededEventArgs args)
    {
        var id = Convert.ToUInt64(Environment.GetEnvironmentVariable("MAIN_CHANNEL"));
        var channel = await sender.GetChannelAsync(id);

        var embed = GetEmbed(args);
        await channel.SendMessageAsync(embed);
    }

    private static DiscordEmbed GetEmbed(MaxPressureExceededEventArgs args)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Spam detected");
        embed.WithDescription($"{args.UserPressure} (max. {args.MaxPressure})");
        embed.WithColor(DiscordColor.Blurple);
        embed.AddField("Content", args.Message.Content);
        return embed.Build();
    }
}