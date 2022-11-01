using Common.Classes;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;
using xivapi_cs;
using xivapi_cs.Enums;
using xivapi_cs.ViewModels.CharacterSearch;

namespace Main.Commands.Ffxiv;

public sealed class Find : SlashCommand
{
    private readonly string _name;
    private readonly string? _server;

    public Find(InteractionContext ctx, string name, string? server) : base(ctx)
    {
        _name = name;
        _server = server;
    }

    public override async Task RunAsync()
    {
        var characterSearch = _server != null
            ? await new XivApiClient().CharacterSearch(_name, _server)
            : await new XivApiClient().CharacterSearch(_name);

        if (characterSearch == null || !characterSearch.Results.Any())
        {
            var errorMsg = $"{_name}{(_server != null ? $"({_server})" : string.Empty)} not found";
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed(errorMsg));
            return;
        }

        if (characterSearch.Results.Length > 1)
        {
            var characterSelect = GetCharacterSelect(characterSearch.Results);
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddComponents(characterSelect)
                .AsEphemeral());
            return;
        }

        await Ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var characterData = await new XivApiClient().CharacterProfileExtended(characterSearch.Results.First().Id,
            CharacterProfileOptions.FreeCompany | CharacterProfileOptions.MinionsMounts);

        if (characterData == null)
        {
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed("Could not get character data."));
            return;
        }

        var stream = await FfxivHelper.GetCharacterSheet(characterData.Character);
        // TODO proper filename
        await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddFile("test123.webp", stream, true));
    }

    #region Instance methods

    private DiscordSelectComponent GetCharacterSelect(IEnumerable<CharacterSearchResult> results)
    {
        var name = ModalHelper.GetModalName(Ctx.User.Id, "ffxivCharacterSheet");
        var options = results.Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(), x.HomeWorldDetails.HomeWorld.ToString()));
        return new DiscordSelectComponent(name, "Select character", options, minOptions: 1, maxOptions: 1);
    }

    #endregion
}