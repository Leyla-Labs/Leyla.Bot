using Common.Helper;
using DSharpPlus.Entities;

namespace Common.Extensions;

public static class DiscordWebhookBuilderExtension
{
    public static DiscordWebhookBuilder AddErrorEmbed(this DiscordWebhookBuilder b, string title,
        string description)
    {
        b.AddEmbed(EmbedHelper.GetErrorBuilder(title, description));
        return b;
    }

    public static DiscordWebhookBuilder AddErrorEmbed(this DiscordWebhookBuilder b, string title)
    {
        b.AddEmbed(EmbedHelper.GetErrorBuilder(title, null));
        return b;
    }
}