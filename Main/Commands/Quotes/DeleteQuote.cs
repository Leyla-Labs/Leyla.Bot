using Common.Extensions;
using Db;
using Db.Models;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public static class DeleteQuote
{
    public static async Task RunSlash(InteractionContext ctx, DiscordMember member, long n)
    {
        if (n > int.MaxValue)
        {
            // TODO make pretty
            await ctx.CreateResponseAsync(
                new DiscordInteractionResponseBuilder().WithContent("Number bigger than MaxInt."));
            return;
        }
        
        var quote = await QuoteHelper.GetQuote(ctx.Guild.Id, member.Id, (int) n);
        
        if (quote == null)
        {
            // TODO make pretty
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Quote not found."));
            return;
        }

        await DeleteFromDatabase(quote);

        var displayName = await ctx.GetDisplayName(quote.MemberId);
        var embed = GetConfirmationEmbed(quote, displayName);
        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    private static async Task DeleteFromDatabase(Quote quote)
    {
        await using var context = new DatabaseContext();
        context.Entry(quote).State = EntityState.Deleted;
        await context.SaveChangesAsync();
    }

    private static DiscordEmbed GetConfirmationEmbed(Quote quote, string displayName)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Quote Deleted");
        embed.WithDescription(
            $"**\"{quote.Text}\"**{Environment.NewLine}- {displayName}, {quote.Date.Year}");
        embed.WithColor(DiscordColor.DarkRed);
        return embed.Build();
    }
}