using Common.Classes;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Main.Handler;

internal sealed class AddToStashSelectedHandler : InteractionHandler
{
    private readonly string _content;

    public AddToStashSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e, string content) :
        base(sender, e)
    {
        _content = content;
    }

    public override async Task RunAsync()
    {
        var modal = GetModal();
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
    }

    private DiscordInteractionResponseBuilder GetModal()
    {
        var stashIds = string.Join(",", EventArgs.Values);
        var modalId = ModalHelper.GetModalName(EventArgs.User.Id, "addToStash", new[] {stashIds});

        var valueTextInput = new TextInputComponent("Value", "value", max_length: 140, min_length: 1,
            style: TextInputStyle.Paragraph, value: _content);

        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle("Add to Stash")
            .WithCustomId(modalId)
            .AddComponents(valueTextInput);
        return response;
    }
}