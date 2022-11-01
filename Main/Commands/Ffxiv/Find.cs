using Common.Classes;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using xivapi_cs;
using xivapi_cs.Enums;

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
            var errorMsg = $"{_name}{(_server != null ? $"({_server})" : string.Empty)} not found";
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed(errorMsg));
            return;
        }
        
        // TODO show select menu if more than one result

        var characterData = await new XivApiClient().CharacterProfileExtended(characterSearch.Results.First().Id,
            CharacterProfileOptions.FreeCompany | CharacterProfileOptions.MinionsMounts);

        if (characterData == null)
        {
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed("Could not get character data."));
            return;
        }

        var stream = await Helper.FfxivHelper.GetCharacterSheet(characterData.Character);
        // TODO proper filename
        await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddFile("test123.webp", stream, true));
    }
}