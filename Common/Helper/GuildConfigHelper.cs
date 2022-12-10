using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Common.Db;
using Common.Db.Models;
using Common.Enums;
using Common.Extensions;
using Common.GuildConfig;
using Common.Interfaces;
using Common.Records;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Helper;

public sealed class GuildConfigHelper : IConfigHelper<DiscordGuild>
{
    private Dictionary<ulong, List<Config>> _guildConfigs = new();

    private GuildConfigHelper()
    {
    }

    public async Task InitialiseAsync()
    {
        _guildConfigs = await LoadGuildConfigsAsync();
    }

    public static string? GetString(string option)
    {
        // when no guildId provided, return default value
        return GuildConfigOptions.Instance.Get(option).DefaultValue;
    }

    public static string? GetString(int optionId)
    {
        // when no guildId provided, return default value
        return GuildConfigOptions.Instance.Get(optionId).DefaultValue;
    }

    public static string? GetDisplayStringForDefaultValue(ConfigOption option, bool allowMentions)
    {
        var defaultStr = GetString(option.Name);
        return defaultStr != null ? GetDisplayStringForGivenValue(option, null, defaultStr, allowMentions) : null;
    }

    public static string GetDisplayStringForDefaultValue(ConfigOption option, bool allowMentions,
        string placeholder)
    {
        var result = GetDisplayStringForDefaultValue(option, allowMentions);
        return result ?? placeholder;
    }

    public async Task<string?> GetStringAsync(string option, ulong entityId)
    {
        var defaultOption = GuildConfigOptions.Instance.Get(option);

        // check if guild in dictionary
        // if so, get config for that specific option
        var cfgGuild = _guildConfigs.TryGetValue(entityId, out var configs)
            ? configs.FirstOrDefault(x => x.ConfigOptionId == defaultOption.Id)
            : null;

        if (cfgGuild != null)
        {
            // Delete function sets value to string.Empty as null would return default value
            // Because of this, we need this check, as an empty string is equivalent to a non-existent config value
            // It just exists as an empty string to block the logic from resetting it to the default value
            return !string.IsNullOrWhiteSpace(cfgGuild.Value) ? cfgGuild.Value : null;
        }

        // if no config set, get default value, set for guild, and return
        // this avoids user confusion if default bot behaviour is ever changed
        if (!string.IsNullOrEmpty(defaultOption.DefaultValue))
        {
            await SetAsync(option, entityId, defaultOption.DefaultValue);
        }

        return defaultOption.DefaultValue;
    }

    public async Task<char?> GetCharAsync(string option, ulong guildId)
    {
        return await GetStringAsync(option, guildId) is { } s ? s[0] : null;
    }

    public async Task<int?> GetIntAsync(string option, ulong guildId)
    {
        return await GetStringAsync(option, guildId) is { } s ? Convert.ToInt32(s) : null;
    }

    public async Task<bool?> GetBoolAsync(string option, ulong guildId)
    {
        return await GetStringAsync(option, guildId) is { } s ? s.Equals("1") : null;
    }

    public async Task<ulong?> GetUlongAsync(string option, ulong guildId)
    {
        return await GetStringAsync(option, guildId) is { } s ? Convert.ToUInt64(s) : null;
    }

    public async Task<decimal?> GetDecimalAsync(string option, ulong guildId)
    {
        return await GetStringAsync(option, guildId) is { } s
            ? Convert.ToDecimal(s, CultureInfo.InvariantCulture)
            : null;
    }

    public async Task<DiscordRole?> GetRoleAsync(string option, DiscordGuild guild)
    {
        var roleId = await GetStringAsync(option, guild.Id) is { } s ? Convert.ToUInt64(s) : default;
        return guild.Roles.TryGetValue(roleId, out var result) ? result : null;
    }

    public async Task<DiscordChannel?> GetChannelAsync(string option, DiscordGuild guild)
    {
        var channelId = await GetStringAsync(option, guild.Id) is { } s ? Convert.ToUInt64(s) : default;
        return guild.Channels.TryGetValue(channelId, out var result) ? result : null;
    }

    public async Task<T?> GetEnumAsync<T>(string option, ulong guildId) where T : Enum
    {
        return await GetStringAsync(option, guildId) is { } s ? (T) (object) Convert.ToInt32(s) : default;
    }

    public async Task<string?> GetDisplayStringForCurrentValueAsync(ConfigOption option, DiscordGuild guild,
        bool allowMentions)
    {
        var currStr = await GetStringAsync(option.Name, guild.Id);
        return currStr != null ? GetDisplayStringForGivenValue(option, guild, currStr, allowMentions) : null;
    }

    public async Task<string?> GetDisplayStringForCurrentValueAsync(ConfigOption option, DiscordGuild guild,
        bool allowMentions, string placeholder)
    {
        var result = await GetDisplayStringForCurrentValueAsync(option, guild, allowMentions);
        return result ?? placeholder;
    }

    public async Task<bool> IsDefaultValueAsync(ConfigOption option, ulong guildId)
    {
        var currValue = await GetStringAsync(option.Name, guildId);
        return option.DefaultValue == currValue;
    }

    public async Task<bool> SetAsync(int optionId, ulong guildId, object value)
    {
        var opt = GuildConfigOptions.Instance.Get(optionId);
        return await SetInternalAsync(opt, guildId, value);
    }

    public async Task<bool> SetAsync(string option, ulong guildId, object value)
    {
        var opt = GuildConfigOptions.Instance.Get(option);
        return await SetInternalAsync(opt, guildId, value);
    }

    public async Task<bool> SetAsync(ConfigOption option, ulong guildId, object value)
    {
        return await SetInternalAsync(option, guildId, value);
    }

    public async Task ResetAsync(int optionId, ulong guildId)
    {
        var defaultValue = GetString(optionId);
        if (defaultValue == null)
        {
            throw new InvalidOperationException("Option does not have a default value");
        }

        await SetAsync(optionId, guildId, defaultValue);
    }

    public async Task ResetAsync(string option, ulong guildId)
    {
        var defaultValue = GetString(option);
        if (defaultValue == null)
        {
            throw new InvalidOperationException("Option does not have a default value");
        }

        await SetAsync(option, guildId, defaultValue);
    }

    public async Task ResetAsync(ConfigOption option, ulong guildId)
    {
        await ResetAsync(option.Id, guildId);
    }

    public async Task DeleteAsync(int optionId, ulong guildId)
    {
        var option = GuildConfigOptions.Instance.Get(optionId);
        await DeleteAsync(option, guildId);
    }

    public async Task DeleteAsync(string optionName, ulong guildId)
    {
        var option = GuildConfigOptions.Instance.Get(optionName);
        await DeleteAsync(option, guildId);
    }

    public async Task DeleteAsync(ConfigOption option, ulong guildId)
    {
        if (!option.Nullable)
        {
            throw new InvalidOperationException("Option value cannot be deleted as it is not nullable.");
        }

        await SetAsync(option, guildId, string.Empty);
    }

    private static async Task<Dictionary<ulong, List<Config>>> LoadGuildConfigsAsync()
    {
        await using var context = new DatabaseContext();

        var configs = await context.Configs.ToListAsync();
        var guilds = configs.Select(x => x.GuildId).Distinct().ToList();

        return guilds.ToDictionary(x => x, x => configs.Where(y => y.GuildId == x).ToList());
    }

    private async Task<bool> SetInternalAsync(ConfigOption opt, ulong guildId, object value)
    {
        var valueStr = opt.ConfigType switch
        {
            ConfigType.Boolean when value is bool b => b ? "1" : "0",
            ConfigType.Int when value is int => value.ToString()!,
            ConfigType.Char when value is char => value.ToString()!,
            ConfigType.Role when value is ulong => value.ToString()!,
            ConfigType.Channel when value is ulong => value.ToString()!,
            ConfigType.Role when value is DiscordRole role => role.Id.ToString(),
            ConfigType.Channel when value is DiscordChannel channel => channel.Id.ToString(),
            ConfigType.Decimal when value is decimal d => d.ToString(CultureInfo.InvariantCulture),
            ConfigType.Enum when value is Enum => ((int) value).ToString(),
            _ => GetStringFromObject(value)
        };

        if (_guildConfigs.TryGetValue(guildId, out var guildConfigs))
        {
            // guild exists in cached configs

            var curr = guildConfigs.FirstOrDefault(x => x.ConfigOptionId == opt.Id);
            if (curr != null)
            {
                // config exists in cached guild config
                // -> edit in cached config and database
                curr.Value = valueStr;
                return await EditConfigAsync(curr);
            }

            // config does not exist in cached guild config
            // -> create in cached configs and add to database
            var cCfg = new Config {ConfigOptionId = opt.Id, GuildId = guildId, Value = valueStr};
            guildConfigs.Add(cCfg);
            return await AddConfigAsync(cCfg);
        }

        {
            // guild does not exist in cached configs
            // -> create cached config for guild, add option to that and database
            var cCfg = new Config {ConfigOptionId = opt.Id, GuildId = guildId, Value = valueStr};
            var cConfigs = new List<Config> {cCfg};
            _guildConfigs.Add(guildId, cConfigs);
            return await AddConfigAsync(cCfg);
        }
    }

    private static string GetStringFromObject(object obj)
    {
        if (obj is not string s)
        {
            throw new ArgumentException("Value not handled by other cases must be of type string", nameof(obj));
        }

        return s;
    }

    private static async Task<bool> AddConfigAsync(Config config)
    {
        await using var context = new DatabaseContext();
        await context.Configs.AddAsync(config);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> EditConfigAsync(Config config)
    {
        await using var context = new DatabaseContext();
        context.Entry(config).Property(x => x.Value).IsModified = true;
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string? GetDisplayStringForGivenValue(ConfigOption option, DiscordGuild? guild, string value,
        bool allowMentions)
    {
        if (guild == null && new[] {ConfigType.Role, ConfigType.Channel}.Contains(option.ConfigType))
        {
            throw new ArgumentNullException(nameof(guild), "Guild must not be null for role and channel options");
        }

        return option.ConfigType switch
        {
            ConfigType.String => value,
            ConfigType.Boolean => value.Equals("1") ? "Yes" : "No",
            ConfigType.Int => value,
            ConfigType.Char => value,
            ConfigType.Role => guild!.Roles.TryGetValue(Convert.ToUInt64(value), out var result)
                ? allowMentions ? result.Mention : result.Name
                : null,
            ConfigType.Channel => guild!.Channels.TryGetValue(Convert.ToUInt64(value), out var result)
                ? allowMentions ? result.Mention : $"#{result.Name}"
                : null,
            ConfigType.Decimal => value,
            ConfigType.Enum when Enum.Parse(option.EnumType!, value) is { } enumVal => enumVal
                .GetAttribute<DisplayAttribute>()?.Name ?? enumVal.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option.ConfigType, "ConfigType not supported")
        };
    }

    #region Singleton

    private static readonly Lazy<GuildConfigHelper> Lazy = new(() => new GuildConfigHelper());
    public static IConfigHelper<DiscordGuild> Instance => Lazy.Value;

    #endregion
}