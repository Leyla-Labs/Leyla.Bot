using Common.Classes;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Ffxiv;

namespace Main.Modules;

[SlashCommandGroup("FFXIV", "Description TODO")]
[SlashRequireGuild]
internal sealed class Ffxiv : ApplicationCommandLogModule
{
    [SlashCommandGroup("Character", "Description TODO")]
    public class Character : ApplicationCommandLogModule
    {
        [SlashCommand("find", "Shows character sheet.")]
        [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task SlashFind(InteractionContext ctx,
            [Option("Name", "Title of character to search for")]
            string name,
            [Option("HomeWorld", "Home world of the character")]
            string? server = null)
        {
            await new Find(ctx, name, server).RunAsync();
        }
    }
}