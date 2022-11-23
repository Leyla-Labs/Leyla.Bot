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

        var (rolesAdd, rolesRemove) = GetRoles(menu);
        await AssignRoles(rolesAdd, rolesRemove);

        if (rolesAdd.Any() || rolesRemove.Any())
        {
            var embed = CreateEmbed(rolesAdd, rolesRemove);
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
        }
        else
        {
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        }
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

    private (ICollection<DiscordRole> rolesAdd, ICollection<DiscordRole> rolesRemove) GetRoles(SelfAssignMenu menu)
    {
        var member = (DiscordMember) EventArgs.User;

        // Get all roles from menu
        var menuRoles = EventArgs.Guild.Roles.Where(x =>
                menu.SelfAssignMenuDiscordEntityAssignments.Select(y => y.DiscordEntityId).Contains(x.Value.Id))
            .Select(x => x.Value).ToList();

        var rolesAdd = menuRoles.Where(x =>
            EventArgs.Values.Select(y => Convert.ToUInt64(y)).Contains(x.Id) &&
            !member.Roles.Select(y => y.Id).Contains(x.Id)).ToArray();

        var rolesRemove = menuRoles.Where(x =>
            !EventArgs.Values.Select(y => Convert.ToUInt64(y)).Contains(x.Id) &&
            member.Roles.Select(y => y.Id).Contains(x.Id)).ToArray();

        return (rolesAdd, rolesRemove);
    }

    private async Task AssignRoles(IEnumerable<DiscordRole> rolesAdd, IEnumerable<DiscordRole> rolesRemove)
    {
        var member = (DiscordMember) EventArgs.User;

        foreach (var role in rolesAdd)
        {
            await member.GrantRoleAsync(role);
        }

        foreach (var role in rolesRemove)
        {
            await member.RevokeRoleAsync(role);
        }
    }

    private static DiscordEmbed CreateEmbed(ICollection<DiscordRole> rolesAdd, ICollection<DiscordRole> rolesRemove)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Roles assigned");

        if (rolesAdd.Any())
        {
            embed.AddField("Roles added", string.Join(Environment.NewLine, rolesAdd.Select(x => x.Mention)), true);
        }

        if (rolesRemove.Any())
        {
            embed.AddField("Roles removed", string.Join(Environment.NewLine, rolesRemove.Select(x => x.Mention)), true);
        }

        return embed.Build();
    }
}