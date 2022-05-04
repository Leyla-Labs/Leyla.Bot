using Common.Extensions;
using Db;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public static class RandomQuote
{
    public static async Task RunSlash(InteractionContext ctx)
    {
        var quote = await GetRandomQuote(ctx.Guild.Id);
        if (quote == null)
        {
            // TODO make pretty
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("No quotes found."));
        }

        var member = await ctx.GetMember(quote!.MemberId);
        if (member == null)
        {
            // TODO make pretty
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Member not found."));
        }

        var embed = GetQuoteEmbed(member!.DisplayName, quote);
        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }

    private static async Task<Quote?> GetRandomQuote(ulong guildId)
    {
        await using var context = new DatabaseContext();
        return await context.Quotes.Where(x =>
            x.Member.GuildId == guildId)
        .OrderBy(x => Guid.NewGuid())
        .FirstOrDefaultAsync();
    }
    
    private static DiscordEmbed GetQuoteEmbed(string displayName, Quote quote)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithDescription(
            $"**\"{quote.Text}\"**{Environment.NewLine}- {displayName}, {quote.Date.Year}");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }
}