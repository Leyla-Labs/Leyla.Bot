using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;

namespace Main.Handler;

internal sealed class SelfAssignMenuRolesSelectedHandler : InteractionHandler
{
    private readonly string _menuId;

    public SelfAssignMenuRolesSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e,
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

        await AssignRoles(menu);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
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

    private async Task AssignRoles(SelfAssignMenu menu)
    {
        var member = (DiscordMember) EventArgs.User;

        // Get all roles from menu
        var menuRoles = EventArgs.Guild.Roles.Where(x =>
                menu.SelfAssignMenuDiscordEntityAssignments.Select(y => y.DiscordEntityId).Contains(x.Value.Id))
            .Select(x => x.Value).ToList();

        // Add newly selected roles
        foreach (var newRole in menuRoles.Where(x =>
                     EventArgs.Values.Select(y => Convert.ToUInt64(y)).Contains(x.Id) &&
                     !member.Roles.Select(y => y.Id).Contains(x.Id)))
        {
            await member.GrantRoleAsync(newRole);
        }

        // Remove unselected roles
        foreach (var revRole in menuRoles.Where(x =>
                     !EventArgs.Values.Select(y => Convert.ToUInt64(y)).Contains(x.Id) &&
                     member.Roles.Select(y => y.Id).Contains(x.Id)))
        {
            await member.RevokeRoleAsync(revRole);
        }
    }
}