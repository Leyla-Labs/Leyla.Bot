using Common.Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Spam.Helper;

namespace Spam.Handler;

internal sealed class RaidModeSelectedHandler : InteractionHandler
{
    private readonly string _raidMode;

    public RaidModeSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e, string raidMode) :
        base(sender, e)
    {
        _raidMode = raidMode;
    }

    public override async Task RunAsync()
    {
        var raidMode = bool.Parse(_raidMode);
        if (raidMode)
        {
            var embed = await RaidHelper.Instance.EnableRaidModeAndGetEmbedAsync(EventArgs.Guild);
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(embed));
        }
    }
}