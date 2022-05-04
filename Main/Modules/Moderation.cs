using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Moderation;

namespace Main.Modules;

public class Moderation : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.UserContextMenu, "Verify")]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    public async Task MenuVerify(ContextMenuContext ctx)
    {
        await Verify.RunMenu(ctx);
    }
}