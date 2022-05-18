using Db.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;

namespace Spam.Events;

public static class SpamHelperOnMaxPressureExceeded
{
    public static async void HandleEvent(DiscordClient sender, MaxPressureExceededEventArgs args)
    {
        var silenceRole = await ConfigHelper.Instance.GetRole("Silence Role", args.Message.Channel.Guild);
        var modChannel = await ConfigHelper.Instance.GetChannel("Moderator Channel", args.Message.Channel.Guild);

        var silenced = false;
        if (silenceRole != null)
        {
            var member = (DiscordMember) args.Message.Author;
            await member.GrantRoleAsync(silenceRole, $"Pressure {args.UserPressure:N2} > {args.MaxPressure}");
            silenced = true;
        }

        if (modChannel == null)
        {
            return;
        }

        var embed = GetEmbed(args, silenced);
        await modChannel.SendMessageAsync(embed);
    }

    private static DiscordEmbed GetEmbed(MaxPressureExceededEventArgs args, bool silenced)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Spam Detected");

        var a = args.Message.Author;
        embed.WithDescription($"{a.Mention}{Environment.NewLine}{a.Username}#{a.Discriminator}");

        if (silenced)
        {
            var silenceStr = $"{Environment.NewLine}{Environment.NewLine}User was silenced.";
            embed.AddField("Actions Taken", silenceStr);
        }

        embed.AddField("Pressure", $"{args.UserPressure:N2} (max. {args.MaxPressure})", true);
        embed.AddField("Channel", args.Message.Channel.Mention, true);

        embed.AddField("Message Content", args.Message.Content);
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }
}