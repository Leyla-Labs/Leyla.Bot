using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Common.Extensions;
using Db.Helper;
using DSharpPlus.Entities;
using Spam.Classes;
using Spam.Enums;

namespace Spam.Helper;

public class SpamHelper
{
    private readonly Dictionary<ulong, Dictionary<ulong, UserPressure>> _pressures = new();

    private SpamHelper()
    {
    }

    public async Task ProcessMessage(DiscordMessage message)
    {
        var guildId = message.Channel.GuildId ?? throw new ArgumentNullException(nameof(message.Channel.GuildId));

        foreach (var type in (PressureType[]) Enum.GetValues(typeof(PressureType)))
        {
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
            PressureType.Repeat => decimal.Multiply(n, GetRepeatCount(guildId, message)),
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
                guildDict[userId].PressureValue += value;
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

    private int GetRepeatCount(ulong guildId, DiscordMessage message)
    {
        if (_pressures.TryGetValue(guildId, out var guildDict))
        {
            // guild dict exists, search for member
            if (guildDict.Any(x => x.Key == message.Author.Id))
            {
                // member exists, check value
                var repeatCount = guildDict.First(x =>
                    x.Key == message.Author.Id).Value.RecentMessages.Count(x =>
                    x.Equals(message.Content.ToLower()));

                guildDict[message.Author.Id].AddMessage(message.Content);
                return repeatCount;
            }

            // member does not exist, create user entry
            guildDict.Add(message.Author.Id, new UserPressure(message.Content));
            return 0;
        }

        {
            // guild dict does not exist, create guild dict and user entry
            _pressures.Add(guildId, new Dictionary<ulong, UserPressure>());
            var dict = _pressures[guildId];
            dict.Add(message.Author.Id, new UserPressure(message.Content));
            return 0;
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