using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;

namespace Main.Commands.Quotes;

public static class ShowQuote
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

        var embed = QuoteHelper.GetQuoteEmbed(member.DisplayName, quote);
        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }
}