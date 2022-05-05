using Common.Classes;
using Db.Statics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.Configuration;

public sealed class Configure : SlashCommand
{
    public Configure(InteractionContext ctx) : base(ctx)
    {
    }
    
    public override async Task RunAsync()
    {
        var categorySelect = GetCategorySelect();
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddComponents(categorySelect)
            .AsEphemeral());
    }

    private DiscordSelectComponent GetCategorySelect()
    {
        var categories = ConfigOptions.Instance.Get();
        var options = categories.Select(x => new DiscordSelectComponentOption(x.Name, x.Id.ToString(), x.Description));
        return new DiscordSelectComponent($"categories-{Ctx.Member.Id}", "Select category to configure", options,
            minOptions: 1, maxOptions: 1);
    }
}