using Common.Classes;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Spam.Helper;

namespace Spam.Handler;

public class RaidModeSelectedHandler : InteractionHandler
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
            await RaidHelper.Instance.EnableRaidMode(EventArgs.Guild, EventArgs.Channel);
        }
    }
}