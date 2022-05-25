using Common.Classes;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.CommandLogs;

public class User : SlashCommand
{
    private readonly DiscordMember _member;

    public User(InteractionContext ctx, DiscordMember member) : base(ctx)
    {
        _member = member;
    }

    public override async Task RunAsync()
    {
        var recent = await CommandLogHelper.Instance.GetRecent(_member);
        var embed = await CommandLogHelper.GetEmbed(Ctx.Guild, recent, false);

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }
}