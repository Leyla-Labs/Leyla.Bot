using Common.Classes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Spam.Helper;

namespace Spam.Commands.Raid;

public class On : SlashCommand
{
    public On(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var embed = await RaidHelper.Instance.EnableRaidModeAndGetEmbed(Ctx.Guild);
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }
}