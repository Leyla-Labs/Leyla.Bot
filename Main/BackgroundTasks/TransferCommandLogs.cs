using Common.Db;
using Common.Db.Models;
using Common.Helper;
using DNTCommon.Web.Core;

namespace Main.BackgroundTasks;

public class TransferCommandLogs : IScheduledTask
{
    public bool IsShuttingDown { get; set; }

    public async Task RunAsync()
    {
        if (IsShuttingDown)
        {
            return;
        }

        var logs = CommandLogHelper.Instance.GetAndClear();

        foreach (var guildLog in logs.GroupBy(x => x.GuildId))
        {
            await MemberHelper.CreateIfNotExist(guildLog.Key, guildLog.Select(x => x.UserId));
        }

        var dbLogs = logs.Select(x => new CommandLog
        {
            Command = x.Command,
            RunAt = x.RunAt,
            UserId = x.UserId,
            GuildId = x.GuildId
        });

        await using var context = new DatabaseContext();

        await context.CommandLogs.AddRangeAsync(dbLogs);
        await context.SaveChangesAsync();
    }
}