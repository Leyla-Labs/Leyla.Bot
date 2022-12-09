using System.Text;
using System.Timers;
using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;
using Spam.Extensions;
using static Common.Strings.Config.Raid;

namespace Spam.Helper;

internal delegate Task RaidDetecedHandler(DiscordClient sender, RaidDetectedEventArgs args);

internal class RaidHelper
{
    private readonly List<LockdownTimer> _lockdownTimers = new();
    private readonly Dictionary<ulong, List<DiscordMember>> _recentJoins = new();

    private RaidHelper()
    {
    }

    public static event RaidDetecedHandler? RaidDetected;

    public async Task AddMemberAsync(DiscordClient sender, DiscordMember member)
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

            if (lastJoin.JoinedAt.UtcDateTime < DateTime.UtcNow.AddMinutes(-1 * raidTime))
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

        var r = GetRaidMembers(guildId, raidSize);
        if (r?.Count == raidSize) // only trigger once limit hit the first time
        {
            RaidDetected?.Invoke(sender, new RaidDetectedEventArgs(r));
            // TODO add cooldown time during which no further notifications to moderators are sent
        }
    }

    public async Task<List<DiscordMember>?> GetRaidMembersAsync(ulong guildId)
    {
        var raidSize = await ConfigHelper.Instance.GetInt(RaidSize.Name, guildId) ?? 0;
        return GetRaidMembers(guildId, raidSize);
    }

    private List<DiscordMember>? GetRaidMembers(ulong guildId, int raidSize)
    {
        _recentJoins.TryGetValue(guildId, out var guildJoins);
        return guildJoins?.Count >= raidSize ? guildJoins : null;
    }

    public async Task<DiscordEmbed> EnableRaidModeAndGetEmbedAsync(DiscordGuild guild)
    {
        await ConfigHelper.Instance.Set(RaidMode.Name, guild.Id, true);

        var raidSize = await ConfigHelper.Instance.GetInt(RaidSize.Name, guild.Id) ?? 0;

        // todo also check if people in list joined within raid time

        var membersToSilence = _recentJoins.TryGetValue(guild.Id, out var guildList) && guildList.Count >= raidSize
            ? guildList
            : new List<DiscordMember>();

        await AddRaidRoleToMembersAsync(guild, membersToSilence);
        await MentionMembersInRaidChannelAsync(guild, membersToSilence);

        return GetRaidModeEnabledEmbed(membersToSilence);
    }

    private static async Task AddRaidRoleToMembersAsync(DiscordGuild guild, IEnumerable<DiscordMember> members)
    {
        var raidRole = await ConfigHelper.Instance.GetRole(RaidRole.Name, guild);

        if (raidRole == null)
        {
            return;
        }

        foreach (var m in members.Where(x => !x.Roles.Select(y => y.Id).Contains(raidRole.Id)))
        {
            await m.GrantRoleAsync(raidRole);
        }
    }

    public static async Task MentionMembersInRaidChannelAsync(DiscordGuild guild, DiscordMember member)
    {
        await MentionMembersInRaidChannelAsync(guild, new List<DiscordMember> {member});
    }

    public static async Task MentionMembersInRaidChannelAsync(DiscordGuild guild, List<DiscordMember> members)
    {
        var raidMessage =
            await ConfigHelper.Instance.GetString(RaidMessage.Name, guild.Id);
        var raidChannel = await ConfigHelper.Instance.GetChannel(RaidChannel.Name, guild);

        if (!string.IsNullOrWhiteSpace(raidMessage) && raidChannel != null)
        {
            foreach (var member in members)
            {
                await raidChannel.SendMessageAsync($"{member.Mention} {raidMessage}");
            }
        }
    }

    private static DiscordEmbed GetRaidModeEnabledEmbed(IReadOnlyCollection<DiscordMember> membersToSilence)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Raid Mode Enabled");

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

    public async Task EnableLockdownAsync(DiscordGuild guild, int duration)
    {
        var timer = new LockdownTimer(guild, duration);
        timer.Elapsed += TimerOnElapsed;

        await guild.ModifyAsync(x => x.VerificationLevel = VerificationLevel.High);

        _lockdownTimers.Add(timer);
        timer.Start();
    }

    private async void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        var lockdownTimer = (LockdownTimer) (sender ?? throw new NullReferenceException(nameof(sender)));
        lockdownTimer.Stop();
        _lockdownTimers.Remove(lockdownTimer);
        await lockdownTimer.DiscordGuild.ModifyAsync(x => x.VerificationLevel = lockdownTimer.VerificationLevel);
        await SendLockdownDisabledMessageAsync(lockdownTimer.DiscordGuild);
    }

    public static async Task SendLockdownDisabledMessageAsync(DiscordGuild guild)
    {
        var modChannel = await ConfigHelper.Instance.GetChannel(Config.Channels.Mod.Name, guild);
        if (modChannel == null)
        {
            // handle this differently?
            return;
        }

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Lockdown Disabled");
        embed.WithDescription(
            $"Lockdown has been disabled. The verification level was reverted to {guild.VerificationLevel}.");
        embed.WithColor(DiscordColor.Blurple);

        await modChannel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed.Build()));
    }

    public void StopLockdownTimer(ulong guildId)
    {
        var timer = _lockdownTimers.FirstOrDefault(x => x.DiscordGuild.Id == guildId);
        if (timer == null)
        {
            return;
        }

        timer.Stop();
        _lockdownTimers.Remove(timer);
    }

    #region Singleton

    private static readonly Lazy<RaidHelper> Lazy = new(() => new RaidHelper());
    public static RaidHelper Instance => Lazy.Value;

    #endregion
}