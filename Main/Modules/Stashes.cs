using DSharpPlus;
using DSharpPlus.SlashCommands;
using Main.Commands.Stashes;

namespace Main.Modules;

public class Stashes : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Add to Stash")]
    public async Task MenuAddToStash(ContextMenuContext ctx)
    {
        await new AddToStash(ctx).RunAsync();
    }
}