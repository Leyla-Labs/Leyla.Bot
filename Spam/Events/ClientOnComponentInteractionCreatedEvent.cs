using DSharpPlus;
using DSharpPlus.EventArgs;
using Spam.Handler;

namespace Spam.Events;

public static class ClientOnComponentInteractionCreatedEvent
{
    public static async Task HandleEvent(DiscordClient sender,
        ComponentInteractionCreateEventArgs e)
    {
        // info consists of userId, name, and any further information after that
        var info = e.Id.Split("-");
        var userId = Convert.ToUInt64(info[0]);

        if (userId != 1 && userId == e.User.Id)
        {
            // 1 if anyone can use component
            return; // TODO handle this
        }

        var additionalInfo = info.Skip(2).ToArray();

        switch (info[1])
        {
            case "raidMode" when additionalInfo.Length < 1:
                throw new NullReferenceException(nameof(additionalInfo));
            case "raidMode":
                await new RaidModeSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            default:
                return;
        }
    }
}