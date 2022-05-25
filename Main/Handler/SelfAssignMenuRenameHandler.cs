using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;

namespace Main.Handler;

public class SelfAssignMenuRenameHandler : ModalHandler
{
    private readonly string _menuId;

    public SelfAssignMenuRenameHandler(DiscordClient sender, ModalSubmitEventArgs eventArgs, string menuId) : base(
        sender, eventArgs)
    {
        _menuId = menuId;
    }

    public override async Task RunAsync()
    {
        await using var context = new DatabaseContext();

        var menu = await GetSelfAssignMenu(context);

        if (menu == null)
        {
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Self assign menu not found.").AsEphemeral());
            return;
        }

        // TODO check for duplicate name

        var title = EventArgs.Values["title"] ?? string.Empty;
        var description = EventArgs.Values["description"];
        await EditInDatabase(context, menu, title, description);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
    }

    private async Task<SelfAssignMenu?> GetSelfAssignMenu(DatabaseContext context)
    {
        var id = Convert.ToInt32(_menuId);
        return await context.SelfAssignMenus.FirstOrDefaultAsync(x => x.Id == id);
    }

    private static async Task EditInDatabase(DbContext context, SelfAssignMenu menu, string title, string? description)
    {
        menu.Title = title;
        menu.Description = description;
        context.Entry(menu).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}