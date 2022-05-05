using Common.Classes;
using Db.Statics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Main.Handler;

public class ConfigurationCategorySelectedHandler : InteractionHandler
{
    public ConfigurationCategorySelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e) :
        base(sender, e)
    {
    }

    public override async Task RunAsync()
    {
        var categoryId = Convert.ToInt32(EventArgs.Values[0]);
        var optionEmbed = GetOptionEmbed(categoryId);
        var optionSelect = GetOptionSelect(categoryId);
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

    private DiscordSelectComponent GetOptionSelect(int categoryId)
    {
        var configOptions = ConfigOptions.Instance.Get().Where(x => x.ConfigOptionCategoryId == categoryId).ToList();
        var options = configOptions.Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(), x.Description));
        return new DiscordSelectComponent($"configOptions-{EventArgs.User.Id}", "Select option to configure", options,
            minOptions: 1, maxOptions: 1);
    }
}