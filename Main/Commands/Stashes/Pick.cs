using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Stashes;

public class Pick : SlashCommand
{
    private readonly string? _stashName;

    public Pick(InteractionContext ctx, string? stashName) : base(ctx)
    {
        _stashName = stashName;
    }

    public override async Task RunAsync()
    {
        var entry = await PickEntry();

        if (entry == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("No entry found").AsEphemeral());
            return;
        }

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(entry.Value));
    }

    #region Instance members

    private async Task<StashEntry?> PickEntry()
    {
        await using var context = new DatabaseContext();

        var query = context.StashEntries.Where(x => x.Stash.GuildId == Ctx.Guild.Id).OrderBy(x => Guid.NewGuid());

        return _stashName != null
            ? await query.Where(x => x.Stash.Name.Equals(_stashName)).FirstOrDefaultAsync()
            : await query.FirstOrDefaultAsync();
    }

    #endregion
}