using System.Text.RegularExpressions;
using Anilist4Net.Enums;

namespace Main.Extensions;

internal static class AniListExtensions
{
    public static string ToDiscordMarkup(this string input)
    {
        input = Regex.Replace(input, "__", "**");
        input = Regex.Replace(input, "~!|!~", "||");
        return input;
    }

    public static string TruncateAndCloseSpoiler(this string input, int maxLength)
    {
        return input.Length <= maxLength
            ? input
            : Regex.Matches(input[..maxLength], "\\|\\|").Count % 2 != 0
                ? $"{input[..maxLength]}||…"
                : $"{input[..maxLength]}…";
    }

    public static string ToFriendlyString(this MediaStatuses status)
    {
        return status switch
        {
            MediaStatuses.NOT_YET_RELEASED => "Not yet released",
            MediaStatuses.RELEASING => "Releasing",
            MediaStatuses.FINISHED => "Finished",
            MediaStatuses.HIATUS => "On hiatus",
            MediaStatuses.CANCELLED => "Cancelled",
            _ => string.Empty
        };
    }

    public static string ToFriendlyString(this MediaFormats format)
    {
        return format switch
        {
            MediaFormats.TV => "TV",
            MediaFormats.MOVIE => "Movie",
            MediaFormats.MUSIC => "Music",
            MediaFormats.ONA => "ONA",
            MediaFormats.OVA => "OVA",
            MediaFormats.MANGA => "Manga",
            MediaFormats.ONE_SHOT => "One Shot",
            MediaFormats.SPECIAL => "Special",
            MediaFormats.NOVEL => "Novel",
            MediaFormats.TV_SHORT => "TV Short",
            _ => string.Empty
        };
    }
}