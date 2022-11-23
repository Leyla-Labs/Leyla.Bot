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

    #region Instance methods

    private DiscordEmbed GetEmbed(SelfAssignMenu menu)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(menu.Title);
        if (!string.IsNullOrWhiteSpace(menu.Description))
        {
            embed.WithDescription(menu.Description);
        }

        if (menu.SelfAssignMenuDiscordEntityAssignments.Count > 0)
        {
            var description = $"{embed.Description}{Environment.NewLine}{Environment.NewLine}" +
                              "This following list will be overwritten by selecting new roles.";

            embed.WithDescription(description);

            var roleNames = Ctx.Guild.Roles
                .Where(x => menu.SelfAssignMenuDiscordEntityAssignments.Select(y => y.DiscordEntityId).Contains(x.Key))
                .Select(x => x.Value.Mention);
            var rolesStr = string.Join(Environment.NewLine, roleNames);
            embed.AddField("Roles", rolesStr);
        }

        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    private async Task<SelfAssignMenu?> GetSelfAssignMenu()
    {
        return await DbCtx.SelfAssignMenus.Where(x =>
                x.GuildId == Ctx.Guild.Id &&
                x.Title.Equals(_title))
            .Include(x => x.SelfAssignMenuDiscordEntityAssignments)
            .FirstOrDefaultAsync();
    }

    private DiscordRoleSelectComponent GetSelectMenu(SelfAssignMenu menu)
    {
        var roleCount = Ctx.Guild.Roles.Count;
        var maxOptions = roleCount > 25 ? 25 : roleCount;
        var customId = ModalHelper.GetModalName(Ctx.User.Id, "manageMenu", new[] {menu.Id.ToString()});
        return new DiscordRoleSelectComponent(customId, "Select roles", minOptions: 2, maxOptions: maxOptions);
    }

    #endregion
}