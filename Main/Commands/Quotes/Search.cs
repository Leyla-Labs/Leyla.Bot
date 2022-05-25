using Common.Classes;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

internal sealed class Search : SlashCommand
{
    private readonly string _query;

    public Search(InteractionContext ctx, string query) : base(ctx)
    {
        _query = query;
    }

    public override async Task RunAsync()
    {
        const int i = 4;
        if (_query.Length < i)
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .AddErrorEmbed($"Search query must be at least {i} characters.").AsEphemeral());
            return;
        }

        var guildQuotes = await GetQuotesForGuild(Ctx.Guild.Id);
        if (guildQuotes.Count == 0)
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("Guild has no quotes.")
                .AsEphemeral());
            return;
        }

        var filteredQuotes = guildQuotes.Where(x => x.Text.Contains(_query)).ToList();
        if (filteredQuotes.Count == 0)
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("No search results.")
                .AsEphemeral());
            return;
        }

        var embed = await GetSearchEmbed(guildQuotes, filteredQuotes);
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    #region Instance methods

    private async Task<DiscordEmbed> GetSearchEmbed(ICollection<Quote> guildQuotes,
        List<Quote> filteredQuotes)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"{filteredQuotes.Count} {(filteredQuotes.Count == 1 ? "Quote" : "Quotes")} Found");

        var quoteStrings = new List<string>();
        foreach (var quote in filteredQuotes)
        {
            var member = await Ctx.GetMember(quote.UserId);
            var index = guildQuotes.Select((q, i) => new {quote = q, index = i}).First(x =>
                x.quote.UserId == quote.UserId && x.quote.Text.Equals(quote.Text)).index;
            quoteStrings.Add($"**\"{quote.Text}\"**{Environment.NewLine}- {member?.DisplayName} • #{index}");
        }

        var desc = string.Join($"{Environment.NewLine}{Environment.NewLine}", quoteStrings);
        desc += $"{Environment.NewLine}{Environment.NewLine}" +
                "You can request any of these quotes using `/quote show username index`, eg. `/quote show Leyla 7`";

        embed.WithDescription(desc);
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    private async Task<List<Quote>> GetQuotesForGuild(ulong guildId)
    {
        return await DbCtx.Quotes.Where(x =>
                x.GuildId == guildId)
            .ToListAsync();
    }

    #endregion
}