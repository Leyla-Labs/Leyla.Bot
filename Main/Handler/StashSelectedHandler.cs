using Common.Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Main.Handler;

public class StashSelectedHandler : InteractionHandler
{
    private readonly string _content;

    public StashSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e, string content) :
        base(sender, e)
    {
        _content = content;
    }

    public override async Task RunAsync()
    {
        var stashIds = string.Join(",", EventArgs.Values);
        var modalId = $"addToStash-{EventArgs.User.Id}-{stashIds}";

        var modal = GetModal(modalId);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
    }

    private DiscordInteractionResponseBuilder GetModal(string modalId)
    {
        var valueTextInput = new TextInputComponent("Value", "value", max_length: 140, min_length: 1,
            style: TextInputStyle.Paragraph, value: _content);

        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle("Add to Stash")
            .WithCustomId(modalId)
            .AddComponents(valueTextInput);
        return response;
    }
}