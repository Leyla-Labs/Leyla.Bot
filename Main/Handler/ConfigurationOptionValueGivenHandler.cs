using Common.Classes;
using Common.Enums;
using Common.Extensions;
using Common.Helper;
using Common.Statics;
using Common.Statics.BaseClasses;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Main.Handler;

internal sealed class ConfigurationOptionValueGivenHandler : ModalHandler
{
    private readonly string _optionId;

    public ConfigurationOptionValueGivenHandler(DiscordClient sender, ModalSubmitEventArgs e, string optionId) :
        base(sender, e)
    {
        _optionId = optionId;
    }

    public override async Task RunAsync()
    {
        var option = ConfigOptions.Instance.Get(Convert.ToInt32(_optionId));
        var value = EventArgs.Values.First(x => x.Key.Equals("value")).Value;

        switch (option.ConfigType)
        {
            case ConfigType.String:
                await ConfigHelper.Instance.Set(option, EventArgs.Interaction.Guild.Id, value);
                break;
            case ConfigType.Int:
                if (!int.TryParse(value, out var valueInt))
                {
                    await ShowErrorAsync(option);
                    return;
                }

                await ConfigHelper.Instance.Set(option, EventArgs.Interaction.Guild.Id, valueInt);
                break;
            case ConfigType.Char:
                var valueChar = Convert.ToChar(value);
                await ConfigHelper.Instance.Set(option, EventArgs.Interaction.Guild.Id, valueChar);
                break;
            case ConfigType.Decimal:
                if (!decimal.TryParse(value, out var valueDecimal))
                {
                    await ShowErrorAsync(option);
                    return;
                }

                await ConfigHelper.Instance.Set(option, EventArgs.Interaction.Guild.Id, valueDecimal);
                break;
            case ConfigType.Boolean:
            case ConfigType.Role:
            case ConfigType.Channel:
            case ConfigType.Enum:
            default:
                throw new ArgumentOutOfRangeException(nameof(option.ConfigType));
        }

        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
    }

    private async Task ShowErrorAsync(ConfigOption option)
    {
        if (!new[] {ConfigType.Int, ConfigType.Decimal}.Contains(option.ConfigType))
        {
            throw new ArgumentOutOfRangeException(nameof(option), option.ConfigType,
                "Only int and decimal have error handling.");
        }

        var description = option.ConfigType == ConfigType.Int
            ? "The input needs to be a whole number. (eg. 2; 84; 0)"
            : "The input needs to be a decimal number. (eg. 14; 8.4; 2.65)";

        var button = CreateButton(option);

        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddErrorEmbed("Invalid input", description).AddComponents(button)
                .AsEphemeral());
    }

    private DiscordButtonComponent CreateButton(StaticField option)
    {
        var customId =
            ModalHelper.GetModalName(EventArgs.Interaction.User.Id, "configOptions", new[] {option.Id.ToString()});
        return new DiscordButtonComponent(ButtonStyle.Primary, customId, "Reopen modal");
    }
}