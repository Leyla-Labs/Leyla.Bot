using Common.Db;
using Common.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Helper;

public static class GuildHelper
{
    public static async Task CreateIfNotExistAsync(ulong guildId)
    {
        await using var context = new DatabaseContext();
        if (!await context.Guilds.AnyAsync(x => x.Id == guildId))
        {
            await context.Guilds.AddAsync(new Guild
            {
                Id = guildId
            });
            await context.SaveChangesAsync();
        }
    }
}