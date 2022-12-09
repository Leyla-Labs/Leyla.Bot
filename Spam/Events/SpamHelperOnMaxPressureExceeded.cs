using System.ComponentModel.DataAnnotations;
using Common.Enums;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;
using Spam.Extensions;

namespace Spam.Events;

internal static class SpamHelperOnMaxPressureExceeded
{
    public static async void HandleEvent(DiscordClient sender, MaxPressureExceededEventArgs args)
    {
        var lastMessage = args.SessionMessages.Last();

        var guild = lastMessage.Channel.Guild;
        var silenceRole = await ConfigHelper.Instance.GetRole("Silence Role", guild);
        var modChannel = await ConfigHelper.Instance.GetChannel("Moderator Channel", guild);
        var silenceChannel = await ConfigHelper.Instance.GetChannel("Silence Channel", guild);
        var silenceMessage = await ConfigHelper.Instance.GetString(Common.Strings.Spam.SilenceMessage, guild.Id);
        var timeoutDuration =
            await ConfigHelper.Instance.GetEnum<TimeoutDuration>(Common.Strings.Spam.Timeout, guild.Id);

        var member = (DiscordMember) lastMessage.Author;
        var reason = $"Pressure {args.UserPressure:N2} > {args.MaxPressure}";

        if (timeoutDuration != TimeoutDuration.None)
        {
            var until = DateTime.Now.AddMinutes(timeoutDuration.GetMinutes());
            await member.TimeoutAsync(until, reason);
        }

        var silenced = false;
        if (silenceRole != null)
        {
            await member.GrantRoleAsync(silenceRole, reason);
            silenced = true;
        }

        var silenceMessageSent = false;
        if (silenced && silenceChannel != null && !string.IsNullOrWhiteSpace(silenceMessage))
        {
            await silenceChannel.SendMessageAsync($"{member.Mention} {silenceMessage}");
            silenceMessageSent = true;
        }

        var messagesDeleted = 0;
        if (await ConfigHelper.Instance.GetBool(Common.Strings.Spam.DeleteMessages, guild.Id) ==
            true)
        {
            var messagesAfter = (await lastMessage.Channel.GetMessagesAfterAsync(lastMessage.Id))
                .Where(x => x.Author.Id == member.Id);

            var messagesToDelete = args.SessionMessages;
            messagesToDelete.AddRange(messagesAfter);
            await lastMessage.Channel.DeleteMessagesAsync(messagesToDelete);

            messagesDeleted = messagesToDelete.Count;
        }

        if (modChannel == null)
        {
            return;
        }

        var embed = GetEmbed(args, lastMessage, timeoutDuration, silenced ? silenceRole : null,
            silenceMessageSent ? silenceChannel : null, messagesDeleted);
        await modChannel.SendMessageAsync(embed);
    }

    private static DiscordEmbed GetEmbed(MaxPressureExceededEventArgs args, DiscordMessage lastMessage,
        TimeoutDuration timeout, DiscordRole? silenceRole, DiscordChannel? silenceChannel, int messagesDeleted)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Spam Detected");

        var a = lastMessage.Author;
        embed.WithDescription($"{a.Mention}{Environment.NewLine}{a.Username}#{a.Discriminator}");

        if (timeout != TimeoutDuration.None || silenceRole != null || silenceChannel != null || messagesDeleted > 0)
        {
            var actionStrings = new List<string>();

            if (timeout != TimeoutDuration.None)
            {
                actionStrings.Add($"User was timed out for {timeout.GetAttribute<DisplayAttribute>()?.Name}.");
            }

            if (silenceRole != null)
            {
                actionStrings.Add($"User was silenced ({silenceRole.Name}).");
            }

            if (silenceChannel != null)
            {
                actionStrings.Add($"User was pinged in {silenceChannel.Mention}.");
            }

            if (messagesDeleted > 0)
            {
                actionStrings.Add($" {messagesDeleted} messages were deleted.");
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