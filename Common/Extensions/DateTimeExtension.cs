namespace Common.Extensions;

public static class DateTimeExtension
{
    public static string GetDisplayString(this DateTime dateTime)
    {
        return $"{dateTime:dd.MM.yyyy HH:mm}";
    }

    public static DateTime? GetDateTimeFromDisplayString(this string displayString)
    {
        try
        {
            // i hate this so much
            var day = Convert.ToInt32(displayString[..2]);
            var month = Convert.ToInt32(displayString.Substring(3, 2));
            var year = Convert.ToInt32(displayString.Substring(6, 4));
            var hour = Convert.ToInt32(displayString.Substring(11, 2));
            var minute = Convert.ToInt32(displayString.Substring(14, 2));
            return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
        }
        catch
        {
            return null;
        }
    }
}