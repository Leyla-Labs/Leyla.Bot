using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;

namespace Main.Handler;

internal sealed class SelfAssignMenuButtonPressedHandler : InteractionHandler
{
    private readonly string _menuId;

    public SelfAssignMenuButtonPressedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e,
        string menuId) : base(sender, e)
    {
        _menuId = menuId;
    }

    public override async Task RunAsync()
    {
        if (await GetSelfAssignMenu() is not { } menu)
        {
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Self assign menu not found.").AsEphemeral());
            return;
        }

        var selectMenu = GetSelectMenu(menu);

        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddComponents(selectMenu).AsEphemeral());
    }

    private async Task<SelfAssignMenu?> GetSelfAssignMenu()
    {
        var id = Convert.ToInt32(_menuId);

        await using var context = new DatabaseContext();

        return await context.SelfAssignMenus.Where(x =>
                x.Id == id)
            .Include(x => x.SelfAssignMenuDiscordEntityAssignments)
            .FirstOrDefaultAsync();
    }

    private DiscordSelectComponent GetSelectMenu(SelfAssignMenu menu)
    {
        var member = (DiscordMember) EventArgs.User;

        var roles = EventArgs.Guild.Roles.Where(x =>
                menu.SelfAssignMenuDiscordEntityAssignments.Select(y =>
                        y.DiscordEntityId)
                    .Contains(x.Key))
            .Select(x => x.Value)
            .ToList();

        var options = roles.Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(),
                isDefault: member.Roles.Any(y => y.Id == x.Id))).ToList();

        var customId = ModalHelper.GetModalName(member.Id, "selfAssignMenuSelected", new[] {menu.Id.ToString()});
        return new DiscordSelectComponent(customId, "Select role(s)",
            options, minOptions: 0, maxOptions: options.Count);
    }
}