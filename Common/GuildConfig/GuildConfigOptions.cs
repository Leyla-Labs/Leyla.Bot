using Common.Classes;
using Common.Comparer;
using Common.Enums;
using Common.Interfaces;
using Common.Records;
using static Common.Strings.Config;

namespace Common.GuildConfig;

public class GuildConfigOptions : IdentifiableSetProvider<ConfigOption>, ISetProvider<ConfigOption>
{
    private GuildConfigOptions() : base(CreateSet())
    {
    }

    public static SortedSet<ConfigOption> CreateSet()
    {
        var set = new SortedSet<ConfigOption>(new ConfigOptionComparer());
        AddRoles(set);
        AddChannels(set);
        AddSpam(set);
        AddRaid(set);
        return set;
    }

    private static void AddRoles(ISet<ConfigOption> set)
    {
        var c = 2;
        var t = ConfigType.Role;

        var roleOptions = new SortedSet<ConfigOption>(new ConfigOptionComparer())
        {
            new(1, 1, Roles.Mod, c, t, false, null, LeylaModule.Main),
            new(5, 2, Roles.Verification, c, t, true, null, LeylaModule.Main),
            new(14, 3, Roles.Silence, c, t, true, null, LeylaModule.Main),
            new(23, 4, Roles.RaidRole, c, t, true, null, LeylaModule.Spam)
        };
        set.UnionWith(roleOptions);
    }

    private static void AddChannels(ISet<ConfigOption> set)
    {
        var c = 3;
        var t = ConfigType.Channel;

        var channelOptions = new SortedSet<ConfigOption>(new ConfigOptionComparer())
        {
            new(2, 1, Channels.Mod, c, t, false, null, LeylaModule.Main),
            new(3, 2, Channels.Log, c, t, true, null, LeylaModule.Logs),
            new(4, 3, Channels.Archive, c, t, true, null, LeylaModule.Logs),
            new(16, 4, Channels.Silence, c, t, true, null, LeylaModule.Main),
            new(22, 5, Channels.RaidChannel, c, t, true, null, LeylaModule.Spam)
        };
        set.UnionWith(channelOptions);
    }

    private static void AddSpam(ISet<ConfigOption> set)
    {
        var c = 4;
        var d = ConfigType.Decimal;
        var m = LeylaModule.Spam;

        var spamOptions = new SortedSet<ConfigOption>(new ConfigOptionComparer())
        {
            new(6, 1, Spam.BasePressure, c, d, false, "10", m),
            new(7, 2, Spam.ImagePressure, c, d, false, "8.3", m),
            new(8, 3, Spam.LengthPressure, c, d, false, "0.00625", m),
            new(9, 4, Spam.LinePressure, c, d, false, "0.714", m),
            new(10, 5, Spam.PingPressure, c, d, false, "2.5", m),
            new(11, 6, Spam.RepeatPressure, c, d, false, "10", m),
            new(12, 7, Spam.MaxPressure, c, d, false, "60", m),
            new(13, 8, Spam.PressureDecay, c, d, false, "2.5", m),
            new(15, 9, Spam.DeleteMessages, c, ConfigType.Boolean, false, "0", m),
            new(17, 10, Spam.SilenceMessage, c, ConfigType.String, true, null, m),
            new(18, 11, Spam.Timeout, c, typeof(TimeoutDuration), false, "0", m)
        };
        set.UnionWith(spamOptions);
    }

    private static void AddRaid(ISet<ConfigOption> set)
    {
        var c = 5;
        var m = LeylaModule.Spam;

        var raidOptions = new SortedSet<ConfigOption>(new ConfigOptionComparer())
        {
            new(19, 1, Raid.RaidMode, c, ConfigType.Boolean, false, "false", m),
            new(20, 2, Raid.RaidSize, c, ConfigType.Int, false, null, m),
            new(21, 3, Raid.RaidTime, c, ConfigType.Int, false, null, m),
            new(24, 4, Raid.RaidMessage, c, ConfigType.String, true, null, m),
            new(25, 5, Raid.LockdownDuration, c, ConfigType.Int, true, "15", m),
            new(26, 6, Raid.NotifyModerators, c, ConfigType.Boolean, false, "1", m)
        };
        set.UnionWith(raidOptions);
    }

    public IEnumerable<ConfigOption> GetByCategory(int id)
    {
        return Set.Where(x => x.ConfigOptionCategory.Id == id);
    }

    public IEnumerable<ConfigOption> GetByCategory(string name)
    {
        return Set.Where(x => x.ConfigOptionCategory.Name.Equals(name));
    }

    public IEnumerable<ConfigOption> GetByCategory(ConfigOptionCategory category)
    {
        return GetByCategory(category.Id);
    }

    #region Singleton

    private static readonly Lazy<GuildConfigOptions> Lazy = new(() => new GuildConfigOptions());
    public static GuildConfigOptions Instance => Lazy.Value;

    #endregion
}