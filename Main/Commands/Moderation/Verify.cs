using Common.Classes;
using Common.Extensions;
using Common.Helper;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;

namespace Main.Commands.Moderation;

internal sealed class Verify : ContextMenuCommand
{
    public Verify(ContextMenuContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var role = await ConfigHelper.Instance.GetRole("Verification Role", Ctx.Guild);
        if (role == null)
        {
            await Ctx.CreateResponseAsync(
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Verification role not found"));
            return;
        }

        if (Ctx.TargetMember.Roles.Contains(role))
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed(
                $"{Ctx.TargetMember.Nickname ?? Ctx.TargetMember.Username} is already verified."));
            return;
        }

        var embed = await GrantRoleAndGetEmbed(role);
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }

    #region Instance methods

    private async Task<DiscordEmbed> GrantRoleAndGetEmbed(DiscordRole role)
    {
        try
        {
            await Ctx.TargetMember.GrantRoleAsync(role,
                $"Verification by {Ctx.Member.Username}#{Ctx.Member.Discriminator}")!;
        }
        catch (Exception e)
        {
            if (e is not UnauthorizedException)
            {
                throw;
            }

            // TODO check if this can be replaced with global exception handler
            var description =
                $"Granting the role {role.Name} to {Ctx.TargetMember.Nickname ?? Ctx.TargetMember.Username} failed. " +
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
            Description =
                $"{Ctx.TargetMember.Mention}{Environment.NewLine}{Ctx.TargetMember.Nickname ?? Ctx.TargetMember.Username}"
        };

        embed.WithThumbnail(Ctx.TargetMember.AvatarUrl);
        embed.WithColor(role.Color);
        return embed.Build();
    }

    #endregion
}