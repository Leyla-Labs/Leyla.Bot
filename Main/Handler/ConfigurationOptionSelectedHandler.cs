using Common.Classes;
using Common.GuildConfig;
using Common.Helper;
using Common.Records;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Main.Enums;

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
        var option = ConfigOptions.Instance.Get(optionId);
        var embed = await CreateEmbed(option);
        var buttons = await CreateButtons(option);

        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AddComponents(buttons).AsEphemeral());
    }

    private async Task<DiscordEmbed> CreateEmbed(ConfigOption option)
    {
        var placeholder = "/";

        var displayString =
            await ConfigHelper.Instance.GetDisplayStringForCurrentValue(option, EventArgs.Guild, true, placeholder);

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(option.Name);
        embed.WithDescription(option.Description);
        embed.AddField("Current Value", displayString);

        if (option.DefaultValue == null)
        {
            return embed.Build();
        }

        // add default value if option has one
        var defaultDisplayString =
            await ConfigHelper.GetDisplayStringForDefaultValue(option, EventArgs.Guild, true, placeholder);
        embed.AddField("Default value", defaultDisplayString, true);

        return embed.Build();
    }

    private async Task<IEnumerable<DiscordButtonComponent>> CreateButtons(ConfigOption option)
    {
        var list = new List<DiscordButtonComponent>();

        var customIdEdit = ModalHelper.GetModalName(EventArgs.User.Id, "configOptionAction",
            new[] {ConfigurationAction.Edit.ToString(), option.Id.ToString()});
        list.Add(new DiscordButtonComponent(ButtonStyle.Primary, customIdEdit, "Edit"));

        var isDefaultValue = await ConfigHelper.Instance.IsDefaultValue(option, EventArgs.Guild.Id);

        if (option.DefaultValue != null && !isDefaultValue)
        {
            // option has default value and currently set value is not said default value
            var customIdReset = ModalHelper.GetModalName(EventArgs.User.Id, "configOptionAction",
                new[] {ConfigurationAction.Reset.ToString(), option.Id.ToString()});
            list.Add(new DiscordButtonComponent(ButtonStyle.Secondary, customIdReset, "Reset to default"));
        }

        if (!option.Nullable || await ConfigHelper.Instance.GetString(option.Name, EventArgs.Guild.Id) == null)
        {
            return list;
        }

        // option is nullable and currently set value is not null
        var customIdDelete = ModalHelper.GetModalName(EventArgs.User.Id, "configOptionAction",
            new[] {ConfigurationAction.Delete.ToString(), option.Id.ToString()});
        list.Add(new DiscordButtonComponent(ButtonStyle.Danger, customIdDelete, "Delete value"));

        return list;
    }
}