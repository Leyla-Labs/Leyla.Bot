using Common.Extensions;
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

        var embed = QuoteHelper.GetQuoteEmbed(member.DisplayName, quote);
        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }
}