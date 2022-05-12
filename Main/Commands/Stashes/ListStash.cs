using System.Text;
using Common.Classes;
using Common.Extensions;
using Db;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Stashes;

public class ListStash : SlashCommand
{
    private readonly string _stashName;

    public ListStash(InteractionContext ctx, string stashName) : base(ctx)
    {
        _stashName = stashName;
    }

    public override async Task RunAsync()
    {
        var stash = await GetStash();

        if (stash == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed($"Stash \"{_stashName}\" not found")
                    .AsEphemeral());
            return;
        }

        var embed = GetEmbed(stash);
        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    #region Static methods

    private static string GetDescription(IReadOnlyList<StashEntry> stashEntries)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < stashEntries.Count; i++)
        {
            var entry = stashEntries[i];
            sb.Append($"**{i + 1}.** {entry.Value}{Environment.NewLine}");
        }

        return sb.ToString();
    }

    #endregion

    #region Instance methods

    private async Task<Stash?> GetStash()
    {
        await using var context = new DatabaseContext();
        return await context.Stashes.Where(x =>
                x.Name.Equals(_stashName))
            .Include(x => x.StashEntries)
            .FirstOrDefaultAsync();
    }

    private DiscordEmbed GetEmbed(Stash stash)
    {
        var description = GetDescription(stash.StashEntries.ToList());

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(stash.Name);
        embed.WithDescription(description);
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}