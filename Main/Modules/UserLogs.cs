using DSharpPlus;
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
        // TODO check if SlashRequireGuild on command group level is enough
        await new ListUserLogs(ctx).RunAsync();
    }
}