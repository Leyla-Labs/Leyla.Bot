using DSharpPlus;
using DSharpPlus.SlashCommands;
using Main.Commands.Quotes;

namespace Main.Modules;

public class Quotes : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Add Quote")]
    public async Task MenuAddQuote(ContextMenuContext ctx)
    {
        await AddQuote.RunMenu(ctx);
    }
}