using Common.Classes;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.CommandLogs;

namespace Main.Modules;

[SlashCommandGroup("CommandLogs", "Description TODO")]
internal sealed class CommandLogs : ApplicationCommandLogModule
{
    [SlashCommand("recent", "Shows recent user logs.")]
    [SlashRequireGuild]
    public async Task SlashRecent(InteractionContext ctx,
        [Option("n", "Number of entries to show (default 10, max 100)")]
        long n = 10)
    {
        await new Recent(ctx, n).RunAsync();
    }
}