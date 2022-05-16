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
    public async Task MenuAddUserLog(ContextMenuContext ctx)
    {
        await new AddUserLog(ctx).RunAsync();
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Show logs")]
    [SlashRequireGuild]
    public async Task SlashListUserLogs(ContextMenuContext ctx)
    {
        await new ListUserLogs(ctx).RunAsync();
    }

    [SlashCommand("edit", "Edits a log.")]
    [SlashRequireGuild]
    public async Task SlashEditUserLog(InteractionContext ctx,
        [Option("user", "User to edit log of")]
        DiscordUser user,
        [Option("n", "Number of log to edit")] long n)
    {
        await new EditUserLog(ctx, (DiscordMember) user, n).RunAsync();
    }
    
    [SlashCommand("delete", "Deletes a log. This is irreversible!")]
    [SlashRequireGuild]
    public async Task SlashDeleteUserLog(InteractionContext ctx,
        [Option("user", "User to delete log of")]
        DiscordUser user,
        [Option("n", "Number of log to delete. This is irreversible!")] long n)
    {
        await new DeleteUserLog(ctx, (DiscordMember) user, n).RunAsync();
    }
}