using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.SelfAssignMenus;

namespace Main.Modules;

[SlashCommandGroup("Menu", "Description TODO")]
[SlashRequireGuild]
public class SelfAssignMenus : ApplicationCommandModule
{
    [SlashCommand("create", "Create a new self assign menu.")]
    public async Task SlashCreate(InteractionContext ctx,
        [Option("Name", "Name of the self assign menu to create")]
        string title,
        [Option("Description", "Optional description of the menu")]
        string? description = null)
    {
        await new Create(ctx, title, description).RunAsync();
    }

    [SlashCommand("list", "List all self assign menus.")]
    public async Task SlashList(InteractionContext ctx)
    {
        await new List(ctx).RunAsync();
    }

    [SlashCommand("rename", "Renames a self assign menu.")]
    public async Task SlashRename(InteractionContext ctx,
        [Option("Name", "Name of the self assign menu to create")]
        string title)
    {
        await new Rename(ctx, title).RunAsync();
    }

    [SlashCommand("manage", "Manages a self assign menu.")]
    public async Task SlashManage(InteractionContext ctx,
        [Option("Name", "Name of the self assign menu to manage")]
        string title)
    {
        await new Manage(ctx, title).RunAsync();
    }
}