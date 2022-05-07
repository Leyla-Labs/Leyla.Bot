using Common.Classes;
using Db.Enums;
using Db.Helper;
using Db.Statics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Main.Handler;

public class ConfigurationOptionSelectedHandler : InteractionHandler
{
    public ConfigurationOptionSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e) :
        base(sender, e)
    {
    }

    public override async Task RunAsync()
    {
        var optionId = Convert.ToInt32(EventArgs.Values[0]);
        var option = ConfigOptions.Instance.Get(optionId);
        var optionDetailsEmbed = GetOptionDetailsEmbed(option);

        switch (option.Type)
        {
            case ConfigType.String:
            case ConfigType.Int:
            case ConfigType.Char:
                var modalBuilder = await GetModal(option);
                await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalBuilder);
                break;
            case ConfigType.Boolean:
                var boolSelect = await GetBoolSelect(option);
                await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(optionDetailsEmbed).AddComponents(boolSelect)
                        .AsEphemeral());
                break;
            case ConfigType.Role:
                var roleSelect = await GetRoleSelect(option);
                await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(optionDetailsEmbed).AddComponents(roleSelect)
                        .AsEphemeral());
                break;
            case ConfigType.Channel:
                var channelSelect = await GetChannelSelect(option);
                await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(optionDetailsEmbed).AddComponents(channelSelect)
                        .AsEphemeral());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option.Type));
        }
    }

    private static DiscordEmbed GetOptionDetailsEmbed(ConfigOption option)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(option.Name);
        embed.WithDescription(option.Description);
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    private async Task<DiscordSelectComponent> GetRoleSelect(ConfigOption option)
    {
        // TODO support more than 25 roles
        // TODO support clearing config by selecting none
        var currentConfig = await ConfigHelper.Instance.GetRole(option.Name, EventArgs.Guild);
        var options = EventArgs.Guild.Roles.Take(25).Select(x =>
            new DiscordSelectComponentOption(x.Value.Name, x.Key.ToString(), isDefault: x.Key == currentConfig?.Id));
        return new DiscordSelectComponent($"configOptionValueSelected-{option.Id}", "Select role", options,
            minOptions: 1, maxOptions: 1);
    }

    private async Task<DiscordSelectComponent> GetChannelSelect(ConfigOption option)
    {
        // TODO support more than 25 channels
        var currentConfig = await ConfigHelper.Instance.GetChannel(option.Name, EventArgs.Guild);
        var channels = await EventArgs.Guild.GetChannelsAsync();
        var channelsFiltered = channels.Where(x => !x.IsCategory).Take(25);
        var options = channelsFiltered.Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(), isDefault: x.Id == currentConfig?.Id));
        return new DiscordSelectComponent($"configOptionValueSelected-{option.Id}", "Select channel", options,
            minOptions: 1, maxOptions: 1);
    }

    private async Task<DiscordSelectComponent> GetBoolSelect(ConfigOption option)
    {
        var currentConfig = await ConfigHelper.Instance.GetBool(option.Name, EventArgs.Guild.Id);
        var options = new List<DiscordSelectComponentOption>
            {new("On", "1", isDefault: currentConfig == true), new("Off", "0", isDefault: currentConfig != true)};
        return new DiscordSelectComponent($"configOptionValueSelected-{option.Id}", "Select value", options,
            minOptions: 1, maxOptions: 1);
    }

    private async Task<DiscordInteractionResponseBuilder> GetModal(ConfigOption option)
    {
        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle(option.Name);
        response.WithCustomId($"configOptionValueGiven-{option.Id}");

        var placeholder = option.Type switch
        {
            ConfigType.String => "Enter text",
            ConfigType.Char => "Enter singular character",
            ConfigType.Int => "Enter digits",
            _ => "Enter value"
        };

        var maxLength = option.Type switch
        {
            ConfigType.String => 100,
            ConfigType.Char => 1,
            ConfigType.Int => 9 // 9 to avoid any value > maxInt troubles
        };

        var currentConfig = await ConfigHelper.Instance.GetString(option.Name, EventArgs.Guild.Id) ?? string.Empty;

        response.AddComponents(new TextInputComponent("Value", "value", placeholder, currentConfig, min_length: 1,
            max_length: maxLength));
        return response;
    }
}