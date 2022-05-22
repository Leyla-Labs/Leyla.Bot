using Common.Classes;

namespace Common.Strings;

public static class Config
{
    public static class Roles
    {
        public static readonly DisplayString Category = new("Roles");

        public static readonly DisplayString Mod = new("Moderator Role");
        public static readonly DisplayString Verification = new("Verification Role");
        public static readonly DisplayString Silence = new("Silence Role");
    }

    public static class Channels
    {
        public static readonly DisplayString Category = new("Channels");

        public static readonly DisplayString Mod = new("Moderator Channel");
        public static readonly DisplayString Log = new("Log Channel");
        public static readonly DisplayString Archive = new("Archive Channel");
        public static readonly DisplayString Silence = new("Silence Channel");
    }

    public static class Spam
    {
        public static readonly DisplayString Category = new("Spam");

        public static readonly DisplayString BasePressure = new(Strings.Spam.BasePressure);
        public static readonly DisplayString ImagePressure = new(Strings.Spam.ImagePressure);
        public static readonly DisplayString LengthPressure = new(Strings.Spam.LengthPressure);
        public static readonly DisplayString LinePressure = new(Strings.Spam.LinePressure);
        public static readonly DisplayString PingPressure = new(Strings.Spam.PingPressure);
        public static readonly DisplayString RepeatPressure = new(Strings.Spam.RepeatPressure);
        public static readonly DisplayString MaxPressure = new(Strings.Spam.MaxPressure);
        public static readonly DisplayString PressureDecay = new(Strings.Spam.PressureDecay);
        public static readonly DisplayString DeleteMessages = new(Strings.Spam.DeleteMessages);
        public static readonly DisplayString SilenceMessage = new(Strings.Spam.SilenceMessage);
        public static readonly DisplayString Timeout = new(Strings.Spam.Timeout);
    }

    public static class Raid
    {
        public static readonly DisplayString Category = new("Raid");

        public static readonly DisplayString RaidMode = new("Raid Mode");
        public static readonly DisplayString RaidSize = new("Raid Size");
        public static readonly DisplayString RaidTime = new("Raid Time");
        public static readonly DisplayString RaidChannel = new("Raid Channel");
        public static readonly DisplayString RaidRole = new("Raid Role");
        public static readonly DisplayString RaidMessage = new("Raid Message");
    }
}