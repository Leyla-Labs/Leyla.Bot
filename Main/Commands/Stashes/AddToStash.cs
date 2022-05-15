using Common.Classes;
using Common.Helper;
using Db;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Stashes;

public class AddToStash : ContextMenuCommand
{
    public AddToStash(ContextMenuContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var selectId = ModalHelper.GetModalName(Ctx.User.Id, "addToStash", new[] {Ctx.TargetMessage.Content});
        var selectComponent = GetStashSelectComponent(await GetStashes(Ctx.Guild.Id), selectId);

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddComponents(selectComponent).AsEphemeral());
    }

    private static async Task<List<Stash>> GetStashes(ulong guildId)
    {
        await using var context = new DatabaseContext();
        return await context.Stashes.Where(x =>
                x.GuildId == guildId)
            .Take(25)
            .ToListAsync();
    }

    private static DiscordSelectComponent GetStashSelectComponent(ICollection<Stash> stashes, string selectId)
    {
        // TODO support more than 25 collections
        var options = stashes.Select(x => new DiscordSelectComponentOption(x.Name, x.Id.ToString()));
        return new DiscordSelectComponent(selectId, "Select stashes to add to", options, minOptions: 1,
            maxOptions: stashes.Count);
    }
}