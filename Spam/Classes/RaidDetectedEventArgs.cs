using DSharpPlus.Entities;

namespace Spam.Classes;

internal class RaidDetectedEventArgs
{
    public readonly List<DiscordMember> RaidMembers;

    public RaidDetectedEventArgs(List<DiscordMember> raidMembers)
    {
        RaidMembers = raidMembers;
    }
}