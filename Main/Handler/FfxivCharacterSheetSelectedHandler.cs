using Common.Classes;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Main.Helper;
using xivapi_cs;
using xivapi_cs.Enums;

namespace Main.Handler;

public class FfxivCharacterSheetSelectedHandler : InteractionHandler
{
    public FfxivCharacterSheetSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs eventArgs) :
        base(sender, eventArgs)
    {
    }

    public override async Task RunAsync()
    {
        // TODO this results in a new msg replying to the ephemeral one, meaning it shows as a reply to an invalid msg.
        // Is there a better way to do this?
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var id = Convert.ToInt32(EventArgs.Values.First());

        var characterData = await new XivApiClient().CharacterProfileExtended(id,
            CharacterProfileOptions.FreeCompany | CharacterProfileOptions.MinionsMounts);

        if (characterData == null)
        {
            await EventArgs.Interaction.EditOriginalResponseAsync(
                new DiscordWebhookBuilder().AddErrorEmbed("Could not get character data."));
            return;
        }

        var stream = await FfxivHelper.GetCharacterSheet(characterData.Character);
        // TODO proper filename
        await EventArgs.Interaction.EditOriginalResponseAsync(
            new DiscordWebhookBuilder().AddFile("test123.webp", stream, true));
    }
}