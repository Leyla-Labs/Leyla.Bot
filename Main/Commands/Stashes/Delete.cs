using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Stashes;

internal sealed class Delete : SlashCommand
{
    private readonly string _stashName;

    public Delete(InteractionContext ctx, string stashName) : base(ctx)
    {
        _stashName = stashName;
    }

    public override async Task RunAsync()
    {
        await using var context = new DatabaseContext();

        if (await GetStashAsync(context) is not { } stash)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Stash not found").AsEphemeral());
            return;
        }

        context.Entry(stash).State = EntityState.Deleted;
        await context.SaveChangesAsync();

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(GetConfirmationEmbed()).AsEphemeral());
    }

    #region Instance methods

    private async Task<Stash?> GetStashAsync(DatabaseContext context)
    {
        return await context.Stashes.Where(x =>
                x.GuildId == Ctx.Guild.Id &&
                x.Name.Equals(_stashName))
            .FirstOrDefaultAsync();
    }

    private DiscordEmbed GetConfirmationEmbed()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Stash Deleted");
        embed.WithDescription($"The stash {_stashName} and all its entries have been deleted.");
        embed.WithColor(DiscordColor.DarkRed);
        return embed.Build();
    }

    #endregion
}