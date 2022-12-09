using Common.Classes;
using DSharpPlus.SlashCommands;
using Main.Commands.Configuration;

namespace Main.Modules;

internal sealed class Configuration : ApplicationCommandLogModule
{
    [SlashCommand("configure", "Configure bot settings.")]
    public async Task SlashConfigureAsync(InteractionContext ctx)
    {
        await new Configure(ctx).RunAsync();
    }
}