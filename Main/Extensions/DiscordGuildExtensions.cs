using Common;
using Common.Enums;
using DSharpPlus.Entities;

namespace Main.Extensions;

internal static class DiscordGuildExtensions
{
    public static async Task<List<LeylaModule>> GetGuildModules(this DiscordGuild guild)
    {
        var modules = new List<LeylaModule>();

        try
        {
            if (await guild.GetMemberAsync(BotIds.Main) != null)
            {
                modules.Add(LeylaModule.Main);
            }
        }
        catch
        {
            // do nothing
        }

        try
        {
            if (await guild.GetMemberAsync(BotIds.Logs) != null)
            {
                modules.Add(LeylaModule.Logs);
            }
        }
        catch
        {
            // do nothing
        }

        try
        {
            if (await guild.GetMemberAsync(BotIds.Spam) != null)
            {
                modules.Add(LeylaModule.Spam);
            }
        }
        catch
        {
            // do nothing
        }

        return modules;
    }
}