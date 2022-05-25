using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Stashes;

public class RemoveFrom : SlashCommand
{
    private readonly string _stashName;
    private readonly string _val;

    public RemoveFrom(InteractionContext ctx, string stashName, string val) : base(ctx)
    {
        _stashName = stashName;
        _val = val;
    }

    public override async Task RunAsync()
    {
        await using var context = new DatabaseContext();

        var entry = await FindEntry(context);

        if (entry == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Entry not found").AsEphemeral());
            return;
        }

        context.Entry(entry).State = EntityState.Deleted;
        await context.SaveChangesAsync();

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(GetConfirmationEmbed(entry)).AsEphemeral());
    }

    #region Instance methods

    private async Task<StashEntry?> FindEntry(DatabaseContext context)
    {
        StashEntry? entry = null;

        if (int.TryParse(_val, out var skip))
        {
            // try to get by PK
            entry = await context.StashEntries.Where(x =>
                    x.Stash.GuildId == Ctx.Guild.Id &&
                    x.Stash.Name.Equals(_stashName))
                .Skip(skip - 1)
                .FirstOrDefaultAsync();
        }

        if (entry != null)
        {
            return entry;
        }

        // try to get by value itself
        return await context.StashEntries.Where(x =>
                x.Stash.GuildId == Ctx.Guild.Id &&
                x.Stash.Name.Equals(_stashName) &&
                x.Value.Equals(_val))
            .FirstOrDefaultAsync();
    }

    private DiscordEmbed GetConfirmationEmbed(StashEntry entry)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Entry Deleted");
        embed.WithDescription($"The entry `{entry.Value}` has been delete from {_stashName}.");
        embed.WithColor(DiscordColor.DarkRed);
        return embed.Build();
    }

    #endregion
}