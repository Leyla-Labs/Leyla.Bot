using Common.Enums;
using DSharpPlus.Entities;

namespace Main.Extensions;

internal static class DiscordGuildExtensions
{
    public static async Task<List<LeylaModule>> GetGuildModules(this DiscordGuild guild)
    {
        // tf are these variable names

        var modules = new List<LeylaModule>();

        var modulesStr = Environment.GetEnvironmentVariable("MODULES") ?? throw new NullReferenceException();
        modulesStr = modulesStr.ToUpperInvariant();
        var modulesArray = modulesStr.Split(";");

        foreach (var moduleStr in modulesArray)
        {
            try
            {
                var id = Environment.GetEnvironmentVariable($"ID_{moduleStr}");
                LeylaModule? module = moduleStr switch
                {
                    "MAIN" when ulong.TryParse(id, out var result) && await guild.GetMemberAsync(result) != null =>
                        LeylaModule.Main,
                    "LOGS" when ulong.TryParse(id, out var result) && await guild.GetMemberAsync(result) != null =>
                        LeylaModule.Logs,
                    "SPAM" when ulong.TryParse(id, out var result) && await guild.GetMemberAsync(result) != null =>
                        LeylaModule.Spam,
                    _ => null
                };
                if (module != null)
                {
                    modules.Add(module.Value);
                }
            }
            catch
            {
                // do nothing
            }
        }

        return modules;
    }
}