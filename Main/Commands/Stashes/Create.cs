using Common.Classes;
using Common.Db;
using Common.Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.Stashes;

public class Create : SlashCommand
{
    private readonly string _stashName;

    public Create(InteractionContext ctx, string stashName) : base(ctx)
    {
        _stashName = stashName;
    }

    public override async Task RunAsync()
    {
        // TODO check if stash with name already exists and show error

        await CreateStashInDatabase(null);
        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(GetConfirmationEmbed()).AsEphemeral());
    }

    #region Instance methods

    private async Task CreateStashInDatabase(ulong? requiredRoleId)
    {
        await using var context = new DatabaseContext();
        await context.Stashes.AddAsync(new Stash
        {
            Name = _stashName,
            GuildId = Ctx.Guild.Id,
            RequiredRoleId = requiredRoleId
        });
        await context.SaveChangesAsync();
    }

    private DiscordEmbed GetConfirmationEmbed()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Stash Created");
        embed.WithDescription(
            $"The stash `{_stashName}` has been created. You can right click messages to add entries to it.");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}