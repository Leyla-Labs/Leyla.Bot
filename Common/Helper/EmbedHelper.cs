using DSharpPlus.Entities;

namespace Common.Helper;

public static class EmbedHelper
{
    internal static DiscordEmbed GetErrorBuilder(string title, string? description)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(title);

        if (description != null) embed.WithDescription(description);

        embed.WithColor(DiscordColor.Red);
        return embed.Build();
    }
}