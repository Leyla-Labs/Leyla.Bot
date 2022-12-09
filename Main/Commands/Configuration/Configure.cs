using Common.Classes;
using Common.GuildConfig;
using Common.Helper;
using Common.Records;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Extensions;

namespace Main.Commands.Configuration;

internal sealed class Configure : SlashCommand
{
    public Configure(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var categories = await GetCategoriesAsync();
        var embed = CreateCategoryEmbed(categories);
        var buttons = CreateButtons(categories);

        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddComponents(buttons)
            .AddComponents(CreateLinkButton()).AddEmbed(embed)
            .AsEphemeral());
    }

    #region Static methods

    private static DiscordEmbed CreateCategoryEmbed(IEnumerable<ConfigOptionCategory> categories)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Select Category to configure");

        foreach (var category in categories)
        {
            embed.AddField(category.Name, category.ConfigStrings.Description);
        }

        return embed.Build();
    }

    private static DiscordLinkButtonComponent CreateLinkButton()
    {
        var url = "https://github.com/Leyla-Labs/Leyla.Bot/wiki/Server-Configuration";
        return new DiscordLinkButtonComponent(url, "Show more details on the wiki");
    }

    #endregion

    #region Instance methods

    private async Task<ICollection<ConfigOptionCategory>> GetCategoriesAsync()
    {
        var modules = await Ctx.Guild.GetGuildModulesAsync();
        return GuildConfigOptionCategories.Instance.Get().Where(x => modules.Contains(x.Module)).ToArray();
    }

    private IEnumerable<DiscordButtonComponent> CreateButtons(IEnumerable<ConfigOptionCategory> categories)
    {
        return categories
            .Select(x => new
            {
                category = x,
                customId = ModalHelper.GetModalName(Ctx.User.Id, "configCategories", new[] {x.Id.ToString()})
            })
            .Select(x => new DiscordButtonComponent(ButtonStyle.Primary, x.customId, x.category.Name))
            .ToArray();
    }

    #endregion
}