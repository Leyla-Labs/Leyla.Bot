using System.Text;
using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Stashes;

public class List : SlashCommand
{
    private readonly string? _stashName;

    public List(InteractionContext ctx, string? stashName) : base(ctx)
    {
        _stashName = stashName;
    }

    public override async Task RunAsync()
    {
        if (_stashName == null)
        {
            var stashSelect = await GetStashSelect();
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddComponents(stashSelect).AsEphemeral());
            return;
        }

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
                x.GuildId == Ctx.Guild.Id &&
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

    private async Task<DiscordSelectComponent> GetStashSelect()
    {
        await using var context = new DatabaseContext();

        var stashes = await context.Stashes.Where(x =>
                x.GuildId == Ctx.Guild.Id)
            .Include(x => x.StashEntries)
            .ToListAsync();

        var options = stashes.Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(), $"{x.StashEntries.Count} entries"));

        var customId = ModalHelper.GetModalName(Ctx.User.Id, "stashSelected");
        return new DiscordSelectComponent(customId, "Select stash to pick from", options, minOptions: 1, maxOptions: 1);
    }

    #endregion
}