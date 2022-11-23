using DSharpPlus.Entities;

namespace Common.Interfaces;

public interface IConfigHelper<in T>
{
    Task Initialise();
    static abstract Task<string?> GetString(string option);
    static abstract Task<string?> GetString(int optionId);

    static abstract Task<string?> GetDisplayStringForDefaultValue(T option, DiscordGuild guild,
        bool allowMentions);

    static abstract Task<string?> GetDisplayStringForDefaultValue(T option, DiscordGuild guild,
        bool allowMentions, string placeholder);

    Task<string?> GetString(string option, ulong guildId);
    Task<char?> GetChar(string option, ulong guildId);
    Task<int?> GetInt(string option, ulong guildId);
    Task<bool?> GetBool(string option, ulong guildId);
    Task<ulong?> GetUlong(string option, ulong guildId);
    Task<decimal?> GetDecimal(string option, ulong guildId);
    Task<DiscordRole?> GetRole(string option, DiscordGuild guild);
    Task<DiscordChannel?> GetChannel(string option, DiscordGuild guild);
    Task<TE?> GetEnum<TE>(string option, ulong guildId) where TE : Enum;

    Task<string?> GetDisplayStringForCurrentValue(T option, DiscordGuild guild,
        bool allowMentions);

    Task<string?> GetDisplayStringForCurrentValue(T option, DiscordGuild guild,
        bool allowMentions, string placeholder);

    Task<bool> IsDefaultValue(T option, ulong guildId);
    Task<bool> Set(int optionId, ulong guildId, object value);
    Task<bool> Set(string option, ulong guildId, object value);
    Task<bool> Set(T option, ulong guildId, object value);
    Task Reset(int optionId, ulong guildId);
    Task Reset(string option, ulong guildId);
    Task Reset(T option, ulong guildId);
    Task Delete(int optionId, ulong guildId);
    Task Delete(string optionName, ulong guildId);
    Task Delete(T option, ulong guildId);
}