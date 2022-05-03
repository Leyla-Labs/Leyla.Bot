using Common.Extensions;
using Db;
using Db.Models;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public static class SearchQuotes
{
    public static async Task RunSlash(InteractionContext ctx, string searchQuery)
    {
        const int i = 4;
        if (searchQuery.Length < i)
        {
            // TODO make pretty
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Search query must be at least {i} characters.").AsEphemeral());
            return;
        }

        var guildQuotes = await GetQuotesForGuild(ctx.Guild.Id);
        if (guildQuotes.Count == 0)
        {
            // TODO make pretty
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Guild has no quotes.")
                .AsEphemeral());
            return;
        }

        var filteredQuotes = guildQuotes.Where(x => x.Text.Contains(searchQuery)).ToList();
        if (filteredQuotes.Count == 0)
        {
            // TODO make pretty
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("No search results.")
                .AsEphemeral());
            return;
        }

        var embed = await GetSearchEmbed(ctx, guildQuotes, filteredQuotes);
        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    private static async Task<List<Quote>> GetQuotesForGuild(ulong guildId)
    {
        await using var context = new DatabaseContext();
        return await context.Quotes.Where(x =>
                x.Member.GuildId == guildId)
            .ToListAsync();
    }

    private static async Task<DiscordEmbed> GetSearchEmbed(BaseContext ctx, ICollection<Quote> guildQuotes,
        List<Quote> filteredQuotes)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"{filteredQuotes.Count} {(filteredQuotes.Count == 1 ? "Quote" : "Quotes")} Found");

        var quoteStrings = new List<string>();
        foreach (var quote in filteredQuotes)
        {
            var member = await ctx.GetMember(quote.MemberId);
            var index = guildQuotes.Select((q, i) => new {quote = q, index = i}).First(x =>
                x.quote.MemberId == quote.MemberId && x.quote.Text.Equals(quote.Text)).index;
            quoteStrings.Add($"**\"{quote.Text}\"**{Environment.NewLine}- {member?.DisplayName} • #{index}");
        }

        var desc = string.Join($"{Environment.NewLine}{Environment.NewLine}", quoteStrings);
        desc += $"{Environment.NewLine}{Environment.NewLine}" +
                "You can request any of these quotes using `/quote show username index`, eg. `/quote show Leyla 7`";

        embed.WithDescription(desc);
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }
}