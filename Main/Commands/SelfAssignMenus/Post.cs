using Common.Classes;
using Common.Db.Models;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.SelfAssignMenus;

internal sealed class Post : SlashCommand
{
    private readonly DiscordChannel _channel;
    private readonly string _title;

    public Post(InteractionContext ctx, DiscordChannel channel, string title) : base(ctx)
    {
        _channel = channel;
        _title = title;
    }

    public override async Task RunAsync()
    {
        if (await GetSelfAssignMenuAsync() is not { } menu)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Self assign menu not found.").AsEphemeral());
            return;
        }

        // Send menu in target channel
        var embed = GetEmbed(menu);
        var button = GetButton(menu);
        await _channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(button));

        // Show confirmation to user
        var confirmationEmbed = GetConfirmationEmbed(menu);
        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(confirmationEmbed).AsEphemeral());
    }

    #region Instance methods

    private async Task<SelfAssignMenu?> GetSelfAssignMenuAsync()
    {
        return await DbCtx.SelfAssignMenus.FirstOrDefaultAsync(x =>
            x.GuildId == Ctx.Guild.Id && x.Title.Equals(_title));
    }

    private DiscordEmbed GetConfirmationEmbed(SelfAssignMenu menu)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Self Assign Menu Posted");
        embed.WithDescription($"The Self Assign Menu {menu.Title} was posted in {_channel.Mention}.");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion

    #region Static methods

    private static DiscordEmbed GetEmbed(SelfAssignMenu menu)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(menu.Title);
        if (!string.IsNullOrWhiteSpace(menu.Description))
        {
            embed.WithDescription(menu.Description);
        }

        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    private static DiscordButtonComponent GetButton(SelfAssignMenu menu)
    {
        var customId = ModalHelper.GetModalName(1, "selfAssignMenu", new[] {menu.Id.ToString()});
        return new DiscordButtonComponent(ButtonStyle.Primary, customId, "Show roles");
    }

    #endregion
}