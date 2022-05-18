using DSharpPlus.Entities;

namespace Spam.Classes;

public class MaxPressureExceededEventArgs
{
    public readonly decimal MaxPressure;
    public readonly DiscordMessage Message;
    public readonly decimal UserPressure;

    public MaxPressureExceededEventArgs(DiscordMessage message, decimal maxPressure,
        decimal userPressure)
    {
        Message = message;
        MaxPressure = maxPressure;
        UserPressure = userPressure;
    }
}