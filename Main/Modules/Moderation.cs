using Common.Checks;
using Common.Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Moderation;

namespace Main.Modules;

internal sealed class Moderation : ApplicationCommandLogModule
{
    [ContextMenu(ApplicationCommandType.UserContextMenu, "Verify")]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    [SlashRequireGuild]
    [MenuRequireTargetMember]
    public async Task MenuVerifyAsync(ContextMenuContext ctx)
    {
        await new Verify(ctx).RunAsync();
    }

    [SlashCommand("silence", "Silences a member.")]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    [SlashRequireGuild]
    public async Task SlashSilenceAsync(InteractionContext ctx,
        [Option("member", "Member to silence")]
        DiscordUser user)
    {
        await new Silence(ctx, (DiscordMember) user).RunAsync();
    }

    [SlashCommand("unsilence", "Unsilences a member.")]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    [SlashRequireGuild]
    public async Task SlashUnsilenceAsync(InteractionContext ctx,
        [Option("member", "Member to unsilence")]
        DiscordUser user)
    {
        await new Unsilence(ctx, (DiscordMember) user).RunAsync();
    }
}