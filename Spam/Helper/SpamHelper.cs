using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;
using Spam.Enums;

namespace Spam.Helper;

internal delegate void MaxPressureExceededHandler(DiscordClient sender, MaxPressureExceededEventArgs args);

internal class SpamHelper
{
    private readonly Dictionary<ulong, Dictionary<ulong, UserPressure>> _pressures = new();

    private SpamHelper()
    {
    }

    public static event MaxPressureExceededHandler? MaxPressureExceeded;

    public async Task ProcessMessage(DiscordClient sender, DiscordMessage message)
    {
        var guildId = message.Channel.GuildId ?? throw new ArgumentNullException(nameof(message.Channel.GuildId));

        foreach (var type in (PressureType[]) Enum.GetValues(typeof(PressureType)))
        {
            await IncreasePressure(type, message, guildId);
        }

        var pressure = _pressures[guildId][message.Author.Id];
        pressure.PressureSessionMessages.Add(message);

        var maxPressure = await ConfigHelper.Instance.GetDecimal(Common.Strings.Spam.MaxPressure, guildId) ??
                          throw new NullReferenceException(Common.Strings.Spam.MaxPressure);

        if (pressure.CurrentPressure > maxPressure)
        {
            MaxPressureExceeded?.Invoke(sender, new MaxPressureExceededEventArgs(pressure, maxPressure));
            pressure.ResetPressure();
        }
    }

    private async Task IncreasePressure(PressureType type, DiscordMessage message, ulong guildId)
    {
        var n = await GetPressureConfig(type, guildId);

        var addValue = type switch
        {
            PressureType.Base => n,
            PressureType.Image => decimal.Add(decimal.Multiply(n, message.Attachments.Count),
                decimal.Multiply(n, message.Embeds.Count)), // attachments * n + embeds * n
            PressureType.Length => decimal.Multiply(n, message.Content.Length),
            PressureType.Line => decimal.Multiply(n,
                Regex.Matches(message.Content, "$", RegexOptions.Multiline).Count - 1),
            PressureType.Ping => decimal.Multiply(n, message.MentionedUsers.Count),
            PressureType.Repeat => IsRepeatedMessage(guildId, message) ? n : 0,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        if (addValue > 0m)
        {
            await AddPressure(guildId, message.Author.Id, addValue);
        }
    }

    private async Task AddPressure(ulong guildId, ulong userId, decimal value)
    {
        if (_pressures.TryGetValue(guildId, out var guildDict))
        {
            if (guildDict.Any(x => x.Key == userId))
            {
                var pressureDecay = await ConfigHelper.Instance.GetDecimal(Common.Strings.Spam.PressureDecay, guildId);
                if (pressureDecay == null)
                {
                    throw new NullReferenceException(nameof(pressureDecay));
                }

                guildDict[userId].IncreasePressure(value, pressureDecay.Value);
            }
            else
            {
                guildDict.Add(userId, new UserPressure(value));
            }
        }
        else
        {
            _pressures.Add(guildId, new Dictionary<ulong, UserPressure>());
            var dict = _pressures[guildId];
            dict.Add(userId, new UserPressure(value));
        }
    }

    private bool IsRepeatedMessage(ulong guildId, DiscordMessage message)
    {
        if (!_pressures.TryGetValue(guildId, out var guildDict))
        {
            return false;
        }

        if (guildDict.All(x => x.Key != message.Author.Id))
        {
            return false;
        }

        // member exists, check value
        var isRepeated = guildDict.First(x =>
            x.Key == message.Author.Id).Value.LastMessage.Equals(message.Content.ToLowerInvariant());
        guildDict[message.Author.Id].LastMessage = message.Content;
        return isRepeated;
    }

    private static async Task<decimal> GetPressureConfig(PressureType type, ulong guildId)
    {
        var configOptionName = type.GetAttribute<DisplayAttribute>();
        if (configOptionName?.Name == null)
        {
            throw new NullReferenceException(nameof(configOptionName));
        }

        var config = await ConfigHelper.Instance.GetDecimal(configOptionName.Name, guildId);

        if (config == null)
        {
            throw new NullReferenceException(nameof(config));
        }

        return config.Value;
    }

    #region Singleton

    private static readonly Lazy<SpamHelper> Lazy = new(() => new SpamHelper());
    public static SpamHelper Instance => Lazy.Value;

    #endregion
}