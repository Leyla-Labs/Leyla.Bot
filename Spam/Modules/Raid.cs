using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Spam.Commands.Raid;

namespace Spam.Modules;

[SlashCommandGroup("Raid", "todo")]
internal sealed class Raid : ApplicationCommandModule
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

    [SlashCommand("list", "Lists all members from the most recent raid.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashRaidList(InteractionContext ctx)
    {
        await new List(ctx).RunAsync();
    }

    [SlashCommand("ban", "Bans all members from the most recent raid. USE WITH CAUTION!")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashRaidBan(InteractionContext ctx)
    {
        await new Ban(ctx).RunAsync();
    }
}