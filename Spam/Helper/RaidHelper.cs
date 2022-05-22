using System.Text;
using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;
using Spam.Extensions;
using static Common.Strings.Config.Raid;

namespace Spam.Helper;

internal delegate void RaidDetecedHandler(DiscordClient sender, RaidDetectedEventArgs args);

internal class RaidHelper
{
    private readonly Dictionary<ulong, List<DiscordMember>> _recentJoins = new();

    private RaidHelper()
    {
    }

    public static event RaidDetecedHandler? RaidDetected;

    public async Task AddMember(DiscordClient sender, DiscordMember member)
    {
        var guildId = member.Guild.Id;

        var raidSize = await ConfigHelper.Instance.GetInt(RaidSize.Name, guildId) ?? 0;
        var raidTime = await ConfigHelper.Instance.GetInt(RaidTime.Name, guildId) ?? 0;

        if (raidTime == 0 || raidSize == 0)
        {
            // do nothing if config parameters are not configured
            return;
        }

        if (_recentJoins.TryGetValue(guildId, out var guildList))
        {
            var lastJoin = guildList.LastOrDefault();

            if (lastJoin == null)
            {
                // no entries in guild list, add first entry
                guildList.Add(member);
                return;
            }

            if (member.JoinedAt.UtcDateTime < DateTime.UtcNow.AddMinutes(-1 * raidTime))
            {
                // it's been longer than the defined raid time since the last join, clear list
                guildList.Clear();
            }

            guildList.Add(member);
        }
        else
        {
            // guild not in dictionary, create guild entry and add member as first entry
            _recentJoins.Add(guildId, new List<DiscordMember>());
            var list = _recentJoins[guildId];
            list.Add(member);
        }

        var r = _recentJoins[guildId];
        if (r.Count >= raidSize)
        {
            RaidDetected?.Invoke(sender, new RaidDetectedEventArgs(r));
        }
    }

    public async Task EnableRaidMode(DiscordGuild guild, DiscordChannel channel)
    {
        await ConfigHelper.Instance.Set(RaidMode.Name, guild.Id, true);

        var raidSize = await ConfigHelper.Instance.GetInt(RaidSize.Name, guild.Id) ?? 0;

        var membersToSilence = _recentJoins.TryGetValue(guild.Id, out var guildList) && guildList.Count >= raidSize
            ? guildList
            : new List<DiscordMember>();

        await SilenceMembers(guild, membersToSilence);

        var embed = GetRaidModeEnabledEmbed(membersToSilence);
        await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed));
    }

    private static async Task SilenceMembers(DiscordGuild guild, List<DiscordMember> members)
    {
        var silenceRole = await ConfigHelper.Instance.GetRole(Config.Roles.Silence.Name, guild);

        if (silenceRole == null)
        {
            return;
        }

        foreach (var m in members.Where(x => !x.Roles.Select(y => y.Id).Contains(silenceRole.Id)))
        {
            await m.GrantRoleAsync(silenceRole);
        }
    }

    private static DiscordEmbed GetRaidModeEnabledEmbed(IReadOnlyCollection<DiscordMember> membersToSilence)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Raid Mode Active");

        var sb = new StringBuilder();
        if (membersToSilence.Count > 0)
        {
            sb.Append($"The following members have been silenced:{Environment.NewLine}{Environment.NewLine}");
            sb.Append(string.Join(Environment.NewLine, membersToSilence.Select(x => x.GetMemberRaidString())));
            sb.Append($"{Environment.NewLine}{Environment.NewLine}");
        }

        sb.Append("All members joining the server going forward will be silenced until raid mode is turned off.");
        embed.WithDescription(sb.ToString());
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #region Singleton

    private static readonly Lazy<RaidHelper> Lazy = new(() => new RaidHelper());
    public static RaidHelper Instance => Lazy.Value;

    #endregion
}