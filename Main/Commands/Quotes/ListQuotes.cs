using System.Text;
using Db;
using Db.Models;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public static class ListQuotes
{
    public static async Task RunSlash(InteractionContext ctx, DiscordMember member)
    {
        var quotes = await GetQuotesForMember(ctx.Guild.Id, member.Id);

        var b = new StringBuilder();
        for (var i = 0; i < quotes.Count; i++)
        {
            b.Append($"**{i + 1}.** {quotes[i].Text}{Environment.NewLine}");
        }

        var quotesStr = quotes.Count > 0
            ? b.ToString()
            : "No quotes";

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Quotes for {member.DisplayName}");
        embed.WithDescription(quotesStr);
        embed.WithColor(DiscordColor.Blurple);

        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()).AsEphemeral());
    }

    private static async Task<List<Quote>> GetQuotesForMember(ulong guildId, ulong userId)
    {
        await using var context = new DatabaseContext();

        return await context.Quotes.Where(x =>
                x.Member.GuildId == guildId &&
                x.MemberId == userId)
            .ToListAsync();
    }
}