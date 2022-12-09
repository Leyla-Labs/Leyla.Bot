using Common.Classes;
using Common.Enums;
using Common.GuildConfig;
using Common.Helper;
using Common.Records;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Main.Handler;

internal sealed class ConfigurationOptionValueSelectedHandler : InteractionHandler
{
    private readonly string _optionId;

    public ConfigurationOptionValueSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e,
        string optionId) : base(sender, e)
    {
        _optionId = optionId;
    }

    public override async Task RunAsync()
    {
        var option = GuildConfigOptions.Instance.Get(Convert.ToInt32(_optionId));
        var value = EventArgs.Values[0];

        switch (option.ConfigType)
        {
            case ConfigType.Boolean:
                var valueBool = value.Equals("1");
                await GuildConfigHelper.Instance.SetAsync(option, EventArgs.Guild.Id, valueBool);
                break;
            case ConfigType.Role:
            case ConfigType.Channel:
                var valueUlong = Convert.ToUInt64(value);
                await GuildConfigHelper.Instance.SetAsync(option, EventArgs.Guild.Id, valueUlong);
                break;
            case ConfigType.Enum:
                await GuildConfigHelper.Instance.SetAsync(option, EventArgs.Guild.Id, value);
                break;
            case ConfigType.String:
            case ConfigType.Int:
            case ConfigType.Char:
            case ConfigType.Decimal:
            default:
                throw new ArgumentOutOfRangeException();
        }

        var embed = await CreateEmbedAsync(option);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    private async Task<DiscordEmbed> CreateEmbedAsync(ConfigOption option)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Value edited");
        embed.WithDescription($"The value for {option.Name} has been edited.");
        embed.AddField("New value",
            await GuildConfigHelper.Instance.GetDisplayStringForCurrentValueAsync(option, EventArgs.Guild, true));
        return embed.Build();
    }
}