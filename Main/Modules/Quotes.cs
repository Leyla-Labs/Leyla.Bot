using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Quotes;

namespace Main.Modules;

[SlashCommandGroup("Quote", "Description TODO")]
public class Quotes : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Add Quote")]
    public async Task MenuAddQuote(ContextMenuContext ctx)
    {
        await AddQuote.RunMenu(ctx);
    }

    [SlashCommand("random", "Shows a random quote.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashRandomQuote(InteractionContext ctx)
    {
        await RandomQuote.RunSlash(ctx);
    }
}