using System.Text;
using Db.Classes;
using Db.Models;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public sealed class ListQuotes : SlashCommand
{
    private readonly DiscordMember _member;

    public ListQuotes(InteractionContext ctx, DiscordMember member) : base(ctx)
    {
        _member = member;
    }

    public override async Task RunAsync()
    {
        var quotes = await GetQuotesForMember(Ctx.Guild.Id, _member.Id);

        var b = new StringBuilder();
        for (var i = 0; i < quotes.Count; i++)
        {
            b.Append($"**{i + 1}.** {quotes[i].Text}{Environment.NewLine}");
        }

        var quotesStr = quotes.Count > 0
            ? b.ToString()
            : "No quotes";

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Quotes for {_member.DisplayName}");
        embed.WithDescription(quotesStr);
        embed.WithColor(DiscordColor.Blurple);

        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()).AsEphemeral());
    }

    #region Instance methods

    private async Task<List<Quote>> GetQuotesForMember(ulong guildId, ulong userId)
    {
        return await DbCtx.Quotes.Where(x =>
                x.Member.GuildId == guildId &&
                x.MemberId == userId)
            .ToListAsync();
    }

    #endregion
}