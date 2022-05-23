using DSharpPlus.Entities;
using Timer = System.Timers.Timer;

namespace Spam.Classes;

public class LockdownTimer : Timer
{
    public readonly DiscordGuild DiscordGuild;
    public readonly VerificationLevel VerificationLevel;

    public LockdownTimer(DiscordGuild guild, int durationMinutes) : base(durationMinutes * 60000)
    {
        DiscordGuild = guild;
        VerificationLevel = guild.VerificationLevel;
    }
}