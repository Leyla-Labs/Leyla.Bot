using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;

namespace Logs.Events;

internal static class ClientOnMessageDeleted
{
    public static async Task HandleEvent(DiscordClient sender, MessageDeleteEventArgs e)
    {
        if (e.Guild == null)
        {
            // do not log messages from direct messages
            return;
        }

        var channel = await ConfigHelper.Instance.GetChannel("Log Channel", e.Guild);
        if (channel == null)
        {
            return;
        }

        var em = new DiscordEmbedBuilder();
        AddTitle(em, e);
        AddMsgAuthor(em, e);
        AddChannel(em, e);
        AddContent(em, e);
        AddEmbed(em, e);
        AddFooter(em, e);

        await channel.SendMessageAsync(em.Build());
    }

    private static void AddTitle(DiscordEmbedBuilder em, MessageDeleteEventArgs e)
    {
        var titleSuffix = e.Message.Author == null ? " (not cached)" : string.Empty;
        em.WithTitle($"Message deleted{titleSuffix}");
    }

    private static void AddMsgAuthor(DiscordEmbedBuilder em, MessageDeleteEventArgs e)
    {
        if (e.Message.Author != null)
        {
            em.AddField("User", e.Message.Author.Mention, true);
        }
    }

    private static void AddChannel(DiscordEmbedBuilder em, MessageDeleteEventArgs e)
    {
        em.AddField("Channel", e.Message.Channel.Mention, true);
    }

    private static void AddContent(DiscordEmbedBuilder em, MessageDeleteEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Message.Content))
        {
            em.AddField("Content", e.Message.Content.Trim());
        }
    }

    private static void AddEmbed(DiscordEmbedBuilder em, MessageDeleteEventArgs e)
    {
        if (e.Message.Embeds.Count == 0)
        {
            return;
        }

        foreach (var mEmb in e.Message.Embeds)
        {
            if (!mEmb.Type.Equals("image"))
            {
                continue;
            }

            if (DiscordHelper.ImageFormats().All(x => !mEmb.Url.ToString().EndsWith($".{x}")))
            {
                continue;
            }

            em.WithImageUrl(mEmb.Url);
            break;
        }
    }

    private static void AddFooter(DiscordEmbedBuilder em, MessageDeleteEventArgs e)
    {
        em.WithFooter(e.Message.CreationTimestamp < DateTimeOffset.Now.AddDays(-1)
                ? $"{e.Message.CreationTimestamp:dd. MMMM yyyy}"
                : e.Message.CreationTimestamp.Humanize(),
            "https://i.imgur.com/FkOFUCC.png");
    }
}