using Common.Classes;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.SelfAssignMenus;

public class Delete : SlashCommand
{
    private readonly string _title;

    public Delete(InteractionContext ctx, string title) : base(ctx)
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

        DbCtx.Entry(menu).State = EntityState.Deleted;
        await DbCtx.SaveChangesAsync();

        var embed = GetConfirmationEmbed(menu.Title);
        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    #region Instance methods

    private async Task<SelfAssignMenu?> GetSelfAssignMenu()
    {
        return await DbCtx.SelfAssignMenus.FirstOrDefaultAsync(x =>
            x.GuildId == Ctx.Guild.Id &&
            x.Title.Equals(_title));
    }

    #endregion

    #region Static methods

    private static DiscordEmbed GetConfirmationEmbed(string title)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Self Assign Menu Deleted");
        embed.WithDescription($"The Self Assign Menu {title} has been deleted.");
        embed.WithColor(DiscordColor.DarkRed);
        return embed.Build();
    }

    #endregion
}