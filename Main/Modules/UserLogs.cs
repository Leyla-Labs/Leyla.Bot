using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.UserLogs;

namespace Main.Modules;

[SlashCommandGroup("UserLogs", "Description TODO")]
public class UserLogs : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.UserContextMenu, "Add User Log")]
    [SlashRequireGuild]
    public async Task MenuAdd(ContextMenuContext ctx)
    {
        await new Add(ctx).RunAsync();
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Show logs")]
    [SlashRequireGuild]
    public async Task SlashList(ContextMenuContext ctx)
    {
        await new List(ctx).RunAsync();
    }

    [SlashCommand("edit", "Edits a log.")]
    [SlashRequireGuild]
    public async Task SlashEdit(InteractionContext ctx,
        [Option("user", "User to edit log of")]
        DiscordUser user,
        [Option("n", "Number of log to edit")] long n)
    {
        await new Edit(ctx, (DiscordMember) user, n).RunAsync();
    }

    [SlashCommand("delete", "Deletes a log. This is irreversible!")]
    [SlashRequireGuild]
    public async Task SlashDelete(InteractionContext ctx,
        [Option("user", "User to delete log of")]
        DiscordUser user,
        [Option("n", "Number of log to delete. This is irreversible!")]
        long n)
    {
        await new Delete(ctx, (DiscordMember) user, n).RunAsync();
    }
}