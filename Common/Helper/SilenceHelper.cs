using Common.Classes;
using Common.Strings;
using DSharpPlus.Entities;

namespace Common.Helper;

public class SilenceHelper
{
    private readonly List<MemberSilence> _memberSilences = new();

    private SilenceHelper()
    {
    }

    public async Task ProcessUserLeft(DiscordGuild guild, DiscordMember member)
    {
        var silenceRole = await ConfigHelper.Instance.GetRole(Config.Roles.Silence.Name, guild);

        if (silenceRole != null && member.Roles.Select(x => x.Id).Contains(silenceRole.Id) &&
            _memberSilences.All(x => x.Member.Guild.Id != member.Guild.Id || x.Member.Id != member.Id))
        {
            _memberSilences.Add(new MemberSilence(member));
        }
    }

    public async Task ProcessUserJoined(DiscordGuild guild, DiscordMember member)
    {
        var entry = _memberSilences.FirstOrDefault(x => x.Member.Guild.Id == guild.Id && x.Member.Id == member.Id);

        if (entry == null)
        {
            return;
        }

        if (entry.SilenceUntil != null && entry.SilenceUntil < DateTime.UtcNow)
        {
            // user was temporarily silenced and silence expired while user was not in guild
            _memberSilences.Remove(entry);
            return;
        }

        var silenceRole = await ConfigHelper.Instance.GetRole(Config.Roles.Silence.Name, guild);

        if (silenceRole == null)
        {
            return;
        }

        await member.GrantRoleAsync(silenceRole);
        _memberSilences.Remove(entry);
    }

    public DateTime AddTimedSilence(DiscordMember member, DateTime until)
    {
        var current = _memberSilences.FirstOrDefault(x =>
            x.Member.Guild.Id == member.Guild.Id &&
            x.Member.Id == member.Id);

        if (current != null)
        {
            // replace existing silence with new timed silence
            _memberSilences.Remove(current);
        }

        until = until.AddSeconds(Convert.ToInt32(decimal.Multiply(until.Second, -1)));
        _memberSilences.Add(new MemberSilence(member, until));
        return until;
    }

    public List<DiscordMember> GetMembersToUnsilence()
    {
        return _memberSilences.Where(x =>
                x.SilenceUntil != null &&
                x.SilenceUntil <= DateTime.UtcNow)
            .Select(x => x.Member)
            .ToList();
    }

    public void RemoveTimedSilence(ulong guildId, ulong userId)
    {
        var entry = _memberSilences.FirstOrDefault(x =>
            x.Member.Guild.Id == guildId &&
            x.Member.Id == userId &&
            x.SilenceUntil != null);

        if (entry != null)
        {
            _memberSilences.Remove(entry);
        }
    }

    #region Singleton

    private static readonly Lazy<SilenceHelper> Lazy = new(() => new SilenceHelper());
    public static SilenceHelper Instance => Lazy.Value;

    #endregion
}