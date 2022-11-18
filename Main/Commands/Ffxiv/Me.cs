using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using Common.Helper;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;
using Microsoft.EntityFrameworkCore;
using xivapi_cs;
using xivapi_cs.Enums;
using xivapi_cs.ViewModels.CharacterProfile;

namespace Main.Commands.Ffxiv;

public class Me : SlashCommand
{
    public Me(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        await Ctx.DeferAsync();

        var claims = await GetCharacterClaims();

        if (!claims.Any())
        {
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed("No claimed characters.",
                "You have not yet claimed any characters as your own. " +
                "You can do so using `/ffxiv character claim`."));
            return;
        }

        var confirmedClaims = claims.Where(x => x.Confirmed).ToArray();

        if (!confirmedClaims.Any())
        {
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed("No confirmed claims.",
                "You have not yet finished claiming any characters ." +
                "Please make sure to follow the instructions when running `/ffxiv character claim.`"));
            return;
        }

        if (confirmedClaims.Length > 1)
        {
            var characterSelect = await GetCharacterSelect(claims);
            await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddComponents(characterSelect));
            return;
        }

        var profileExtended = await new XivApiClient().CharacterProfileExtended(confirmedClaims.First().Id,
            CharacterProfileOptions.FreeCompany | CharacterProfileOptions.MinionsMounts);

        if (profileExtended == null)
        {
            await Ctx.EditResponseAsync(
                new DiscordWebhookBuilder().AddErrorEmbed(
                    $"Character not found (Lodestone ID {confirmedClaims.First().Id})"));
            return;
        }

        await FfxivHelper.RespondToSlashWithSheet(Ctx, profileExtended);
    }

    private async Task<IReadOnlyCollection<CharacterClaim>> GetCharacterClaims()
    {
        await using var context = new DatabaseContext();

        return await context.CharacterClaims.Where(x =>
                x.UserId == Ctx.User.Id && x.Confirmed)
            .ToArrayAsync();
    }

    private async Task<DiscordSelectComponent> GetCharacterSelect(IEnumerable<CharacterClaim> claims)
    {
        var list = new List<Character>();

        foreach (var claim in claims)
        {
            var character = await new XivApiClient().CharacterProfile(claim.CharacterId, CharacterProfileOptions.None);

            if (character != null)
            {
                list.Add(character.Character);
            }
        }

        var name = ModalHelper.GetModalName(Ctx.User.Id, "ffxivCharacterSheet");
        var options = list.Take(25).Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(), x.HomeWorld.HomeWorld.ToString()));
        var suffix = list.Count > 25
            ? $" (Showing 25/{list.Count} results)./"
            : $" ({list.Count} results).";
        return new DiscordSelectComponent(name, $"Select character {suffix}", options, minOptions: 1, maxOptions: 1);
    }
}