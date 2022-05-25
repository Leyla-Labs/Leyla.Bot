using Common.Classes;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

internal sealed class Delete : SlashCommand
{
    private readonly DiscordMember _member;
    private readonly long _n;

    public Delete(InteractionContext ctx, DiscordMember member, long n) : base(ctx)
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
            // TODO make pretty
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("Quote not found."));
            return;
        }

        await DeleteFromDatabase(quote);

        var displayName = await Ctx.GetDisplayName(quote.MemberId);
        var embed = GetConfirmationEmbed(quote, displayName);
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    private async Task DeleteFromDatabase(Quote quote)
    {
        DbCtx.Entry(quote).State = EntityState.Deleted;
        await DbCtx.SaveChangesAsync();
    }

    #region Static methods

    private static DiscordEmbed GetConfirmationEmbed(Quote quote, string displayName)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Quote Deleted");
        embed.WithDescription(
            $"**\"{quote.Text}\"**{Environment.NewLine}- {displayName}, {quote.Date.Year}");
        embed.WithColor(DiscordColor.DarkRed);
        return embed.Build();
    }

    #endregion
}