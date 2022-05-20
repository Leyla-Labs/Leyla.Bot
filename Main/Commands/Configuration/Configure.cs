using Common.Classes;
using Common.Helper;
using Common.Statics;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Extensions;

namespace Main.Commands.Configuration;

public sealed class Configure : SlashCommand
{
    public Configure(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var categorySelect = await GetCategorySelect();
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddComponents(categorySelect)
            .AsEphemeral());
    }

    #region Instance methods

    private async Task<DiscordSelectComponent> GetCategorySelect()
    {
        var modules = await Ctx.Guild.GetGuildModules();
        var categories = ConfigOptionCategories.Instance.Get().Where(x => modules.Contains(x.Module));
        var options = categories.Select(x => new DiscordSelectComponentOption(x.Name, x.Id.ToString(), x.Description));
        var name = ModalHelper.GetModalName(Ctx.User.Id, "configCategories");
        return new DiscordSelectComponent(name, "Select category to configure", options,
            minOptions: 1, maxOptions: 1);
    }

    #endregion
}