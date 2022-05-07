using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using Main.Handler.BaseClasses;

namespace Main.Handler;

public sealed class SlashExecutionChecksFailedExceptionHandler : SlashCommandErrorHandler
{
    private readonly SlashExecutionChecksFailedException _ex;

    public SlashExecutionChecksFailedExceptionHandler(SlashCommandErrorEventArgs e,
        SlashExecutionChecksFailedException ex) : base(e)
    {
        _ex = ex;
    }

    public override async Task HandleException()
    {
        var embed = _ex.FailedChecks[0] switch
        {
            SlashRequireBotPermissionsAttribute check => GetRequireBotPermissionEmbed(check),
            _ => throw new ArgumentOutOfRangeException()
        };

        await Args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    private static DiscordEmbed GetRequireBotPermissionEmbed(SlashRequireBotPermissionsAttribute attr)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Missing Permission",
            Description = attr.Permissions.ToPermissionString() // TODO to title case
        };

        embed.WithColor(DiscordColor.Red);

        return embed.Build();
    }
}