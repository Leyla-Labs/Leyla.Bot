using Common.Classes;
using Common.Extensions;
using Db;
using Db.Enums;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.UserLogs;

public class ListUserLogs : ContextMenuCommand
{
    public ListUserLogs(ContextMenuContext ctx) : base(ctx)
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
            await AddLogFields(embedBuilder, userLogs, UserLogType.Warning);
            await AddLogFields(embedBuilder, userLogs, UserLogType.Silence);
            await AddLogFields(embedBuilder, userLogs, UserLogType.Ban);
        }

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embedBuilder.Build()).AsEphemeral());
    }

    private async Task<List<UserLog>> GetUserLogs()
    {
        await using var context = new DatabaseContext();
        return await context.UserLogs.Where(x => x.MemberId == Ctx.TargetMember.Id).ToListAsync();
    }

    private DiscordEmbedBuilder GetEmbedBuilder()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Logs for {Ctx.TargetMember.DisplayName}");
        embed.WithColor(DiscordColor.Blurple);
        return embed;
    }

    private async Task AddLogFields(DiscordEmbedBuilder embed, ICollection<UserLog> userLogs, UserLogType type)
    {
        if (userLogs.All(x => x.Type != type))
        {
            return;
        }
        
        var logs = new List<string>();
        foreach (var userLog in userLogs.Where(x => x.Type == type))
        {
            logs.Add(await GetUserLogString(userLog));
        }

        var logStr = string.Join($"{Environment.NewLine}", logs);
        embed.AddField(type.ToString(), logStr);
    }

    private async Task<string> GetUserLogString(UserLog log)
    {
        var dateStr = Formatter.Timestamp(log.Date, TimestampFormat.ShortDateTime);
        var author = await Ctx.GetMember(log.AuthorId);
        var authorName = author?.DisplayName ?? log.AuthorId.ToString();
        return $"{dateStr} • {authorName}{Environment.NewLine}{log.Text}";
    }
}