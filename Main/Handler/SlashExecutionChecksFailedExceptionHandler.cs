using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using Main.Handler.BaseClasses;

namespace Main.Handler;

public class SlashExecutionChecksFailedExceptionHandler : SlashCommandErrorHandler
{
    private readonly SlashExecutionChecksFailedException _ex;

    public SlashExecutionChecksFailedExceptionHandler(SlashCommandErrorEventArgs e,
        SlashExecutionChecksFailedException ex) : base(e)
    {
        _ex = ex;
    }

    public override async Task HandleException()
    {
        if (_ex.FailedChecks.Count <= 0) throw new NotImplementedException();

        if (_ex.FailedChecks[0] is SlashRequireBotPermissionsAttribute attr)
        {
            var embed = GetRequireBotPermissionEmbed(attr);
            await Args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(embed));
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private static DiscordEmbed GetRequireBotPermissionEmbed(SlashRequireBotPermissionsAttribute attr)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Missing Permission",
            Description = attr.Permissions.ToPermissionString() // TODO to title case
        };

        embed.WithColor(DiscordColor.IndianRed);

        return embed.Build();
    }
}