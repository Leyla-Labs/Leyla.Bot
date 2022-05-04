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

public static class EditQuote
{
    public static async Task RunSlash(InteractionContext ctx, DiscordMember member, long n)
    {
        if (n > int.MaxValue)
        {
            await ctx.CreateResponseAsync(
                new DiscordInteractionResponseBuilder().AddErrorEmbed("That number is way too high!"));
            return;
        }

        var quote = await QuoteHelper.GetQuote(ctx.Guild.Id, member.Id, (int) n);

        if (quote == null)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("Quote not found."));
            return;
        }

        var displayName = await ctx.GetDisplayName(quote.MemberId);

        // show modal
        var responseBuilder = GetModal(ctx, quote, displayName);
        await ctx.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);

        // wait for user response to modal
        var interactivity = ctx.Client.GetInteractivity();
        var userResponse = await interactivity.WaitForModalAsync($"modal-editquote-{ctx.User.Id}", ctx.User);
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

    private static DiscordInteractionResponseBuilder GetModal(BaseContext ctx, Quote quote, string displayName)
    {
        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle($"Edit Quote: {displayName}")
            .WithCustomId($"modal-editquote-{ctx.User.Id}")
            .AddComponents(new TextInputComponent("Quote", "text", max_length: 2000, min_length: 1,
                style: TextInputStyle.Paragraph, value: quote.Text));
        return response;
    }

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
}