using Common.Checks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Moderation;

namespace Main.Modules;

public class Moderation : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.UserContextMenu, "Verify")]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    [SlashRequireGuild]
    [MenuRequireTargetMember]
    public async Task MenuVerify(ContextMenuContext ctx)
    {
        await new Verify(ctx).RunAsync();
    }

    [SlashCommand("silence", "Silences a member.")]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    [SlashRequireGuild]
    public async Task SlashSilence(InteractionContext ctx,
        [Option("member", "Member to silence")]
        DiscordUser user)
    {
        await new Silence(ctx, (DiscordMember) user).RunAsync();
    }

    [SlashCommand("unsilence", "Unsilences a member.")]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    [SlashRequireGuild]
    public async Task SlashUnsilence(InteractionContext ctx,
        [Option("member", "Member to unsilence")]
        DiscordUser user)
    {
        await new Unsilence(ctx, (DiscordMember) user).RunAsync();
    }
}