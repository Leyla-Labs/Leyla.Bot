using SixLabors.ImageSharp;
using xivapi_cs.Enums;

namespace Main.Extensions;

internal static class XivapiExtensions
{
    public static async Task<Image> GetCrest(this GrandCompany gc)
    {
        var fileName = $"Resources/chat_messengericon_town0{(int) gc}.png";
        return await Image.LoadAsync(fileName);
    }
}