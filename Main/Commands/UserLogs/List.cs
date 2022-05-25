using System.Text;
using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Enums;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.UserLogs;

internal sealed class List : ContextMenuCommand
{
    public List(ContextMenuContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var embedBuilder = GetEmbedBuilder();

        var userLogs = await GetUserLogs();
        if (userLogs.Count == 0)
        {
            embedBuilder.WithDescription("No logs");
        }
        else
        {
            var sb = new StringBuilder();
            await AddLogFields(sb, userLogs, UserLogType.Warning);
            await AddLogFields(sb, userLogs, UserLogType.Silence);
            await AddLogFields(sb, userLogs, UserLogType.Ban);
            embedBuilder.WithDescription(sb.ToString());
        }

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()).AsEphemeral());
    }

    private async Task<List<(int, UserLog)>> GetUserLogs()
    {
        await using var context = new DatabaseContext();

        var userLogs = await context.UserLogs.Where(x =>
                x.Member.GuildId == Ctx.Guild.Id &&
                x.MemberId == Ctx.TargetUser.Id)
            .ToListAsync();

        return userLogs.Select((x, i) => new {userLog = x, index = i}).Select(x => (x.index, x.userLog)).ToList();
    }

    private DiscordEmbedBuilder GetEmbedBuilder()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Logs for {Ctx.TargetMember?.DisplayName ?? Ctx.TargetUser.Username}");
        embed.WithColor(DiscordColor.Blurple);
        return embed;
    }

    private async Task AddLogFields(StringBuilder sb, ICollection<(int, UserLog)> userLogs, UserLogType type)
    {
        if (userLogs.All(x => x.Item2.Type != type))
        {
            return;
        }

        var logs = new List<string>();
        foreach (var userLog in userLogs.Where(x => x.Item2.Type == type))
        {
            logs.Add(await GetUserLogString(userLog));
        }

        sb.Append($"**__{type.ToString()}__** ({logs.Count}){Environment.NewLine}{Environment.NewLine}");
        var logStr = string.Join($"{Environment.NewLine}{Environment.NewLine}", logs);
        logStr = $"{logStr}{Environment.NewLine}{Environment.NewLine}";
        sb.Append(logStr);
    }

    private async Task<string> GetUserLogString((int, UserLog) log)
    {
        var dateStr = Formatter.Timestamp(log.Item2.Date, TimestampFormat.ShortDateTime);
        var author = await Ctx.GetMember(log.Item2.AuthorId);
        var authorName = author?.DisplayName ?? log.Item2.AuthorId.ToString();
        var n = Environment.NewLine;

        var additionalDetails = !string.IsNullOrWhiteSpace(log.Item2.AdditionalDetails)
            ? $"{log.Item2.AdditionalDetails}{n}"
            : string.Empty;

        return $"**{log.Item2.Reason}**{n}{additionalDetails}{log.Item1 + 1} • {dateStr} • {authorName}";
    }
}