using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Stashes;

namespace Main.Modules;

[SlashCommandGroup("Stash", "Description TODO")]
[SlashRequireGuild]
public class Stashes : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Add to Stash")]
    public async Task MenuAddToStash(ContextMenuContext ctx)
    {
        await new AddToStash(ctx).RunAsync();
    }

    [SlashCommand("create", "Create a new stash.")]
    public async Task SlashCreateStash(InteractionContext ctx,
        [Option("Name", "Name of the stash to create")]
        string title)
    {
        await new CreateStash(ctx, title).RunAsync();
    }

    [SlashCommand("list", "List all entries in stash.")]
    public async Task SlashListStash(InteractionContext ctx,
        [Option("Name", "Name of the stash list entries of")]
        string title)
    {
        await new ListStash(ctx, title).RunAsync();
    }
}