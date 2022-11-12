using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Common.Extensions;
using Main.Classes.FfxivCharacterSheet;
using Main.Extensions;
using RestSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using xivapi_cs.ViewModels.CharacterProfile;
using Attribute = xivapi_cs.Enums.Attribute;
using Job = xivapi_cs.Enums.Job;

namespace Main.Helper;

public class CharacterSheetHelper
{
    private readonly FontCollection _fontCollection = new();
    private readonly CharacterProfileExtended _profile;

    private CharacterSheetHelper(CharacterProfileExtended profile)
    {
        _profile = profile;
    }

    private CharacterExtended Character => _profile.Character;
    private Image Image { get; set; } = null!;

    public static async Task<CharacterSheetHelper> Create(CharacterProfileExtended profile)
    {
        var helper = new CharacterSheetHelper(profile);
        await helper.Initialize();
        return helper;
    }

    private async Task Initialize()
    {
        Image = await Image.LoadAsync("Resources/characterTemplateBase.png");
        LoadFonts();
    }

    private void LoadFonts()
    {
        _fontCollection.Add("Resources/OpenSans-VariableFont_wdth,wght.ttf");
        _fontCollection.Add("Resources/Vollkorn-VariableFont_wght.ttf");
        _fontCollection.Add("Resources/Antonio-ExtraLight.ttf");
    }

    public async Task<MemoryStream> GetCharacterSheet()
    {
        AddCharacterPortrait();
        await AddPortraitFrame();

        await AddJobIcon();
        await AddJobFrame();
        await AddActiveJobLevel();

        await AddCharacterName();
        await AddJobLevels();

        // must be after AddJobLevels since OpenSans is loaded there
        var gc = await AddGrandCompany();
        var fc = await AddFreeCompany();
        await AddAttributes();

        if (!gc && !fc)
        {
            await AddNewAdventurer();
        }

        return await ConvertToMemoryStream();
    }

    private void AddCharacterPortrait()
    {
        // get portrait as byte[]
        var client = new RestClient();
        var request = new RestRequest(Character.Portrait, Method.GET);
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
        Image.Mutate(x => x.DrawImage(imgPortrait, new Point(16, 66), 1));
    }

    private async Task AddPortraitFrame()
    {
        var imgFrame = await Image.LoadAsync("Resources/characterTemplateFrame.png");
        Image.Mutate(x => x.DrawImage(imgFrame, 1));
    }

    private async Task AddJobFrame()
    {
        var imgJob = await Image.LoadAsync("Resources/characterTemplateJob.png");
        Image.Mutate(x => x.DrawImage(imgJob, 1));
    }

    private async Task AddJobIcon()
    {
        if (Character.ActiveClassJob.Job.JobEnum == null)
        {
            return;
        }

        var filePath = $"Resources/Jobs/{Character.ActiveClassJob.Job.JobEnum.ToString()!.ToLower()}.png";
        var imgJob = await Image.LoadAsync(filePath);

        imgJob.Mutate(x => x.Resize(68, 68));
        Image.Mutate(x => x.DrawImage(imgJob, CoordinatesOther.JobIcon, 1));
    }

    private Task AddActiveJobLevel()
    {
        var circle = new EllipsePolygon(CoordinatesOther.ActiveJobLevelBackground, Values.ActiveJobLevelRadius);
        Image.Mutate(x => x.Draw(new Color(Values.ActiveJobLevelBackground), Values.ActiveJobLevelThickness, circle));

        var family = _fontCollection.Get("Antonio ExtraLight");
        var font = family.CreateFont(Values.ActiveJobLevelFontSize, FontStyle.Regular);
        var text = $"Lv. {Character.ActiveClassJob.Level}";

        var textOptions = new TextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = CoordinatesOther.ActiveJobLevelText
        };

        Image.Mutate(x => x.DrawText(textOptions, text, Color.White));
        return Task.CompletedTask;
    }

    private Task AddCharacterName()
    {
        var family = _fontCollection.Get("Vollkorn");
        var nameProperties = new NameProperties(Character);

        var fontName = family.CreateFont(nameProperties.Name.Size, FontStyle.Regular);

        var optionsName = new TextOptions(fontName)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(nameProperties.Name.X, nameProperties.Name.Y)
        };

        Image.Mutate(x => x.DrawText(optionsName, Character.Name, Color.Black));

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

        Image.Mutate(x => x.DrawText(optionsTitle, Character.Title.Name, Color.Black));

        return Task.CompletedTask;
    }

    private Task AddJobLevels()
    {
        var family = _fontCollection.Get("Open Sans");
        var font = family.CreateFont(28, FontStyle.Regular);

        foreach (var job in (Job[]) Enum.GetValues(typeof(Job)))
        {
            var jobLevel = Character.ClassJobs.FirstOrDefault(x =>
                x.Job.JobEnum == job && x.Level > 0)?.Level;

            var levelString = jobLevel != null ? jobLevel.ToString() : "-";

            var options = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Origin = JobCoordinates.Get(job)
            };

            Image.Mutate(x => x.DrawText(options, levelString, Color.White));
        }

        return Task.CompletedTask;
    }

    private async Task<bool> AddGrandCompany()
    {
        if (Character.GrandCompany == null)
        {
            return false;
        }

        var crest = await Character.GrandCompany.GrandCompanyEnum.GetCrest();
        crest.Mutate(x => x.Resize(Values.DimensionsGcFcCrest, Values.DimensionsGcFcCrest, KnownResamplers.Lanczos3));

        if (Character.FreeCompanyId == null)
        {
            // if player not in any free company, use the fc space to show the gc logo and name
            var gcName = Character.GrandCompany.GrandCompanyEnum.GetAttribute<DisplayAttribute>()?.Name ??
                         throw new NullReferenceException(nameof(Character.GrandCompany.GrandCompanyEnum));
            await PrintInTopValueArea(gcName, crest);
        }
        else
        {
            // if player is in a free company, print gc crest
            Image.Mutate(x => x.DrawImage(crest, CoordinatesOther.GcBottom, 1));
        }

        return true;
    }

    private async Task<bool> AddFreeCompany()
    {
        if (_profile.FreeCompany == null)
        {
            return false;
        }

        var crest = await GetFreeCompanyCrest(_profile);
        crest.Mutate(x => x.Resize(Values.DimensionsGcFcCrest, Values.DimensionsGcFcCrest, KnownResamplers.Lanczos3));
        await PrintInTopValueArea(_profile.FreeCompany.Name, crest);
        return true;
    }

    private Task AddAttributes()
    {
        var job = Character.ActiveClassJob.Job.JobEnum;

        if (job == null)
        {
            throw new ArgumentNullException(nameof(Character), "JobEnum is null");
        }

        var attributes = job.Value.GetDisplayAttributes();

        var family = _fontCollection.Get("Open Sans");
        var font = family.CreateFont(Values.FontSizeAttributes, FontStyle.Regular);

        PrintAttributes(font, attributes.Take(2), CoordinatesOther.AttributesPrimary, true);
        PrintAttributes(font, attributes.Skip(2), CoordinatesOther.AttributesSecondary, false);

        return Task.CompletedTask;
    }

    private void PrintAttributes(Font font, IEnumerable<Attribute> attributes, Point origin, bool primary)
    {
        var options = new TextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = origin
        };

        var result = attributes.Select(x =>
            $"{GetAttributeName(x, primary)}:{Values.AttributeGapSmall}{GetAttributeValue(Character, x)}");

        var text = string.Join(Values.AttributeGapBig, result);

        Image.Mutate(x => x.DrawText(options, text, Color.White));
    }

    private Task AddNewAdventurer()
    {
        return PrintInTopValueArea("New Adventurer");
    }

    private Task PrintInTopValueArea(string text, Image? crest = null)
    {
        var family = _fontCollection.Get("Open Sans");
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
        Image.Mutate(x => x.DrawText(textOptions, text, Color.White));

        if (crest == null)
        {
            return Task.CompletedTask;
        }

        // get text width and calculate position of crest
        var textWidth = TextMeasurer.Measure(text, textOptions);
        coords.X -= (int) decimal.Divide((int) textWidth.Width, 2) +
                    Values.DimensionsGcFcCrest + Values.GcCrestPadding;
        coords.Y -= (int) decimal.Divide(Values.DimensionsGcFcCrest, 2);

        Image.Mutate(x => x.DrawImage(crest, coords, 1));
        return Task.CompletedTask;
    }

    private async Task<MemoryStream> ConvertToMemoryStream()
    {
        var stream = new MemoryStream();
        await Image.SaveAsync(stream, new WebpEncoder());
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
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

    private static string GetAttributeName(Attribute a, bool fullName)
    {
        var dAttr = a.GetAttribute<DisplayAttribute>() ?? throw new NullReferenceException("Attribute is null.");
        return (fullName ? dAttr.Name : dAttr.ShortName ?? dAttr.Name) ??
               throw new NullReferenceException("Name attribute is missing");
    }

    private static int GetAttributeValue(CharacterExtended character, Attribute attr)
    {
        return character.GearSet.Attributes.Where(x =>
                x.Attribute == attr)
            .Select(x => x.Value)
            .First();
    }
}