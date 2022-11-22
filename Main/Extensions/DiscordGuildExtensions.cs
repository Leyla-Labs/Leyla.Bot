using Common.Enums;
using Common.Helper;
using DSharpPlus.Entities;

namespace Main.Extensions;

internal static class DiscordGuildExtensions
{
    public static async Task<List<LeylaModule>> GetGuildModules(this DiscordGuild guild)
    {
        var modules = new List<LeylaModule>();

        foreach (var (module, userId) in LeylaModuleHelper.LeylaModules)
        {
            if (await guild.GetMemberAsync(userId) != null)
            {
                modules.Add(module);
            }
        }

        return modules;
    }
}