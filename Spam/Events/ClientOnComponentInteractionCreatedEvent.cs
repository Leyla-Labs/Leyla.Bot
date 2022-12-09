using Common.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Spam.Handler;

namespace Spam.Events;

internal abstract class ClientOnComponentInteractionCreatedEvent : IEventHandler<ComponentInteractionCreateEventArgs>
{
    public static async Task HandleEventAsync(DiscordClient sender,
        ComponentInteractionCreateEventArgs e)
    {
        // info consists of userId, name, and any further information after that
        var info = e.Id.Split("_");
        var userId = Convert.ToUInt64(info[0]);

        if (userId != 1 && userId != e.User.Id)
        {
            // 1 if anyone can use component
            return; // TODO handle this
        }

        var additionalInfo = info.Skip(2).ToArray();

        switch (info[1])
        {
            case "raidMode" when additionalInfo.Length < 1:
            case "disableLockdown" when additionalInfo.Length < 1:
                throw new ArgumentNullException(nameof(e), nameof(additionalInfo));
            case "raidMode":
                await new RaidModeSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "disableLockdown":
                await new DisableLockdownHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            default:
                return;
        }
    }
}