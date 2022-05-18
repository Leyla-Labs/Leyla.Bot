using DSharpPlus.Entities;

namespace Spam.Classes;

public class MaxPressureExceededEventArgs
{
    public readonly decimal MaxPressure;
    public readonly List<DiscordMessage> SessionMessages;
    public readonly decimal UserPressure;

    public MaxPressureExceededEventArgs(UserPressure userPressure, decimal maxPressure)
    {
        SessionMessages = userPressure.PressureSessionMessages;
        MaxPressure = maxPressure;
        UserPressure = userPressure.CurrentPressure;
    }
}