using Common.Classes;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.CommandLogs;

internal sealed class Recent : SlashCommand
{
    private readonly int _n;

    public Recent(InteractionContext ctx, long n) : base(ctx)
    {
        _n = n <= 100 ? Convert.ToInt32(n) : 100;
    }

    public override async Task RunAsync()
    {
        var recent = await CommandLogHelper.Instance.GetRecentAsync(Ctx.Guild.Id, _n);
        var embed = await CommandLogHelper.GetEmbedAsync(Ctx.Guild, recent, true);

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }
}