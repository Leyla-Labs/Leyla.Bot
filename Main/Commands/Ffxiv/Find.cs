using Common.Classes;
using Common.Helper;
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
        var profile =
            await FfxivHelper.SearchAndGetCharacterData(Ctx, _name, _server, "ffxivCharacterSheet", false,
                async x => await new XivApiClient().CharacterProfileExtended(x,
                    CharacterProfileOptions.FreeCompany | CharacterProfileOptions.MinionsMounts));

        if (profile == null)
        {
            return;
        }

        await FfxivHelper.RespondToSlashWithSheet(Ctx, profile);
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