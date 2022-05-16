using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Common.Extensions;
using Db.Helper;
using DSharpPlus.Entities;

namespace Spam.Helper;

public class SpamHelper
{
    private readonly Dictionary<ulong, Dictionary<ulong, decimal>> _pressures;

    private SpamHelper()
    {
        _pressures = new Dictionary<ulong, Dictionary<ulong, decimal>>();
    }

    public async Task ProcessMessage(DiscordMessage message)
    {
        var guildId = message.Channel.GuildId ?? throw new ArgumentNullException(nameof(message.Channel.GuildId));

        foreach (var type in (PressureType[]) Enum.GetValues(typeof(PressureType)))
        {
            if (type == PressureType.Repeat)
            {
                // TODO handle this as well
                continue;
            }

            await IncreasePressure(type, message, guildId);
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
            PressureType.Repeat => throw new NotImplementedException(nameof(PressureType.Repeat)),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        if (addValue > 0m)
        {
            AddPressure(guildId, message.Author.Id, addValue);
        }
    }

    private void AddPressure(ulong guildId, ulong userId, decimal value)
    {
        if (_pressures.TryGetValue(guildId, out var guildDict))
        {
            if (guildDict.Any(x => x.Key == userId))
            {
                guildDict[userId] += value;
            }
            else
            {
                guildDict.Add(userId, value);
            }
        }
        else
        {
            _pressures.Add(guildId, new Dictionary<ulong, decimal>());
            var dict = _pressures[guildId];
            dict.Add(userId, value);
        }
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