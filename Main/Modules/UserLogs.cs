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
    public async Task SlashListQuotes(ContextMenuContext ctx)
    {
        await new ListUserLogs(ctx).RunAsync();
    }

    [SlashCommand("edit", "Edits a log.")]
    [SlashRequireGuild]
    public async Task SlashListQuotes(InteractionContext ctx,
        [Option("user", "User to edit log of")]
        DiscordUser user,
        [Option("n", "Number of log to edit")] long n)
    {
        await new EditUserLog(ctx, (DiscordMember) user, n).RunAsync();
    }
}