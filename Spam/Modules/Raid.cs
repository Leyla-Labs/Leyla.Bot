using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Spam.Commands.Raid;

namespace Spam.Modules;

[SlashCommandGroup("Raid", "todo")]
public class Raid : ApplicationCommandModule
{
    [SlashCommand("on", "Turns raid mode on.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashRaidOn(InteractionContext ctx)
    {
        await new On(ctx).RunAsync();
    }

    [SlashCommand("off", "Turns raid mode off.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashRaidOff(InteractionContext ctx)
    {
        await new Off(ctx).RunAsync();
    }
}