using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;

namespace Logs.Events;

internal static class ClientOnMessageUpdated
{
    public static async Task HandleEvent(DiscordClient sender, MessageUpdateEventArgs e)
    {
        if (e.Guild == null)
        {
            // do not log messages from direct messages
            return;
        }

        if (LeylaModuleHelper.UserIds.Contains(e.Author.Id))
        {
            // do not log messages from Leyla modules
            return;
        }

        var channel = await GuildConfigHelper.Instance.GetChannel("Log Channel", e.Guild);
        if (channel == null)
        {
            return;
        }

        var em = new DiscordEmbedBuilder();

        AddTitle(em);
        AddMsgAuthor(em, e);
        AddChannel(em, e);
        AddJumpLink(em, e);
        AddContentNew(em, e);
        AddContentOld(em, e);
        AddFooter(em, e);

        await channel.SendMessageAsync(em.Build());
    }

    private static void AddTitle(DiscordEmbedBuilder em)
    {
        em.WithTitle("Message edited");
    }

    private static void AddMsgAuthor(DiscordEmbedBuilder em, MessageUpdateEventArgs e)
    {
        if (e.Message.Author != null)
        {
            em.AddField("User", e.Message.Author.Mention, true);
        }
    }

    private static void AddChannel(DiscordEmbedBuilder em, MessageUpdateEventArgs e)
    {
        em.AddField("Channel", e.Message.Channel.Mention, true);
    }

    private static void AddJumpLink(DiscordEmbedBuilder em, MessageUpdateEventArgs e)
    {
        em.AddField("Message", $"[Link]({e.Message.JumpLink})", true);
    }

    private static void AddContentNew(DiscordEmbedBuilder em, MessageUpdateEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Message.Content))
        {
            em.AddField("Content (new)", e.Message.Content.Trim());
        }
    }

    private static void AddContentOld(DiscordEmbedBuilder em, MessageUpdateEventArgs e)
    {
        var s = e.MessageBefore == null
            ? "`not cached`"
            : !string.IsNullOrWhiteSpace(e.MessageBefore.Content)
                ? e.MessageBefore.Content.Trim()
                : null;

        if (s != null)
        {
            em.AddField("Content (old)", s);
        }
    }

    private static void AddFooter(DiscordEmbedBuilder em, MessageUpdateEventArgs e)
    {
        em.WithFooter(e.Message.CreationTimestamp < DateTimeOffset.Now.AddDays(-1)
                ? $"{e.Message.CreationTimestamp:dd. MMMM yyyy}"
                : e.Message.CreationTimestamp.Humanize(),
            "https://i.imgur.com/FkOFUCC.png");
    }
}