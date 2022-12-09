using Common.Classes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.CommandLogs;

namespace Main.Modules;

[SlashCommandGroup("CommandLogs", "Description TODO")]
internal sealed class CommandLogs : ApplicationCommandLogModule
{
    [SlashCommand("recent", "Shows recent user logs.")]
    [SlashRequireGuild]
    public async Task SlashRecentAsync(InteractionContext ctx,
        [Option("n", "Number of entries to show (default 10, max 100)")]
        long n = 10)
    {
        await new Recent(ctx, n).RunAsync();
    }

    [SlashCommand("user", "Shows 10 most recent logs for given user.")]
    [SlashRequireGuild]
    public async Task SlashUserAsync(InteractionContext ctx,
        [Option("user", "User to show entries of.")]
        DiscordUser user)
    {
        await new User(ctx, (DiscordMember) user).RunAsync();
    }
}