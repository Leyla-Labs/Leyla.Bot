using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Enums;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;

namespace Main.Handler;

internal sealed class SelfAssignMenuManageHandler : InteractionHandler
{
    private readonly string _menuId;

    public SelfAssignMenuManageHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e, string menuId) :
        base(sender, e)
    {
        _menuId = menuId;
    }

    public override async Task RunAsync()
    {
        await using var context = new DatabaseContext();

        var menu = await GetSelfAssignMenuAsync(context);

        if (menu == null)
        {
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Self assign menu not found.").AsEphemeral());
            return;
        }

        var roleIds = EventArgs.Values.Select(x => Convert.ToUInt64(x)).ToArray(); // skipcq: CS-R1068
        await SetValuesAsync(context, menu, roleIds);
        var embed = CreateEmbed(menu, roleIds);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    private async Task<SelfAssignMenu?> GetSelfAssignMenuAsync(DatabaseContext context)
    {
        var id = Convert.ToInt32(_menuId);
        return await context.SelfAssignMenus.Where(x =>
                x.Id == id)
            .Include(x => x.SelfAssignMenuDiscordEntityAssignments)
            .FirstOrDefaultAsync();
    }

    private DiscordEmbed CreateEmbed(SelfAssignMenu menu, IEnumerable<ulong> roleIds)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Updated: {menu.Title}");
        embed.WithDescription("The previous roles have been overwritten by your new selection.");

        var roles = EventArgs.Guild.Roles.Where(x => roleIds.Contains(x.Key)).Select(x => x.Value.Mention);
        embed.AddField("Roles", string.Join(Environment.NewLine, roles));

        return embed.Build();
    }

    private static async Task SetValuesAsync(DatabaseContext context, SelfAssignMenu menu, IEnumerable<ulong> roleIds)
    {
        // delete assignments for roles unselected by user
        foreach (var assignment in menu.SelfAssignMenuDiscordEntityAssignments.Where(x =>
                     !roleIds.Contains(x.DiscordEntityId)))
        {
            context.Entry(assignment).State = EntityState.Deleted;
        }

        // create assignments for newly selected roles
        var createList = new List<SelfAssignMenuDiscordEntityAssignment>();
        foreach (var roleId in roleIds.Where(x =>
                     !menu.SelfAssignMenuDiscordEntityAssignments.Select(y => y.DiscordEntityId).Contains(x)))
        {
            await DiscordEntityHelper.CreateIfNotExistAsync(DiscordEntityType.Role, roleId, menu.GuildId);

            createList.Add(new SelfAssignMenuDiscordEntityAssignment
            {
                SelfAssignMenuId = menu.Id,
                DiscordEntityId = roleId
            });
        }

        if (createList.Count > 0)
        {
            await context.SelfAssignMenuDiscordEntityAssignments.AddRangeAsync(createList);
        }

        await context.SaveChangesAsync();
    }
}