using Anilist4Net;
using DSharpPlus.Entities;
using Main.Extensions;

namespace Main.Helper;

public static class AniListHelper
{
    public static void AddCommonMediaFieldsTop(DiscordEmbedBuilder embed, Media media)
    {
        embed.WithTitle(media.EnglishTitle ?? media.RomajiTitle ?? media.NativeTitle);
        embed.WithDescription($"{media.RomajiTitle}{Environment.NewLine}{media.NativeTitle}");
        embed.AddField("Format", media.Format.ToFriendlyString(), true);
        embed.AddField("Released", $"{media.AiringStartDate:MMM yyyy}", true);
    }

    public static void AddCommonMediaFieldsBottom(DiscordEmbedBuilder embed, Media media)
    {
        embed.AddField("Status", media.Status.ToFriendlyString(), true);
        embed.AddField("Average Score", media.AverageScore != null ? $"{media.AverageScore}%" : "-", true);
        embed.AddField("Mean Score", media.MeanScore != null ? $"{media.MeanScore}%" : "-", true);
        if (media.Genres?.Length > 0)
        {
            embed.AddField("Genres", string.Join(", ", media.Genres));
        }

        if (media.DescriptionMd != null)
        {
            embed.AddField("Description",
                media.DescriptionMd.StripHtml().ToDiscordMarkup().TruncateAndCloseSpoiler(140));
        }
    }

    public static void AddCommonMediaEmbedProperties(DiscordEmbedBuilder embed, Media media)
    {
        embed.WithThumbnail(media.CoverImageExtraLarge);
        embed.WithImageUrl(media.BannerImage);
        embed.WithFooter("AniList", "https://i.imgur.com/zqa6OEk.png");
        embed.WithColor(2010108);
    }

    public static DiscordLinkButtonComponent GetSiteButton(Media media)
    {
        return new DiscordLinkButtonComponent(media.SiteUrl, "View all details on AniList");
    }
}