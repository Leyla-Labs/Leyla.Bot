using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Moderation;

namespace Main.Modules;

public class Moderation : ApplicationCommandModule
{
    [SlashCommand("verify", "Verifies a user")]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    public async Task SlashVerify(InteractionContext ctx, [Option("User", "User to verify")] DiscordUser user)
    {
        await Verify.RunSlash(ctx, user);
    }
}