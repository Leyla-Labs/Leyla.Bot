using Common.Classes;
using Common.Extensions;
using Db;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Stashes;

public class DeleteStash : SlashCommand
{
    private readonly string _stashName;
    
    public DeleteStash(InteractionContext ctx, string stashName) : base(ctx)
    {
        _stashName = stashName;
    }

    public override async Task RunAsync()
    {
        await using var context = new DatabaseContext();

        if (await GetStash(context) is not { } stash)
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
    
    private async Task<Stash?> GetStash(DatabaseContext context)
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