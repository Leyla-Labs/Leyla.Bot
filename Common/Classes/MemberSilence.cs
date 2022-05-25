using DSharpPlus.Entities;

namespace Common.Classes;

public class MemberSilence
{
    public readonly DiscordMember Member;
    public readonly DateTime? SilenceUntil;

    public MemberSilence(DiscordMember member)
    {
        Member = member;
    }

    public MemberSilence(DiscordMember member, DateTime silenceUntil)
    {
        Member = member;
        var utc = silenceUntil.ToUniversalTime();
        SilenceUntil = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, 0);
    }
}