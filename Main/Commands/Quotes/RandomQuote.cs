using Common.Classes;
using Common.Extensions;
using Db;
using Db.Models;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public sealed class RandomQuote : SlashCommand
{
    public RandomQuote(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var quote = await GetRandomQuote(Ctx.Guild.Id);
        if (quote == null)
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("No quotes found."));
        }

        var member = await Ctx.GetMember(quote!.MemberId);
        if (member == null)
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("Member not found."));
        }

        var embed = GetQuoteEmbed(member!.DisplayName, quote);
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }

    #region Static methods

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

    #endregion
}