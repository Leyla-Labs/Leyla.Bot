using Common.Helper;
using DSharpPlus.Entities;

namespace Common.Extensions;

public static class DiscordInteractionResponseBuilderExtension
{
    public static DiscordInteractionResponseBuilder AddErrorEmbed(this DiscordInteractionResponseBuilder b,
        string title, string? description = null)
    {
        b.AddEmbed(EmbedHelper.GetErrorBuilder(title, description));
        return b;
    }
}