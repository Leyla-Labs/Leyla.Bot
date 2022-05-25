using Common.Enums;

namespace Spam.Extensions;

internal static class TimeoutDurationExtensions
{
    public static int GetMinutes(this TimeoutDuration timeoutDuration)
    {
        return timeoutDuration switch
        {
            TimeoutDuration.None => 0,
            TimeoutDuration.SixtySeconds => 1,
            TimeoutDuration.FiveMinutes => 5,
            TimeoutDuration.TenMinutes => 10,
            TimeoutDuration.OneHour => 60,
            TimeoutDuration.OneDay => 1440,
            TimeoutDuration.OneWeek => 10080,
            _ => throw new ArgumentOutOfRangeException(nameof(timeoutDuration), timeoutDuration, null)
        };
    }
}