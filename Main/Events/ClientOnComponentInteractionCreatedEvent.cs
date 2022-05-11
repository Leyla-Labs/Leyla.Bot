using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Handler;

namespace Main.Events;

public static class ClientOnComponentInteractionCreatedEvent
{
    public static async Task ClientOnComponentInteractionCreated(DiscordClient sender,
        ComponentInteractionCreateEventArgs e)
    {
        var info = e.Id.Split("-");
        ulong? secondaryInfo = info.Length > 1 && ulong.TryParse(info[1], out var result) ? result : null;

        switch (info[0])
        {
            case "configCategories":
                await new ConfigurationCategorySelectedHandler(sender, e).RunAsync();
                break;
            case "configOptions":
                await new ConfigurationOptionSelectedHandler(sender, e).RunAsync();
                break;
            case "configOptionValueSelected" when secondaryInfo == null:
                throw new NullReferenceException(nameof(secondaryInfo));
            case "configOptionValueSelected":
                await new ConfigurationOptionValueSelectedHandler(sender, e, secondaryInfo.Value).RunAsync();
                break;
            case "addToStash" when info.Length < 2:
                throw new NullReferenceException(nameof(info));
            case "addToStash":
                await new StashSelectedHandler(sender, e, info[1]).RunAsync();
                break;
        }
    }
}