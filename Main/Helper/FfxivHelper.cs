using System.Numerics;
using Main.Classes;
using RestSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using xivapi_cs.ViewModels.CharacterProfile;

namespace Main.Helper;

public static class FfxivHelper
{
    public static async Task<MemoryStream> GetCharacterSheet(CharacterExtended character)
    {
        using var imgBase = await Image.LoadAsync("Resources/characterTemplateBase.png");

        AddCharacterPortrait(imgBase, character);
        await AddPortraitFrame(imgBase);
        await AddJobFrame(imgBase);
        await AddCharacterName(imgBase, character);

        return await ConvertToMemoryStream(imgBase);
    }

    private static void AddCharacterPortrait(Image img, CharacterBase character)
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
        img.Mutate(x => x.DrawImage(imgPortrait, new Point(16, 66), 1));
    }

    private static async Task AddPortraitFrame(Image img)
    {
        using var imgFrame = await Image.LoadAsync("Resources/characterTemplateFrame.png");
        img.Mutate(x => x.DrawImage(imgFrame, 1));
    }

    private static async Task AddJobFrame(Image img)
    {
        using var imgJob = await Image.LoadAsync("Resources/characterTemplateJob.png");
        img.Mutate(x => x.DrawImage(imgJob, 1));
    }

    private static Task AddCharacterName(Image img, CharacterExtended character)
    {
        var collection = new FontCollection();
        var family = collection.Add("Resources/Vollkorn-VariableFont_wght.ttf");
        var nameProperties = new FfxivNameProperties(character);

        var fontName = family.CreateFont(nameProperties.Name.Size, FontStyle.Regular);

        var optionsName = new TextOptions(fontName)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(nameProperties.Name.X, nameProperties.Name.Y)
        };

        img.Mutate(x => x.DrawText(optionsName, character.Name, Color.Black));

        if (nameProperties.Title == null)
        {
            // no title, return
            return Task.CompletedTask;
        }

        var fontTitle = family.CreateFont(nameProperties.Title.Size, FontStyle.Regular);

        var optionsTitle = new TextOptions(fontTitle)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(nameProperties.Title.X, nameProperties.Title.Y)
        };

        img.Mutate(x => x.DrawText(optionsTitle, character.Title.Name, Color.Black));

        return Task.CompletedTask;
    }

    private static async Task<MemoryStream> ConvertToMemoryStream(Image img)
    {
        var stream = new MemoryStream();
        await img.SaveAsync(stream, new WebpEncoder());
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}