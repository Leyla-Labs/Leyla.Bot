using Common.Extensions;
using Db.Helper;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;

namespace Main.Commands.Moderation;

public static class Verify
{
    public static async Task RunMenu(ContextMenuContext ctx)
    {
        var role = await ConfigHelper.Instance.GetRole("Verification Role", ctx.Guild);
        if (role == null)
        {
            await ctx.CreateResponseAsync(
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Verification role not found"));
            return;
        }

        if (ctx.TargetMember.Roles.Contains(role))
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed(
                $"{ctx.TargetMember.Nickname ?? ctx.TargetMember.Username} is already verified."));
            return;
        }

        var embed = await GrantRoleAndGetEmbed(ctx.TargetMember!, ctx.Member, role);
        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed));
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
            if (e is not UnauthorizedException)
            {
                throw;
            }

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