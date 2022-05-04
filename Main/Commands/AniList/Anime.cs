using Anilist4Net;
using Anilist4Net.Enums;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;
using Humanizer.Localisation;
using Main.Helper;

namespace Main.Commands.AniList;

public static class Anime
{
    public static async Task RunSlash(InteractionContext ctx, string title)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var anime = int.TryParse(title, out var result)
            ? await new Client().GetMediaById(result)
            : await new Client().GetMediaBySearch(title, MediaTypes.ANIME);

        if (anime == null ||
            !new[]
            {
                MediaFormats.TV, MediaFormats.ONA, MediaFormats.OVA, MediaFormats.MOVIE, MediaFormats.MOVIE,
                MediaFormats.MUSIC, MediaFormats.SPECIAL, MediaFormats.TV_SHORT
            }.Contains(anime.Format))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed($"\"{title}\" not found"));
            return;
        }

        var embed = new DiscordEmbedBuilder();
        AniListHelper.AddCommonMediaFieldsTop(embed, anime);
        embed.AddFields(anime);
        AniListHelper.AddCommonMediaFieldsBottom(embed, anime);
        AniListHelper.AddCommonMediaEmbedProperties(embed, anime);
        var btn = AniListHelper.GetSiteButton(anime);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()).AddComponents(btn));
    }

    private static void AddFields(this DiscordEmbedBuilder embed, Media anime)
    {
        if (anime.Episodes > 1)
        {
            embed.AddField("Episodes", anime.Episodes.ToString(), true);
        }

        if (anime.Format != MediaFormats.MOVIE || anime.Duration == null)
        {
            return;
        }

        var duration = TimeSpan.FromMinutes((int) anime.Duration);
        var durationString = duration.Humanize(maxUnit: TimeUnit.Hour, minUnit: TimeUnit.Minute, precision: 2);

        embed.AddField("Duration", durationString, true);
    }
}