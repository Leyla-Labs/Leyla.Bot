using Common.Classes;
using Common.Helper;
using Common.Statics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Main.Extensions;

namespace Main.Handler;

internal sealed class ConfigurationCategorySelectedHandler : InteractionHandler
{
    public ConfigurationCategorySelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e) :
        base(sender, e)
    {
    }

    public override async Task RunAsync()
    {
        var categoryId = Convert.ToInt32(EventArgs.Values[0]);
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
        var modules = await EventArgs.Guild.GetGuildModulesAsync();
        var configOptions = ConfigOptions.Instance.Get().Where(x => x.ConfigOptionCategoryId == categoryId);
        configOptions = configOptions.Where(x => x.Module == null || modules.Contains(x.Module.Value));
        var options = configOptions.Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(), x.Description));
        var customId = ModalHelper.GetModalName(EventArgs.User.Id, "configOptions");
        return new DiscordSelectComponent(customId, "Select option to configure", options,
            minOptions: 1, maxOptions: 1);
    }
}