using System.ComponentModel.DataAnnotations;
using Common.Classes;
using Common.GuildConfig;
using Common.Helper;
using Common.Records;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Main.Enums;
using xivapi_cs.Extensions;

namespace Main.Handler;

public class ConfigurationOptionSelectedHandler : InteractionHandler
{
    private readonly string? _optionId;

    public ConfigurationOptionSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e) :
        base(sender, e)
    {
    }

    public ConfigurationOptionSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e,
        string optionId) :
        this(sender, e)
    {
        _optionId = optionId;
    }

    public override async Task RunAsync()
    {
        // try to get from values (select menu, see ConfigurationCategorySelectedHandler)
        // fallback to optionId (button, see ConfigurationOptionValueGivenHandler)
        var optionIdString = EventArgs.Values.FirstOrDefault() ?? _optionId
            ?? throw new ArgumentNullException(nameof(_optionId), "both value sources are null.");

        var optionId = Convert.ToInt32(optionIdString);
        var option = GuildConfigOptions.Instance.Get(optionId);
        var embed = await CreateEmbed(option);
        var buttons = await CreateButtonsAsync(option);

        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AddComponents(buttons).AsEphemeral());
    }

    private async Task<DiscordEmbed> CreateEmbed(ConfigOption option)
    {
        var placeholder = "/";
        var embed = new DiscordEmbedBuilder();

        var typeDisplayAttr = option.ConfigType.GetAttribute<DisplayAttribute>() ??
                              throw new NullReferenceException("DisplayAttribute for ConfigType must not be null");
        embed.AddField($"Type: {typeDisplayAttr.Name}", typeDisplayAttr.Description);

        var displayString =
            await GuildConfigHelper.Instance.GetDisplayStringForCurrentValueAsync(option, EventArgs.Guild, true,
                placeholder);

        embed.WithTitle(option.Name);
        embed.WithDescription(option.ConfigStrings.Description);
        embed.AddField("Current Value", displayString, true);

        if (option.DefaultValue == null)
        {
            return embed.Build();
        }

        // add default value if option has one
        var defaultDisplayString = GuildConfigHelper.GetDisplayStringForDefaultValue(option, true, placeholder);
        embed.AddField("Default value", defaultDisplayString, true);

        return embed.Build();
    }

    private async Task<IEnumerable<DiscordComponent>> CreateButtonsAsync(ConfigOption option)
    {
        var list = new List<DiscordComponent>();

        var customIdEdit = ModalHelper.GetModalName(EventArgs.User.Id, "configOptionAction",
            new[] {ConfigurationAction.Edit.ToString(), option.Id.ToString()});
        list.Add(new DiscordButtonComponent(ButtonStyle.Primary, customIdEdit, "Edit"));

        var isDefaultValue = await GuildConfigHelper.Instance.IsDefaultValueAsync(option, EventArgs.Guild.Id);

        if (option.DefaultValue != null && !isDefaultValue)
        {
            // option has default value and currently set value is not said default value
            var customIdReset = ModalHelper.GetModalName(EventArgs.User.Id, "configOptionAction",
                new[] {ConfigurationAction.Reset.ToString(), option.Id.ToString()});
            list.Add(new DiscordButtonComponent(ButtonStyle.Danger, customIdReset, "Revert to default"));
        }

        if (option.Nullable && await GuildConfigHelper.Instance.GetStringAsync(option.Name, EventArgs.Guild.Id) != null)
        {
            // option is nullable and currently set value is not null
            var customIdDelete = ModalHelper.GetModalName(EventArgs.User.Id, "configOptionAction",
                new[] {ConfigurationAction.Delete.ToString(), option.Id.ToString()});
            list.Add(new DiscordButtonComponent(ButtonStyle.Danger, customIdDelete, "Delete value"));
        }

        if (option.ConfigStrings.WikiUrl is { } url)
        {
            list.Add(new DiscordLinkButtonComponent(url, "More details on the wiki"));
        }

        return list;
    }
}