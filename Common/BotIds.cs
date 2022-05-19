namespace Common;

public static class BotIds
{
#if DEBUG
    public const ulong Main = 971328045501775882;
    public const ulong Logs = 969938165550972929;
    public const ulong Spam = 975811281820975206;
#else
    // TODO add prod ids
    public const ulong Main = 000000000000000000;
    public const ulong Logs = 000000000000000000;
    public const ulong Spam = 000000000000000000;
#endif
}