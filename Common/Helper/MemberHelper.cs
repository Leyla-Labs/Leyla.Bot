using Common.Db;
using Common.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Helper;

public static class MemberHelper
{
    public static async Task CreateIfNotExistAsync(ulong guildId, ulong userId)
    {
        await GuildHelper.CreateIfNotExistAsync(guildId);

        await using var context = new DatabaseContext();
        if (!await context.Members.AnyAsync(x => x.UserId == userId && x.GuildId == guildId))
        {
            await context.Members.AddAsync(new Member
            {
                UserId = userId,
                GuildId = guildId
            });
            await context.SaveChangesAsync();
        }
    }

    public static async Task CreateIfNotExistAsync(ulong guildId, IEnumerable<ulong> userIds)
    {
        await GuildHelper.CreateIfNotExistAsync(guildId);

        await using var context = new DatabaseContext();

        var guildMembers = context.Members.Where(x => x.GuildId == guildId);
        var newUserIds = userIds.Where(x => !guildMembers.Select(y => y.UserId).Contains(x)).Distinct();
        var newMembers = newUserIds.Select(x => new Member {UserId = x, GuildId = guildId}).ToList();

        await context.Members.AddRangeAsync(newMembers);

        await context.SaveChangesAsync();
    }
}