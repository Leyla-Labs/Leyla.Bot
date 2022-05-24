using Common.Helper;
using Common.Strings;
using DSharpPlus.Entities;

namespace Spam.Helper;

public class SilenceHelper
{
    // key guildId, value userId
    private readonly List<KeyValuePair<ulong, ulong>> _userSilences = new();

    private SilenceHelper()
    {
    }

    public async Task ProcessUserLeft(DiscordGuild guild, DiscordMember member)
    {
        var silenceRole = await ConfigHelper.Instance.GetRole(Config.Roles.Silence.Name, guild);

        if (silenceRole != null && member.Roles.Select(x => x.Id).Contains(silenceRole.Id))
        {
            _userSilences.Add(new KeyValuePair<ulong, ulong>(guild.Id, member.Id));
        }
    }

    public async Task ProcessUserJoined(DiscordGuild guild, DiscordMember member)
    {
        var entry = _userSilences.FirstOrDefault(x => x.Key == guild.Id && x.Value == member.Id);

        if (entry.Key == 0)
        {
            return;
        }

        var silenceRole = await ConfigHelper.Instance.GetRole(Config.Roles.Silence.Name, guild);

        if (silenceRole == null)
        {
            return;
        }

        await member.GrantRoleAsync(silenceRole);
        _userSilences.Remove(entry);
    }

    #region Singleton

    private static readonly Lazy<SilenceHelper> Lazy = new(() => new SilenceHelper());
    public static SilenceHelper Instance => Lazy.Value;

    #endregion
}