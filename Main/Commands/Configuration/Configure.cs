using Common.Classes;
using Common.Helper;
using Common.Statics;
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
        //var categorySelect = await GetCategorySelect();

        var categories = await GetCategories();
        var embed = CreateCategoryEmbed(categories);
        var buttons = CreateButtons(categories);

        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddComponents(buttons).AddEmbed(embed)
            .AsEphemeral());
    }

    #region Static methods

    private static DiscordEmbed CreateCategoryEmbed(IEnumerable<ConfigOptionCategory> categories)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Select Category to configure");

        foreach (var category in categories)
        {
            embed.AddField(category.Name, category.Description);
        }

        return embed.Build();
    }

    #endregion

    #region Instance methods

    private async Task<ICollection<ConfigOptionCategory>> GetCategories()
    {
        var modules = await Ctx.Guild.GetGuildModules();
        return ConfigOptionCategories.Instance.Get().Where(x => modules.Contains(x.Module)).ToArray();
    }

    private IEnumerable<DiscordButtonComponent> CreateButtons(IEnumerable<ConfigOptionCategory> categories)
    {
        return categories
            .Select(x => new
            {
                category = x,
                customId = ModalHelper.GetModalName(Ctx.User.Id, "configCategories", new[] {x.Id.ToString()})
            })
            .Select(x => new DiscordButtonComponent(ButtonStyle.Secondary, x.customId, x.category.Name))
            .ToArray();
    }

    #endregion
}