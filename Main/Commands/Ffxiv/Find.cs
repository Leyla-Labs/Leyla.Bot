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
        await Ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var characterSearch = _server != null
            ? await new XivApiClient().CharacterSearch(_name, _server)
            : await new XivApiClient().CharacterSearch(_name);

        if (characterSearch == null || !characterSearch.Results.Any())
        {
            var errorMsg = $"{_name}{(_server != null ? $" ({_server})" : string.Empty)} not found.";
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed(errorMsg));
            return;
        }

        if (characterSearch.Results.Length > 1)
        {
            var characterSelect = GetCharacterSelect(characterSearch);
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddComponents(characterSelect));
            return;
        }

        var characterData = await new XivApiClient().CharacterProfileExtended(characterSearch.Results.First().Id,
            CharacterProfileOptions.FreeCompany | CharacterProfileOptions.MinionsMounts);

        if (characterData == null)
        {
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed("Could not get character data."));
            return;
        }

        var helper = await CharacterSheetHelper.Create(characterData);
        var stream = await helper.GetCharacterSheet();
        // TODO proper filename
        await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddFile("test123.webp", stream, true));
    }

    #region Instance methods

    private DiscordSelectComponent GetCharacterSelect(CharacterSearch result)
    {
        var name = ModalHelper.GetModalName(Ctx.User.Id, "ffxivCharacterSheet");
        var options = result.Results.Take(25).Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(), x.HomeWorldDetails.HomeWorld.ToString()));
        var t = result.Pagination.ResultsTotal;
        var suffix = t > 25
            ? $" (Showing 25/{t} results)./"
            : $" ({t} results).";
        return new DiscordSelectComponent(name, $"Select character {suffix}", options, minOptions: 1, maxOptions: 1);
    }

    #endregion
}