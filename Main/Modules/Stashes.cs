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
        // TODO make name optional, show menu with list of stashes if not provided
        await new ListStash(ctx, title).RunAsync();
    }

    [SlashCommand("pick", "Pick a random entry.")]
    public async Task SlashPick(InteractionContext ctx,
        [Option("Name", "Name of the stash pick from. Picks from all stashes if not provided.")]
        string? title = null)
    {
        await new Pick(ctx, title).RunAsync();
    }

    [SlashCommand("show", "Shows a specific stash entry.")]
    public async Task SlashShowStashEntry(InteractionContext ctx,
        [Option("Name", "Name of the stash to show entry from.")]
        string title,
        [Option("n", "Number of the entry to show. You can find this using /stash list.")]
        long n)
    {
        await new ShowStashEntry(ctx, title, n).RunAsync();
    }
}