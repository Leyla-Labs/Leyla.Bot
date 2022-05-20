using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;

namespace Main.Handler;

public class PickStashSelectedHandler : InteractionHandler
{
    public PickStashSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e) : base(sender, e)
    {
    }

    public override async Task RunAsync()
    {
        var entry = await GetStashEntry();

        if (entry == null)
        {
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Entry not found").AsEphemeral());
            return;
        }

        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        await EventArgs.Channel.SendMessageAsync(entry.Value);
    }

    private async Task<StashEntry?> GetStashEntry()
    {
        var id = Convert.ToInt32(EventArgs.Values[0]);

        await using var context = new DatabaseContext();
        return await context.StashEntries.Where(x =>
                x.StashId == id)
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }
}