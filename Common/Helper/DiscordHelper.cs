namespace Common.Helper;

public static class DiscordHelper
{
    public static IEnumerable<string> ImageFormats()
    {
        return new[] {"jpg", "jpeg", "png", "webp", "gif"};
    }
}