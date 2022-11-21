using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Common.Db;
using Common.Db.Models;
using Common.Enums;
using Common.Extensions;
using Common.GuildConfig;
using Common.Records;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Helper;

public sealed class ConfigHelper
{
    private Dictionary<ulong, List<Config>> _guildConfigs = new();

    private ConfigHelper()
    {
    }

    public async Task Initialise()
    {
        _guildConfigs = await LoadGuildConfigs();
    }

    private static async Task<Dictionary<ulong, List<Config>>> LoadGuildConfigs()
    {
        await using var context = new DatabaseContext();

        var configs = await context.Configs.ToListAsync();
        var guilds = configs.Select(x => x.GuildId).Distinct().ToList();

        return guilds.ToDictionary(x => x, x => configs.Where(y => y.GuildId == x).ToList());
    }

    public static Task<string?> GetString(string option)
    {
        // when no guildId provided, return default value
        return Task.FromResult(ConfigOptions.Instance.Get(option).DefaultValue);
    }

    public static Task<string?> GetString(int optionId)
    {
        // when no guildId provided, return default value
        return Task.FromResult(ConfigOptions.Instance.Get(optionId).DefaultValue);
    }

    public static async Task<string?> GetDisplayStringForDefaultValue(ConfigOption option, DiscordGuild guild,
        bool allowMentions)
    {
        var defaultStr = await GetString(option.Name);
        return defaultStr != null ? GetDisplayStringForGivenValue(option, guild, defaultStr, allowMentions) : null;
    }

    public static async Task<string?> GetDisplayStringForDefaultValue(ConfigOption option, DiscordGuild guild,
        bool allowMentions, string placeholder)
    {
        var result = await GetDisplayStringForDefaultValue(option, guild, allowMentions);
        return result ?? placeholder;
    }

    public async Task<string?> GetString(string option, ulong guildId)
    {
        var defaultOption = ConfigOptions.Instance.Get(option);

        // check if guild in dictionary
        // if so, get config for that specific option
        var cfgGuild = _guildConfigs.TryGetValue(guildId, out var configs)
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
            await Set(option, guildId, defaultOption.DefaultValue);
        }

        return defaultOption.DefaultValue;
    }

    public async Task<char?> GetChar(string option, ulong guildId)
    {
        return await GetString(option, guildId) is { } s ? s[0] : null;
    }

    public async Task<int?> GetInt(string option, ulong guildId)
    {
        return await GetString(option, guildId) is { } s ? Convert.ToInt32(s) : null;
    }

    public async Task<bool?> GetBool(string option, ulong guildId)
    {
        return await GetString(option, guildId) is { } s ? s.Equals("1") : null;
    }

    public async Task<ulong?> GetUlong(string option, ulong guildId)
    {
        return await GetString(option, guildId) is { } s ? Convert.ToUInt64(s) : null;
    }

    public async Task<decimal?> GetDecimal(string option, ulong guildId)
    {
        return await GetString(option, guildId) is { } s ? Convert.ToDecimal(s, CultureInfo.InvariantCulture) : null;
    }

    public async Task<DiscordRole?> GetRole(string option, DiscordGuild guild)
    {
        var roleId = await GetString(option, guild.Id) is { } s ? Convert.ToUInt64(s) : default;
        return guild.Roles.TryGetValue(roleId, out var result) ? result : null;
    }

    public async Task<DiscordChannel?> GetChannel(string option, DiscordGuild guild)
    {
        var channelId = await GetString(option, guild.Id) is { } s ? Convert.ToUInt64(s) : default;
        return guild.Channels.TryGetValue(channelId, out var result) ? result : null;
    }

    public async Task<T?> GetEnum<T>(string option, ulong guildId) where T : Enum
    {
        return await GetString(option, guildId) is { } s ? (T) (object) Convert.ToInt32(s) : default;
    }

    public async Task<string?> GetDisplayStringForCurrentValue(ConfigOption option, DiscordGuild guild,
        bool allowMentions)
    {
        var currStr = await GetString(option.Name, guild.Id);
        return currStr != null ? GetDisplayStringForGivenValue(option, guild, currStr, allowMentions) : null;
    }

    public async Task<string?> GetDisplayStringForCurrentValue(ConfigOption option, DiscordGuild guild,
        bool allowMentions, string placeholder)
    {
        var result = await GetDisplayStringForCurrentValue(option, guild, allowMentions);
        return result ?? placeholder;
    }

    public async Task<bool> IsDefaultValue(ConfigOption option, ulong guildId)
    {
        var currValue = await GetString(option.Name, guildId);
        return option.DefaultValue == currValue;
    }

    public async Task<bool> Set(int optionId, ulong guildId, object value)
    {
        var opt = ConfigOptions.Instance.Get(optionId);
        return await SetInternal(opt, guildId, value);
    }

    public async Task<bool> Set(string option, ulong guildId, object value)
    {
        var opt = ConfigOptions.Instance.Get(option);
        return await SetInternal(opt, guildId, value);
    }

    public async Task<bool> Set(ConfigOption option, ulong guildId, object value)
    {
        return await SetInternal(option, guildId, value);
    }

    public async Task Reset(int optionId, ulong guildId)
    {
        var defaultValue = await GetString(optionId);
        if (defaultValue == null)
        {
            throw new InvalidOperationException("Option does not have a default value");
        }

        await Set(optionId, guildId, defaultValue);
    }

    public async Task Reset(string option, ulong guildId)
    {
        var defaultValue = await GetString(option);
        if (defaultValue == null)
        {
            throw new InvalidOperationException("Option does not have a default value");
        }

        await Set(option, guildId, defaultValue);
    }

    public async Task Reset(ConfigOption option, ulong guildId)
    {
        await Reset(option.Id, guildId);
    }

    public async Task Delete(int optionId, ulong guildId)
    {
        var option = ConfigOptions.Instance.Get(optionId);
        await Delete(option, guildId);
    }

    public async Task Delete(string optionName, ulong guildId)
    {
        var option = ConfigOptions.Instance.Get(optionName);
        await Delete(option, guildId);
    }

    public async Task Delete(ConfigOption option, ulong guildId)
    {
        if (!option.Nullable)
        {
            throw new InvalidOperationException("Option value cannot be deleted as it is not nullable.");
        }

        await Set(option, guildId, string.Empty);
    }

    private async Task<bool> SetInternal(ConfigOption opt, ulong guildId, object value)
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
                return await EditConfig(curr);
            }

            // config does not exist in cached guild config
            // -> create in cached configs and add to database
            var cCfg = new Config {ConfigOptionId = opt.Id, GuildId = guildId, Value = valueStr};
            guildConfigs.Add(cCfg);
            return await AddConfig(cCfg);
        }

        {
            // guild does not exist in cached configs
            // -> create cached config for guild, add option to that and database
            var cCfg = new Config {ConfigOptionId = opt.Id, GuildId = guildId, Value = valueStr};
            var cConfigs = new List<Config> {cCfg};
            _guildConfigs.Add(guildId, cConfigs);
            return await AddConfig(cCfg);
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

    private static async Task<bool> AddConfig(Config config)
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

    private static async Task<bool> EditConfig(Config config)
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

    private static string? GetDisplayStringForGivenValue(ConfigOption option, DiscordGuild guild, string value,
        bool allowMentions)
    {
        return option.ConfigType switch
        {
            ConfigType.String => value,
            ConfigType.Boolean => value.Equals("1") ? "Yes" : "No",
            ConfigType.Int => value,
            ConfigType.Char => value,
            ConfigType.Role => guild.Roles.TryGetValue(Convert.ToUInt64(value), out var result)
                ? allowMentions ? result.Mention : result.Name
                : null,
            ConfigType.Channel => guild.Channels.TryGetValue(Convert.ToUInt64(value), out var result)
                ? allowMentions ? result.Mention : $"#{result.Name}"
                : null,
            ConfigType.Decimal => value,
            ConfigType.Enum when Enum.Parse(option.EnumType!, value) is { } enumVal => enumVal
                .GetAttribute<DisplayAttribute>()?.Name ?? enumVal.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option.ConfigType, "ConfigType not supported")
        };
    }

    #region Singleton

    private static readonly Lazy<ConfigHelper> Lazy = new(() => new ConfigHelper());
    public static ConfigHelper Instance => Lazy.Value;

    #endregion
}