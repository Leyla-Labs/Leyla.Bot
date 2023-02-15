using Common.Classes;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Main.Helper;
using xivapi_cs;
using xivapi_cs.Enums;

namespace Main.Handler;

public class FfxivCharacterClaimSelectedHandler : InteractionHandler
{
    public FfxivCharacterClaimSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e) :
        base(sender, e)
    {
    }

    public override async Task RunAsync()
    {
        await EventArgs.Interaction.DeferAsync(true);

        var id = Convert.ToInt32(EventArgs.Values.First());

        var characterData = await new XivApiClient().GetCharacterProfileAsync(id, CharacterProfileOptions.None);

        if (characterData == null)
        {
            await EventArgs.Interaction.EditOriginalResponseAsync(
                new DiscordWebhookBuilder().AddErrorEmbed("Could not get character data."));
            return;
        }

        var name = characterData.Character.Name;
        var (status, code) = await FfxivHelper.CreateClaimIfNotExistAsync(EventArgs.User.Id, characterData.Character.Id,
            characterData.Character.Bio);
        var embed = FfxivHelper.CreateCharacterClaimStatusEmbed(status, name, code);

        await EventArgs.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }
}