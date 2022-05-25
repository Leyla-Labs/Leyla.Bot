using Common.Classes;
using Common.Db.Models;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.SelfAssignMenus;

internal sealed class Rename : SlashCommand
{
    private readonly string _title;

    public Rename(InteractionContext ctx, string title) : base(ctx)
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

        var modal = GetModal(menu);
        await Ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
    }

    #region Instance methods

    private async Task<SelfAssignMenu?> GetSelfAssignMenu()
    {
        return await DbCtx.SelfAssignMenus.FirstOrDefaultAsync(x =>
            x.GuildId == Ctx.Guild.Id &&
            x.Title.Equals(_title));
    }

    private DiscordInteractionResponseBuilder GetModal(SelfAssignMenu menu)
    {
        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle(menu.Title);
        response.WithCustomId(ModalHelper.GetModalName(Ctx.User.Id, "renameMenu", new[] {menu.Id.ToString()}));
        response.AddComponents(new TextInputComponent("Title", "title", min_length: 1, max_length: 35,
            value: menu.Title));
        response.AddComponents(new TextInputComponent("Description", "description", min_length: 0, max_length: 140,
            required: false, value: menu.Description));
        return response;
    }

    #endregion
}