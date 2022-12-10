using Common.Records;

namespace Common.Strings;

public static class Config
{
    private static string UrlConfig => "https://github.com/Leyla-Labs/Leyla.Bot/wiki/Server-Configuration";

    public static class Roles
    {
        public static readonly ConfigStrings Category = new("Roles", "Various roles for different modules.", UrlRoles);

        public static readonly ConfigStrings Mod = new("Moderator Role",
            "Common role for all moderators. Leyla will mention this role in various situations.",
            $"{UrlRoles}#moderator-role",
            "Required for bot operation.");

        public static readonly ConfigStrings Verification = new("Verification Role",
            "If your server uses a manual verification system, you can give this role to people by right clicking them.",
            $"{UrlRoles}#verification-role");

        public static readonly ConfigStrings Silence = new("Silence Role",
            "When manually silencing or when anti-spam is triggered by a user, this role will be given to them.",
            $"{UrlRoles}#silence-role");

        public static readonly ConfigStrings RaidRole = new("Raid Role",
            "If `Raid Mode` is on, all members joining the server will be assigned this role.",
            $"{UrlRoles}#raid-role");

        private static string UrlRoles => $"{UrlConfig}:-Roles";
    }

    public static class Channels
    {
        public static readonly ConfigStrings Category = new("Channels", "Various channels for different modules.",
            UrlChannels);

        public static readonly ConfigStrings Mod = new("Moderator Channel",
            "Leyla will post system messages and certain messages from the `Logs` and `Spam` modules in this channel.",
            $"{UrlChannels}#moderator-channel");

        public static readonly ConfigStrings Log = new("Log Channel",
            "`Leyla Logs` will post various log messages in this channel, like message deletions and edits.",
            $"{UrlChannels}#log-channel");

        public static readonly ConfigStrings Archive = new("Archive Channel", "Currently unused.",
            $"{UrlChannels}#archive-channel");

        public static readonly ConfigStrings Silence = new("Silence Channel",
            "Members with the Silence Role will have access to this channel.",
            $"{UrlChannels}#silence-channel");

        public static readonly ConfigStrings RaidChannel = new("Raid Channel",
            "Members with the Raid Role will have access to this channel.",
            $"{UrlChannels}#raid-channel");

        private static string UrlChannels => $"{UrlConfig}:-Channels";
    }

    public static class Spam
    {
        public static readonly ConfigStrings Category = new("Spam", "Anti-spam settings.", UrlSpam);

        public static readonly ConfigStrings BasePressure = new(Strings.Spam.BasePressure,
            "The base pressure of a message. This is the amount of pressure added to a user when they send a message.",
            $"{UrlSpam}#base-pressure");

        public static readonly ConfigStrings ImagePressure = new(Strings.Spam.ImagePressure,
            "The pressure added to a user per image/link in a message. Added on top of base pressure.",
            $"{UrlSpam}#image-pressure");

        public static readonly ConfigStrings LengthPressure = new(Strings.Spam.LengthPressure,
            "The pressure added to a user per character in a message. Added on top of base pressure.",
            $"{UrlSpam}#length-pressure");

        public static readonly ConfigStrings LinePressure = new(Strings.Spam.LinePressure,
            "The pressure added to a user per newline in a message. Added on top of base pressure.",
            $"{UrlSpam}#line-pressure");

        public static readonly ConfigStrings PingPressure = new(Strings.Spam.PingPressure,
            "The pressure added to a user per ping in a message. Added on top of base pressure.",
            $"{UrlSpam}#ping-pressure");

        public static readonly ConfigStrings RepeatPressure = new(Strings.Spam.RepeatPressure,
            "The pressure added to a user when they send a message that is the same as the previous one. Added on top of base pressure.",
            $"{UrlSpam}#repeat-pressure");

        public static readonly ConfigStrings MaxPressure = new(Strings.Spam.MaxPressure,
            "The maximum pressure a user can have. If a user exceeds this, they will be silenced using the `Silence Role.`",
            $"{UrlSpam}#max-pressure");

        public static readonly ConfigStrings PressureDecay =
            new(Strings.Spam.PressureDecay, "The amount of pressure a user loses per second.",
                $"{UrlSpam}#pressure-decay");

        public static readonly ConfigStrings DeleteMessages =
            new(Strings.Spam.DeleteMessages,
                "If this is on, all messages that caused a user to get silenced will be deleted.",
                $"{UrlSpam}#delete-messages");

        public static readonly ConfigStrings SilenceMessage =
            new(Strings.Spam.SilenceMessage, "The message to send in the `Silence Channel` when a user is silenced.",
                $"{UrlSpam}#silence-message");

        public static readonly ConfigStrings Timeout = new(Strings.Spam.Timeout,
            "Optionally use Discord's built-in timeout feature. Not recommended.",
            $"{UrlSpam}#timeout");

        private static string UrlSpam => $"{UrlConfig}:-Spam";
    }

    public static class Raid
    {
        public static readonly ConfigStrings Category = new("Raid", "Anti-raid settings.", UrlRaid);

        public static readonly ConfigStrings RaidMode = new("Raid Mode",
            "Whether raid mode is enabled. When raid mode is enabled, new members will be assigned the Raid Role as soon as they join the server.",
            $"{UrlRaid}#raid-mode");

        public static readonly ConfigStrings RaidSize = new("Raid Size",
            "The amount of members that must join the server within the `Raid Time` to trigger raid mode.",
            $"{UrlRaid}#raid-size");

        public static readonly ConfigStrings RaidTime = new("Raid Time",
            "The amount of minutes in which `Raid Size` members must join the server to trigger raid mode.",
            $"{UrlRaid}#raid-time");

        public static readonly ConfigStrings RaidMessage = new("Raid Message",
            "The message to send in the `Raid Channel` when a user is assigned the `Raid Role`.",
            $"{UrlRaid}#raid-message");

        public static readonly ConfigStrings LockdownDuration = new("Lockdown Duration",
            "The amount of minutes to lock down the server for when a raid is detected. The verification level will be set to `High` and reset afterwards.",
            $"{UrlRaid}#lockdown-duration");

        public static readonly ConfigStrings NotifyModerators = new("Notify Moderators",
            "Leyla will always send a message in the `Mod Channel` when a raid is detected. If this option is enabled, Leyla will mention the `Moderator Role` alongside said message.",
            $"{UrlRaid}#notify-moderators");

        private static string UrlRaid => $"{UrlConfig}:-Raid";
    }
}