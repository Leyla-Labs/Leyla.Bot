using DSharpPlus.SlashCommands;
using Main.Commands.SelfAssignMenus;

namespace Main.Modules;

[SlashCommandGroup("Menu", "Description TODO")]
public class SelfAssignMenus : ApplicationCommandModule
{
    [SlashCommand("create", "Create a new self assign menu.")]
    public async Task SlashCreateSelfAssignMenu(InteractionContext ctx,
        [Option("Name", "Name of the self assign menu to create")]
        string title,
        [Option("Description", "Optional description of the menu")]
        string? description = null)
    {
        await new Create(ctx, title, description).RunAsync();
    }
}