using DSharpPlus.SlashCommands;
using Main.Commands.Configuration;

namespace Main.Modules;

internal sealed class Configuration : ApplicationCommandModule
{
    [SlashCommand("configure", "Configure bot settings.")]
    public async Task SlashConfigure(InteractionContext ctx)
    {
        await new Configure(ctx).RunAsync();
    }
}