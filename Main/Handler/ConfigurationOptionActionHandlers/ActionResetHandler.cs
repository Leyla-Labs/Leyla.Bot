using Common.Classes;
using Common.GuildConfig;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Main.Handler.ConfigurationOptionActionHandlers;

public class ActionResetHandler : InteractionHandler
{
    private readonly string _optionId;

    public ActionResetHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e, string optionId) :
        base(sender, e)
    {
        _optionId = optionId;
    }

    public override async Task RunAsync()
    {
        var optionId = Convert.ToInt32(_optionId);
        await GuildConfigHelper.Instance.Reset(optionId, EventArgs.Guild.Id);

        var embed = await CreateEmbed(optionId);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    private async Task<DiscordEmbed> CreateEmbed(int optionId)
    {
        var option = ConfigOptions.Instance.Get(optionId);

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Value reset");
        embed.WithDescription($"The value for {option.Name} has been reset.");
        embed.AddField("New value",
            await GuildConfigHelper.GetDisplayStringForDefaultValue(option, EventArgs.Guild, true));
        return embed.Build();
    }
}