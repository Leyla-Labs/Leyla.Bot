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

        if (lastMessage.Channel.Guild == null)
        {
            throw new NullReferenceException(nameof(lastMessage.Channel.Guild));
        }

        var guild = lastMessage.Channel.Guild;
        var silenceRole = await ConfigHelper.Instance.GetRole("Silence Role", guild);
        var modChannel = await ConfigHelper.Instance.GetChannel("Moderator Channel", guild);
        var silenceChannel = await ConfigHelper.Instance.GetChannel("Silence Channel", guild);
        var silenceMessage = await ConfigHelper.Instance.GetString(Db.Strings.Spam.SilenceMessage, guild.Id);

        var member = (DiscordMember) lastMessage.Author;

        var silenced = false;
        if (silenceRole != null)
        {
            await member.GrantRoleAsync(silenceRole, $"Pressure {args.UserPressure:N2} > {args.MaxPressure}");
            silenced = true;
        }

        var silenceMessageSent = false;
        if (silenced && silenceChannel != null && !string.IsNullOrWhiteSpace(silenceMessage))
        {
            await silenceChannel.SendMessageAsync($"{member.Mention} {silenceMessage}");
            silenceMessageSent = true;
        }

        var messagesDeleted = false;
        if (await ConfigHelper.Instance.GetBool(Db.Strings.Spam.DeleteMessages, guild.Id) ==
            true)
        {
            var messagesAfter = (await lastMessage.Channel.GetMessagesAfterAsync(lastMessage.Id))
                .Where(x => x.Author.Id == member.Id);

            var messagesToDelete = args.SessionMessages;
            messagesToDelete.AddRange(messagesAfter);
            await lastMessage.Channel.DeleteMessagesAsync(messagesToDelete);

            messagesDeleted = true;
        }

        if (modChannel == null)
        {
            return;
        }

        var embed = GetEmbed(args, lastMessage, silenced, silenceMessageSent, messagesDeleted);
        await modChannel.SendMessageAsync(embed);
    }

    private static DiscordEmbed GetEmbed(MaxPressureExceededEventArgs args, DiscordMessage lastMessage, bool silenced,
        bool silenceMessageSent, bool messagesDeleted)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Spam Detected");

        var a = lastMessage.Author;
        embed.WithDescription($"{a.Mention}{Environment.NewLine}{a.Username}#{a.Discriminator}");

        if (silenced || silenceMessageSent || messagesDeleted)
        {
            var actionStrings = new List<string>();

            if (silenced)
            {
                actionStrings.Add("User was silenced.");
            }

            if (silenceMessageSent)
            {
                actionStrings.Add("User was pinged in silence channel.");
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