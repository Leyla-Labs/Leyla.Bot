using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Common.Extensions;
using Main.Classes.FfxivCharacterSheet;
using Main.Extensions;
using RestSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using xivapi_cs.ViewModels.CharacterProfile;
using Job = xivapi_cs.Enums.Job;

namespace Main.Helper;

public static class FfxivHelper
{
    public static async Task<MemoryStream> GetCharacterSheet(CharacterExtended character)
    {
        using var imgBase = await Image.LoadAsync("Resources/characterTemplateBase.png");

        var fontCollection = new FontCollection();

        AddCharacterPortrait(imgBase, character);
        await AddPortraitFrame(imgBase);
        await AddJobFrame(imgBase);
        await AddCharacterName(imgBase, character, fontCollection);
        await AddJobLevels(imgBase, character, fontCollection);

        // must be after AddJobLevels since OpenSans is loaded there
        await AddGrandCompany(imgBase, character, fontCollection);

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
        var imgFrame = await Image.LoadAsync("Resources/characterTemplateFrame.png");
        img.Mutate(x => x.DrawImage(imgFrame, 1));
    }

    private static async Task AddJobFrame(Image img)
    {
        var imgJob = await Image.LoadAsync("Resources/characterTemplateJob.png");
        img.Mutate(x => x.DrawImage(imgJob, 1));
    }

    private static Task AddCharacterName(Image img, CharacterExtended character, IFontCollection fontCollection)
    {
        var family = fontCollection.Add("Resources/Vollkorn-VariableFont_wght.ttf");
        var nameProperties = new NameProperties(character);

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

    private static Task AddJobLevels(Image img, CharacterExtended character, IFontCollection fontCollection)
    {
        var family = fontCollection.Add("Resources/OpenSans-VariableFont_wdth,wght.ttf");
        var font = family.CreateFont(28, FontStyle.Regular);

        foreach (var job in (Job[]) Enum.GetValues(typeof(Job)))
        {
            var jobLevel = character.ClassJobs.FirstOrDefault(x =>
                x.Job.JobEnum == job && x.Level > 0)?.Level;

            var levelString = jobLevel != null ? jobLevel.ToString() : "-";

            var options = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Origin = JobCoordinates.Get(job)
            };

            img.Mutate(x => x.DrawText(options, levelString, Color.White));
        }

        return Task.CompletedTask;
    }

    private static async Task<bool> AddGrandCompany(Image img, CharacterExtended character,
        IFontCollection fontCollection)
    {
        if (character.GrandCompany == null)
        {
            return false;
        }

        var crest = await character.GrandCompany.GrandCompanyEnum.GetCrest();

        crest.Mutate(x => x.Resize(Values.DimensionsGcCrest, Values.DimensionsGcCrest, KnownResamplers.Lanczos3));

        var coordinates = character.FreeCompanyId == null
            ? CoordinatesOther.GrandCompanyTop
            : CoordinatesOther.GrandCompanyBottom;

        if (character.FreeCompanyId == null)
        {
            // if player not in any free company, use the fc space to show the gc logo and name

            var gcName = character.GrandCompany.GrandCompanyEnum.GetAttribute<DisplayAttribute>()?.Name ??
                         throw new NullReferenceException(nameof(character.GrandCompany.GrandCompanyEnum));

            var family = fontCollection.Get("Open Sans");
            var font = family.CreateFont(Values.FontSizeGrandCompany, FontStyle.Regular);

            var options = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Origin = new Vector2(coordinates.X, coordinates.Y - 2) // arbitrarily move up 2px, looks better
            };

            // print gc name
            img.Mutate(x => x.DrawText(options, gcName, Color.White));

            // get gc name width and calculate position of gc crest
            var textWidth = TextMeasurer.Measure(gcName, options);
            coordinates.X -= (int) decimal.Divide((int) textWidth.Width, 2) +
                             Values.DimensionsGcCrest + Values.GcCrestPadding;
            coordinates.Y -= (int) decimal.Divide(Values.DimensionsGcCrest, 2);
        }

        img.Mutate(x => x.DrawImage(crest, coordinates, 1));

        return true;
    }

    private static async Task<MemoryStream> ConvertToMemoryStream(Image img)
    {
        var stream = new MemoryStream();
        await img.SaveAsync(stream, new WebpEncoder());
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}