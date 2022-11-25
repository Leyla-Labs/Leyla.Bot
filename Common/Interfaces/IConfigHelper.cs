using Common.Records;
using DSharpPlus.Entities;

namespace Common.Interfaces;

public interface IConfigHelper<in T> where T : SnowflakeObject
{
    /// <summary>
    ///     Loads configs from the database. Must be run on startup.
    /// </summary>
    Task Initialise();

    /// <summary>
    ///     Get default value for a config option.
    /// </summary>
    /// <param name="option">Name of the config option.</param>
    /// <returns>Default value for the given config option.</returns>
    static abstract Task<string?> GetString(string option);

    /// <summary>
    ///     Get default value for a config option.
    /// </summary>
    /// <param name="optionId">Id of the config option.</param>
    static abstract Task<string?> GetString(int optionId);

    /// <summary>
    ///     Get human readable string for the default value of a config option.
    /// </summary>
    /// <param name="option">ConfigOption to get display string for.</param>
    /// <param name="allowMentions">If the returned string is allowed to be a mention. Avoid in select menus.</param>
    /// <returns>Human readable string.</returns>
    static abstract Task<string?> GetDisplayStringForDefaultValue(ConfigOption option, bool allowMentions);

    /// <inheritdoc cref="GetDisplayStringForDefaultValue(ConfigOption, bool)" />
    /// <param name="placeholder">Value to return if option does not have a default value.</param>
    /// <remarks>This overload is meant to be used when the returned value must not be null.</remarks>
    static abstract Task<string?> GetDisplayStringForDefaultValue(ConfigOption option, bool allowMentions,
        string placeholder);

    /// <summary>
    ///     Get current value for a config option.
    /// </summary>
    /// <param name="option">Name of the config option.</param>
    /// <param name="entityId">Id of entity to get value for.</param>
    /// <returns>Current value as string.</returns>
    Task<string?> GetString(string option, ulong entityId);

    /// <inheritdoc cref="GetString(string, ulong)" />
    /// <returns>Current value as char.</returns>
    Task<char?> GetChar(string option, ulong entityId);

    /// <inheritdoc cref="GetString(string, ulong)" />
    /// <returns>Current value as integer.</returns>
    Task<int?> GetInt(string option, ulong entityId);

    /// <inheritdoc cref="GetString(string, ulong)" />
    /// <returns>Current value as boolean.</returns>
    Task<bool?> GetBool(string option, ulong entityId);

    /// <inheritdoc cref="GetString(string, ulong)" />
    /// <returns>Current value as ulong.</returns>
    Task<ulong?> GetUlong(string option, ulong entityId);

    /// <inheritdoc cref="GetString(string, ulong)" />
    /// <returns>Current value as decimal.</returns>
    Task<decimal?> GetDecimal(string option, ulong entityId);

    /// <inheritdoc cref="GetString(string, ulong)" />
    /// <returns>Current value as the given enum.</returns>
    Task<TE?> GetEnum<TE>(string option, ulong entityId) where TE : Enum;

    /// <summary>
    ///     Get current value for a config option.
    /// </summary>
    /// <param name="option">Name of the config option.</param>
    /// <param name="entity">Entity to get value for.</param>
    /// <returns>Current value as role.</returns>
    Task<DiscordRole?> GetRole(string option, T entity);

    /// <inheritdoc cref="GetRole(string, T)" />
    /// <returns>Current value as role.</returns>
    Task<DiscordChannel?> GetChannel(string option, T entity);

    /// <summary>
    ///     Get human readable string for the current value of a config option.
    /// </summary>
    /// <param name="option">ConfigOption to get display string for.</param>
    /// <param name="entity">Entity to get value for.</param>
    /// <param name="allowMentions">If the returned string is allowed to be a mention. Avoid in select menus.</param>
    /// <returns>Human readable string.</returns>
    Task<string?> GetDisplayStringForCurrentValue(ConfigOption option, T entity, bool allowMentions);

    /// <inheritdoc cref="GetDisplayStringForCurrentValue(ConfigOption, T ,bool)" />
    /// <param name="placeholder">Value to return if option does not have a default value.</param>
    /// <remarks>This overload is meant to be used when the returned value must not be null.</remarks>
    Task<string?> GetDisplayStringForCurrentValue(ConfigOption option, T entity, bool allowMentions,
        string placeholder);

    /// <summary>
    ///     Check if the currently set value is equal to the default value for the given config option.
    /// </summary>
    /// <param name="option">ConfigOption the given value is for.</param>
    /// <param name="entityId">Id of entity to get value for.</param>
    /// <returns>True if current value is equal to the option's default value.</returns>
    Task<bool> IsDefaultValue(ConfigOption option, ulong entityId);

    /// <summary>
    ///     Set the value for a config option for the given Entity.
    /// </summary>
    /// <param name="optionId">Id of the config option.</param>
    /// <param name="entityId">Id of the entity to set value for.</param>
    /// <param name="value">Value to set for given option and entity.</param>
    /// <returns>True if setting the value was successful.</returns>
    Task<bool> Set(int optionId, ulong entityId, object value);

    /// <summary>
    ///     Set the value for a config option for the given Entity.
    /// </summary>
    /// <param name="option">Name of the config option.</param>
    /// <param name="entityId">Id of the entity to set value for.</param>
    /// <param name="value">Value to set for given option and entity.</param>
    /// <returns>True if setting the value was successful.</returns>
    Task<bool> Set(string option, ulong entityId, object value);

    /// <summary>
    ///     Set the value for a config option for the given Entity.
    /// </summary>
    /// <param name="option">The config option.</param>
    /// <param name="entityId">Id of the entity to set value for.</param>
    /// <param name="value">Value to set for given option and entity.</param>
    /// <returns>True if setting the value was successful.</returns>
    Task<bool> Set(ConfigOption option, ulong entityId, object value);

    /// <summary>
    ///     Reset the value for a config option for the given Entity to its default value.
    /// </summary>
    /// <param name="optionId">Id of the config option.</param>
    /// <param name="entityId">Id of the entity to reset value for.</param>
    /// <throws>InvalidOperationException if the option does not have a default value.</throws>
    Task Reset(int optionId, ulong entityId);

    /// <summary>
    ///     Reset the value for a config option for the given Entity to its default value.
    /// </summary>
    /// <param name="option">Name of the config option.</param>
    /// <param name="entityId">Id of the entity to reset value for.</param>
    /// <throws>InvalidOperationException if the option does not have a default value.</throws>
    Task Reset(string option, ulong entityId);

    /// <summary>
    ///     Reset the value for a config option for the given Entity to its default value.
    /// </summary>
    /// <param name="option">The config option.</param>
    /// <param name="entityId">Id of the entity to reset value for.</param>
    /// <throws>InvalidOperationException if the option does not have a default value.</throws>
    Task Reset(ConfigOption option, ulong entityId);

    /// <summary>
    ///     Deletes value for a config option for the given Entity.
    /// </summary>
    /// <param name="optionId">Id of the config option.</param>
    /// <param name="entityId">Id of the entity to delete value for.</param>
    /// <throws>InvalidOperationException if the option is not nullable.</throws>
    Task Delete(int optionId, ulong entityId);

    /// <summary>
    ///     Deletes value for a config option for the given Entity.
    /// </summary>
    /// <param name="option">Name of the config option.</param>
    /// <param name="entityId">Id of the entity to delete value for.</param>
    /// <throws>InvalidOperationException if the option is not nullable.</throws>
    Task Delete(string option, ulong entityId);

    /// <summary>
    ///     Deletes value for a config option for the given Entity.
    /// </summary>
    /// <param name="option">The config option.</param>
    /// <param name="entityId">Id of the entity to delete value for.</param>
    /// <throws>InvalidOperationException if the option is not nullable.</throws>
    Task Delete(ConfigOption option, ulong entityId);
}