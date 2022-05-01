using Db.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;

namespace Main.Commands.Moderation;

public static class Verify
{
    public static async Task RunMenu(ContextMenuContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        if (ctx.TargetMember == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Member not found."));
            return;
        }

        var role = await ConfigHelper.GetRole("Verification Role", ctx.Guild);
        if (role == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Verification role not found."));
            return;
        }

        if (ctx.TargetMember?.Roles.Contains(role) == true)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                $"{ctx.TargetMember.Nickname ?? ctx.TargetMember.Username} is already verified."));
            return;
        }

        var embed = await GrantRoleAndGetEmbed(ctx.TargetMember!, ctx.Member, role);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }
    
    public static async Task RunSlash(InteractionContext ctx, DiscordUser user)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var member = user as DiscordMember;

        if (member == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Member not found."));
            return;
        }

        var role = await ConfigHelper.GetRole("Verification Role", ctx.Guild);
        if (role == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Verification role not found."));
            return;
        }

        if (member.Roles.Contains(role))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                $"{member.Nickname ?? member.Username} is already verified."));
            return;
        }

        var embed = await GrantRoleAndGetEmbed(member, ctx.Member, role);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    private static async Task<DiscordEmbed> GrantRoleAndGetEmbed(DiscordMember targetMember, DiscordUser actingUser,
        DiscordRole role)
    {
        try
        {
            await targetMember.GrantRoleAsync(role,
                $"Verification by {actingUser.Username}#{actingUser.Discriminator}")!;
        }
        catch (Exception e)
        {
            if (e is not UnauthorizedException) throw;

            // TODO check if this can be replaced with global exception handler
            var description =
                $"Granting the role {role.Name} to {targetMember.Nickname ?? targetMember.Username} failed. " +
                "Please check the role hierarchy and make sure the verification role is not above the bot role.";
            return new DiscordEmbedBuilder
            {
                Title = "Granting role failed",
                Description = description
            }.WithColor(DiscordColor.IndianRed).Build();
        }


        var embed = new DiscordEmbedBuilder
        {
            Title = "Now Verified:",
            Description = $"{targetMember.Mention}{Environment.NewLine}{targetMember.Nickname ?? targetMember.Username}"
        };

        embed.WithThumbnail(targetMember.AvatarUrl);
        embed.WithColor(role.Color);
        return embed.Build();
    }
}