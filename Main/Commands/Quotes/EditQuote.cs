using Common.Classes;
using Common.Extensions;
using Db;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Main.Helper;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public sealed class EditQuote : SlashCommand
{
    private readonly DiscordMember _member;
    private readonly long _n;

    public EditQuote(InteractionContext ctx, DiscordMember member, long n) : base(ctx)
    {
        _member = member;
        _n = n;
    }

    public override async Task RunAsync()
    {
        if (_n > int.MaxValue)
        {
            await Ctx.CreateResponseAsync(
                new DiscordInteractionResponseBuilder().AddErrorEmbed("That number is way too high!"));
            return;
        }

        var quote = await QuoteHelper.GetQuote(Ctx.Guild.Id, _member.Id, (int) _n);

        if (quote == null)
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("Quote not found."));
            return;
        }

        var displayName = await Ctx.GetDisplayName(quote.MemberId);

        // show modal
        var responseBuilder = GetModal(quote, displayName);
        await Ctx.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);

        // wait for user response to modal
        var interactivity = Ctx.Client.GetInteractivity();
        var userResponse = await interactivity.WaitForModalAsync($"modal-editquote-{Ctx.User.Id}", Ctx.User);
        if (userResponse.TimedOut)
        {
            return;
        }

        // get value from modal and edit in database
        var modalInteraction = userResponse.Result.Interaction;
        var text = userResponse.Result.Values["text"];
        await EditInDatabase(quote, text);

        // show confirmation embed
        var embed = GetConfirmationEmbed(quote, displayName);
        await modalInteraction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    #region Instance methods

    private DiscordInteractionResponseBuilder GetModal(Quote quote, string displayName)
    {
        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle($"Edit Quote: {displayName}")
            .WithCustomId($"modal-editquote-{Ctx.User.Id}")
            .AddComponents(new TextInputComponent("Quote", "text", max_length: 2000, min_length: 1,
                style: TextInputStyle.Paragraph, value: quote.Text));
        return response;
    }

    #endregion

    #region Static methods

    private static async Task EditInDatabase(Quote quote, string text)
    {
        await using var context = new DatabaseContext();
        quote.Text = text;
        context.Entry(quote).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    private static DiscordEmbed GetConfirmationEmbed(Quote quote, string displayName)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Quote Edited");
        embed.WithDescription(
            $"**\"{quote.Text}\"**{Environment.NewLine}- {displayName}, {quote.Date.Year}");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}