using Db.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;

namespace Spam.Events;

public static class SpamHelperOnMaxPressureExceeded
{
    public static async void HandleEvent(DiscordClient sender, MaxPressureExceededEventArgs args)
    {
        var lastMessage = args.SessionMessages.Last();

        if (lastMessage.Channel.GuildId == null || lastMessage.Channel.Guild == null)
        {
            throw new NullReferenceException(nameof(lastMessage.Channel.Guild));
        }

        var silenceRole = await ConfigHelper.Instance.GetRole("Silence Role", lastMessage.Channel.Guild);
        var modChannel = await ConfigHelper.Instance.GetChannel("Moderator Channel", lastMessage.Channel.Guild);

        var silenced = false;
        if (silenceRole != null)
        {
            var member = (DiscordMember) lastMessage.Author;
            await member.GrantRoleAsync(silenceRole, $"Pressure {args.UserPressure:N2} > {args.MaxPressure}");
            silenced = true;
        }

        var messagesDeleted = false;
        if (await ConfigHelper.Instance.GetBool(Db.Strings.Spam.DeleteMessages, lastMessage.Channel.GuildId.Value) ==
            true)
        {
            await lastMessage.Channel.DeleteMessagesAsync(args.SessionMessages);
            messagesDeleted = true;
        }

        if (modChannel == null)
        {
            return;
        }

        var embed = GetEmbed(args, lastMessage, silenced, messagesDeleted);
        await modChannel.SendMessageAsync(embed);
    }

    private static DiscordEmbed GetEmbed(MaxPressureExceededEventArgs args, DiscordMessage lastMessage, bool silenced,
        bool messagesDeleted)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Spam Detected");

        var a = lastMessage.Author;
        embed.WithDescription($"{a.Mention}{Environment.NewLine}{a.Username}#{a.Discriminator}");

        if (silenced || messagesDeleted)
        {
            var actionStrings = new List<string>();

            if (silenced)
            {
                actionStrings.Add("User was silenced.");
            }

            if (messagesDeleted)
            {
                actionStrings.Add("Messages were deleted.");
            }

            var n = Environment.NewLine;
            var silenceStr = $"{n}{n}{string.Join(n, actionStrings)}";
            embed.AddField("Actions Taken", silenceStr);
        }

        embed.AddField("Pressure", $"{args.UserPressure:N2} (max. {args.MaxPressure})", true);
        embed.AddField("Channel", lastMessage.Channel.Mention, true);

        embed.AddField("Message Content", lastMessage.Content);
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }
}