using Common.Classes;
using Common.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;

namespace Main.Commands.Quotes;

internal sealed class Show : SlashCommand
{
    private readonly DiscordMember _member;
    private readonly long _n;

    public Show(InteractionContext ctx, DiscordMember member, long n) : base(ctx)
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

        var quote = await QuoteHelper.GetQuoteAsync(Ctx.Guild.Id, _member.Id, (int) _n);

        if (quote == null)
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("Quote not found."));
            return;
        }

        var embed = QuoteHelper.GetQuoteEmbed(_member.DisplayName, quote);
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }
}