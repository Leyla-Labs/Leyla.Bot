using Common.Db;
using Common.Db.Models;
using Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Common.Helper;

public static class DiscordEntityHelper
{
    public static async Task CreateIfNotExist(DiscordEntityType type, ulong id, ulong guildId)
    {
        await GuildHelper.CreateIfNotExist(guildId);

        await using var context = new DatabaseContext();
        if (!await context.DiscordEntities.AnyAsync(x =>
                x.GuildId == guildId && x.DiscordEntityType == type && x.Id == id))
        {
            await context.DiscordEntities.AddAsync(new DiscordEntity
            {
                Id = id,
                GuildId = guildId,
                DiscordEntityType = type
            });
            await context.SaveChangesAsync();
        }
    }

    public static async Task DeleteIfExists(DiscordEntityType type, ulong id, ulong guildId)
    {
        await using var context = new DatabaseContext();

        var entry = await context.DiscordEntities.FirstOrDefaultAsync(x =>
            x.GuildId == guildId &&
            x.DiscordEntityType == type &&
            x.Id == id);

        if (entry != null)
        {
            context.Entry(entry).State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }
    }
}