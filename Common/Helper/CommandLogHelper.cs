using Common.Classes;
using Common.Db;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Common.Helper;

public class CommandLogHelper
{
    private readonly List<CommandLogLocal> _commandLogs = new();

    private CommandLogHelper()
    {
    }

    public void AddLog(InteractionContext ctx)
    {
        _commandLogs.Add(new CommandLogLocal(ctx.Guild.Id, ctx.User.Id, ctx.CommandName, DateTime.Now));
    }

    public List<CommandLogLocal> GetAndClear()
    {
        var list = _commandLogs.ToList();
        _commandLogs.Clear();
        return list;
    }

    public async Task<List<CommandLogLocal>> GetRecent(ulong guildId, int n)
    {
        return await GetRecent(guildId, null, n);
    }

    public async Task<List<CommandLogLocal>> GetRecent(DiscordMember member)
    {
        return await GetRecent(member.Guild.Id, member.Id, 10);
    }

    private async Task<List<CommandLogLocal>> GetRecent(ulong guildId, ulong? userId, int n)
    {
        var recentLocalEnumerable = _commandLogs.Where(x =>
            x.GuildId == guildId);

        if (userId != null)
        {
            recentLocalEnumerable = recentLocalEnumerable.Where(x => x.UserId == userId);
        }

        var recentLocal = recentLocalEnumerable.OrderByDescending(x => x.RunAt).Take(n).ToList();

        await using var context = new DatabaseContext();

        var recentDbEnumerable = context.CommandLogs.Where(x =>
            x.GuildId == guildId);

        if (userId != null)
        {
            recentDbEnumerable = recentDbEnumerable.Where(x => x.UserId == userId);
        }

        var recentDb = await recentDbEnumerable.OrderByDescending(x => x.RunAt)
            .Take(n - recentLocal.Count)
            .Select(x => new CommandLogLocal(x))
            .ToListAsync();

        recentLocal.AddRange(recentDb);
        return recentLocal;
    }

    public static async Task<DiscordEmbed> GetEmbed(DiscordGuild guild, IReadOnlyCollection<CommandLogLocal> logs,
        bool nameInEntries)
    {
        var members = new List<DiscordMember>();
        foreach (var userId in logs.Select(x => x.UserId).Distinct())
        {
            members.Add(await guild.GetMemberAsync(userId));
        }

        var embed = new DiscordEmbedBuilder();

        var member = !nameInEntries ? members.FirstOrDefault() : null;
        var titleAppend = member != null ? $" for {member.DisplayName}" : string.Empty;
        embed.WithTitle($"{logs.Count} Recent Command Logs{titleAppend}");
        embed.WithDescription(string.Join(Environment.NewLine,
            logs.Select(x => GetCommandLogStr(members, x, nameInEntries))));
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    private static string GetCommandLogStr(IEnumerable<DiscordMember> members, CommandLogBase log, bool nameInEntries)
    {
        var member = members.FirstOrDefault(x => x.Id == log.UserId);
        var runAt = Formatter.Timestamp(log.RunAt, TimestampFormat.ShortDateTime);
        var memberStr = member != null ? member.Mention : log.UserId.ToString();
        var memberStrFull = nameInEntries ? $"• {memberStr} " : string.Empty;
        return $"{runAt} {memberStrFull}• {log.Command}";
    }

    #region Singleton

    private static readonly Lazy<CommandLogHelper> Lazy = new(() => new CommandLogHelper());
    public static CommandLogHelper Instance => Lazy.Value;

    #endregion
}