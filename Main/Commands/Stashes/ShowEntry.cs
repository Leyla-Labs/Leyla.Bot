using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Stashes;

internal sealed class ShowEntry : SlashCommand
{
    private readonly long _n;
    private readonly string _stashName;

    public ShowEntry(InteractionContext ctx, string stashName, long n) : base(ctx)
    {
        _stashName = stashName;
        _n = n;
    }

    public override async Task RunAsync()
    {
        var entry = await GetStashEntry();

        if (entry == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Entry not found").AsEphemeral());
            return;
        }

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(entry.Value));
    }

    private async Task<StashEntry?> GetStashEntry()
    {
        // TODO show error message if entire stash does not exist

        await using var context = new DatabaseContext();
        return await context.StashEntries.Where(x =>
                x.Stash.GuildId == Ctx.Guild.Id &&
                x.Stash.Name.Equals(_stashName))
            .Skip(Convert.ToInt32(_n) - 1)
            .FirstOrDefaultAsync();
    }
}