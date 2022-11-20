using Common.Classes;
using Common.GuildConfig;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Main.Extensions;

namespace Main.Handler;

internal sealed class ConfigurationCategorySelectedHandler : InteractionHandler
{
    private readonly string _categoryId;

    public ConfigurationCategorySelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e,
        string categoryId) :
        base(sender, e)
    {
        _categoryId = categoryId;
    }

    public override async Task RunAsync()
    {
        var categoryId = Convert.ToInt32(_categoryId);
        var optionEmbed = GetOptionEmbed(categoryId);
        var optionSelect = await GetOptionSelect(categoryId);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(optionEmbed).AddComponents(optionSelect).AsEphemeral());
    }

    private static DiscordEmbed GetOptionEmbed(int categoryId)
    {
        var category = ConfigOptionCategories.Instance.Get(categoryId);

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(category.Name);
        embed.WithDescription(category.Description);
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    private async Task<DiscordSelectComponent> GetOptionSelect(int categoryId)
    {
        var modules = await EventArgs.Guild.GetGuildModules();
        var configOptions = ConfigOptions.Instance.Get().Where(x => x.ConfigOptionCategory.Id == categoryId);
        configOptions = configOptions.Where(x => modules.Contains(x.Module)).ToArray();

        var currentValues = new Dictionary<int, string>();
        foreach (var configOption in configOptions)
        {
            var currVal =
                await ConfigHelper.Instance.GetDisplayStringForCurrentValue(configOption, EventArgs.Guild, false);
            if (currVal != null)
            {
                currentValues.Add(configOption.Id, currVal);
            }
        }

        var options = configOptions.Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(),
                currentValues.TryGetValue(x.Id, out var result) ? result : null));
        var customId = ModalHelper.GetModalName(EventArgs.User.Id, "configOptions");
        return new DiscordSelectComponent(customId, "Select option to configure", options,
            minOptions: 1, maxOptions: 1);
    }
}