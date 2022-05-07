using Common.Checks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Main.Handler.BaseClasses;

namespace Main.Handler;

public sealed class ContextMenuExecutionChecksFailedExceptionHandler : ContextMenuErrorHandler
{
    private readonly ContextMenuExecutionChecksFailedException _ex;
    
    public ContextMenuExecutionChecksFailedExceptionHandler(ContextMenuErrorEventArgs e, ContextMenuExecutionChecksFailedException ex) : base(e)
    {
        _ex = ex;
    }

    public override async Task HandleException()
    {
        var embed = _ex.FailedChecks[0] switch
        {
            MenuRequireTargetMember => GetRequireTargetMemberEmbed(),
            _ => throw new NotImplementedException()
        };

        await Args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    private static DiscordEmbed GetRequireTargetMemberEmbed()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Member required");
        embed.WithDescription("It looks like the user you selected has left the server.");
        embed.WithColor(DiscordColor.Red);
        return embed.Build();
    }
}