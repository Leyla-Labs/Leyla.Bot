using DSharpPlus;
using DSharpPlus.Entities;

namespace Spam.Extensions;

internal static class DiscordMemberHelper
{
    public static string GetMemberRaidString(this DiscordMember m)
    {
        var dateStr = Formatter.Timestamp(m.JoinedAt);
        return $"{m.Username}#{m.Discriminator} | {m.Id} | {dateStr}";
    }
}