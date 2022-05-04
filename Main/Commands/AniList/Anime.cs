using Anilist4Net;
using Anilist4Net.Enums;
using Common.Classes;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;
using Humanizer.Localisation;
using Main.Helper;

namespace Main.Commands.AniList;

public sealed class Anime : SlashCommand
{
    private readonly string _title;

    public Anime(InteractionContext ctx, string title) : base(ctx)
    {
        _title = title;
    }

    public override async Task RunAsync()
    {
        await Ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var anime = int.TryParse(_title, out var result)
            ? await new Client().GetMediaById(result)
            : await new Client().GetMediaBySearch(_title, MediaTypes.ANIME);

        if (anime == null ||
            !new[]
            {
                MediaFormats.TV, MediaFormats.ONA, MediaFormats.OVA, MediaFormats.MOVIE, MediaFormats.MOVIE,
                MediaFormats.MUSIC, MediaFormats.SPECIAL, MediaFormats.TV_SHORT
            }.Contains(anime.Format))
        {
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed($"\"{_title}\" not found"));
            return;
        }

        var embed = new DiscordEmbedBuilder();
        AniListHelper.AddCommonMediaFieldsTop(embed, anime);
        AddFields(embed, anime);
        AniListHelper.AddCommonMediaFieldsBottom(embed, anime);
        AniListHelper.AddCommonMediaEmbedProperties(embed, anime);
        var btn = AniListHelper.GetSiteButton(anime);
        await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()).AddComponents(btn));
    }

    #region Static methods

    private static void AddFields(DiscordEmbedBuilder embed, Media anime)
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

    #endregion
}