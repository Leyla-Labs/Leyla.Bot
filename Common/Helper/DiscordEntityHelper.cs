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
}