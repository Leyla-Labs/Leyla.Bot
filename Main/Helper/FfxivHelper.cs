using RestSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using xivapi_cs.ViewModels.CharacterProfile;

namespace Main.Helper;

public static class FfxivHelper
{
    public static async Task<MemoryStream> GetCharacterSheet(CharacterBase character)
    {
        using var imgBase = await Image.LoadAsync("Resources/characterTemplateBase.png");

        AddCharacterPortrait(imgBase, character);
        await AddPortraitFrame(imgBase);
        await AddJobFrame(imgBase);
        
        return await ConvertToMemoryStream(imgBase);
    }

    private static void AddCharacterPortrait(Image imgBase, CharacterBase character)
    {
        // get portrait as byte[]
        var client = new RestClient();
        var request = new RestRequest(character.Portrait, Method.GET);
        var restResponse = client.DownloadData(request);

        // load image and resize
        var imgPortrait = Image.Load<Rgba32>(restResponse);
        imgPortrait.Mutate(x => x.Resize(592, 808, KnownResamplers.Lanczos3));
        
        // crop image
        var coordX = 464;
        var coordY = 808;
        var rect = new Rectangle(imgPortrait.Width / 2 - coordX / 2,
            imgPortrait.Height / 2 - coordY / 2,
            coordX,
            coordY);
        imgPortrait.Mutate(x => x.Crop(rect));
        
        // draw cropped portrait on top of base image
        imgBase.Mutate(x => x.DrawImage(imgPortrait, new Point(16, 66), 1));
    }

    private static async Task AddPortraitFrame(Image imgBase)
    {
        using var imgFrame = await Image.LoadAsync("Resources/characterTemplateFrame.png");
        imgBase.Mutate(x => x.DrawImage(imgFrame, 1));
    }

    private static async Task AddJobFrame(Image imgBase)
    {
        using var imgJob = await Image.LoadAsync("Resources/characterTemplateJob.png");
        imgBase.Mutate(x => x.DrawImage(imgJob, 1));
    }

    private static async Task<MemoryStream> ConvertToMemoryStream(Image imgBase)
    {
        var stream = new MemoryStream();
        await imgBase.SaveAsync(stream, new WebpEncoder());
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}