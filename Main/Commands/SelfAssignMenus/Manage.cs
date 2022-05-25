using Common.Classes;
using Common.Db.Models;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.SelfAssignMenus;

internal sealed class Manage : SlashCommand
{
    private readonly string _title;

    public Manage(InteractionContext ctx, string title) : base(ctx)
    {
        _title = title;
    }

    public override async Task RunAsync()
    {
        var menu = await GetSelfAssignMenu();

        if (menu == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Self assign menu not found.").AsEphemeral());
            return;
        }

        var selectMenu = GetSelectMenu(menu);
        var embed = GetEmbed(menu);

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AddComponents(selectMenu).AsEphemeral());
    }

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

    #endregion

    #region Instance methods

    private async Task<SelfAssignMenu?> GetSelfAssignMenu()
    {
        return await DbCtx.SelfAssignMenus.Where(x =>
                x.GuildId == Ctx.Guild.Id &&
                x.Title.Equals(_title))
            .Include(x => x.SelfAssignMenuDiscordEntityAssignments)
            .FirstOrDefaultAsync();
    }

    private DiscordSelectComponent GetSelectMenu(SelfAssignMenu menu)
    {
        var roles = Ctx.Guild.Roles.Select(x => x.Value);

        var options = roles.Select(x => new DiscordSelectComponentOption(x.Name, x.Id.ToString(),
                isDefault: menu.SelfAssignMenuDiscordEntityAssignments.Select(y => y.DiscordEntityId).Contains(x.Id)))
            .ToList();
        var customId = ModalHelper.GetModalName(Ctx.User.Id, "manageMenu", new[] {menu.Id.ToString()});
        return new DiscordSelectComponent(customId, "Select roles", options, minOptions: 2,
            maxOptions: options.Count);
    }

    #endregion
}