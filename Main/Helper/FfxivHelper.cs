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
    public static async Task<MemoryStream> GetCharacterSheet(CharacterProfileExtended profile)
    {
        using var imgBase = await Image.LoadAsync("Resources/characterTemplateBase.png");

        var character = profile.Character;
        var fontCollection = new FontCollection();

        AddCharacterPortrait(imgBase, character);
        await AddPortraitFrame(imgBase);

        await AddJobIcon(imgBase, character);
        await AddJobFrame(imgBase);

        await AddCharacterName(imgBase, character, fontCollection);
        await AddJobLevels(imgBase, character, fontCollection);

        // must be after AddJobLevels since OpenSans is loaded there
        var gc = await AddGrandCompany(imgBase, character, fontCollection);
        var fc = await AddFreeCompany(imgBase, profile, fontCollection);

        if (!gc && !fc)
        {
            await AddNewAdventurer(imgBase, fontCollection);
        }

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

    private static async Task AddJobIcon(Image img, CharacterExtended character)
    {
        if (character?.ActiveClassJob.Job.JobEnum == null)
        {
            return;
        }

        var filePath = $"Resources/Jobs/{character.ActiveClassJob.Job.JobEnum.ToString()!.ToLower()}.png";
        var imgJob = await Image.LoadAsync(filePath);

        imgJob.Mutate(x => x.Resize(68, 68));
        img.Mutate(x => x.DrawImage(imgJob, CoordinatesOther.JobIcon, 1));
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
        IReadOnlyFontCollection fontCollection)
    {
        if (character.GrandCompany == null)
        {
            return false;
        }

        var crest = await character.GrandCompany.GrandCompanyEnum.GetCrest();
        crest.Mutate(x => x.Resize(Values.DimensionsGcFcCrest, Values.DimensionsGcFcCrest, KnownResamplers.Lanczos3));

        if (character.FreeCompanyId == null)
        {
            // if player not in any free company, use the fc space to show the gc logo and name
            var gcName = character.GrandCompany.GrandCompanyEnum.GetAttribute<DisplayAttribute>()?.Name ??
                         throw new NullReferenceException(nameof(character.GrandCompany.GrandCompanyEnum));
            await PrintInTopValueArea(img, fontCollection, gcName, crest);
        }
        else
        {
            // if player is in a free company, print gc crest
            img.Mutate(x => x.DrawImage(crest, CoordinatesOther.GcBottom, 1));
        }

        return true;
    }

    private static async Task<bool> AddFreeCompany(Image img, CharacterProfileExtended profile,
        IReadOnlyFontCollection fontCollection)
    {
        if (profile.FreeCompany == null)
        {
            return false;
        }

        var crest = await GetFreeCompanyCrest(profile);
        crest.Mutate(x => x.Resize(Values.DimensionsGcFcCrest, Values.DimensionsGcFcCrest, KnownResamplers.Lanczos3));
        await PrintInTopValueArea(img, fontCollection, profile.FreeCompany.Name, crest);
        return true;
    }

    /// <summary>
    ///     Sends rest request to XIVAPI and returns FC crest in singular image
    /// </summary>
    /// <param name="profile">Free Company must not be null!</param>
    /// <returns>Free Company Crest</returns>
    private static Task<Image> GetFreeCompanyCrest(CharacterProfileBase profile)
    {
        var client = new RestClient();

        var list = profile.FreeCompany!.Crest
            .Select(x => new RestRequest(x, Method.GET)) // convert url to rest request
            .Select(x => client.DownloadData(x)) // download data from lodestone
            .Select(Image.Load<Rgba32>).ToList(); // add to list

        foreach (var layer in list.Skip(1))
        {
            list[0].Mutate(x => x.DrawImage(layer, 1));
        }

        return Task.FromResult(list[0] as Image);
    }

    private static Task AddNewAdventurer(Image img, IReadOnlyFontCollection fontCollection)
    {
        return PrintInTopValueArea(img, fontCollection, "New Adventurer");
    }

    private static Task PrintInTopValueArea(Image img, IReadOnlyFontCollection fontCollection, string text,
        Image? crest = null)
    {
        var family = fontCollection.Get("Open Sans");
        var font = family.CreateFont(Values.FontSizeGrandCompany, FontStyle.Regular);

        // if no crest, get coordinates without offset
        var coords = crest == null ? CoordinatesOther.TextTop : CoordinatesOther.FcOrGcTop;

        var textOptions = new TextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(coords.X, coords.Y - 2) // arbitrarily move text up 2px, looks better
        };

        // print text
        img.Mutate(x => x.DrawText(textOptions, text, Color.White));

        if (crest == null)
        {
            return Task.CompletedTask;
        }

        // get text width and calculate position of crest
        var textWidth = TextMeasurer.Measure(text, textOptions);
        coords.X -= (int) decimal.Divide((int) textWidth.Width, 2) +
                    Values.DimensionsGcFcCrest + Values.GcCrestPadding;
        coords.Y -= (int) decimal.Divide(Values.DimensionsGcFcCrest, 2);

        img.Mutate(x => x.DrawImage(crest, coords, 1));
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