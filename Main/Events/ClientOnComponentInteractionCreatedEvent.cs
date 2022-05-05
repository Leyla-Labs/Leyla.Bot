using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Handler;

namespace Main.Events;

public static class ClientOnComponentInteractionCreatedEvent
{
    public static async Task ClientOnComponentInteractionCreated(DiscordClient sender,
        ComponentInteractionCreateEventArgs e)
    {
        switch (e.Id)
        {
            case "configCategories":
                await new ConfigurationCategorySelectedHandler(sender, e).RunAsync();
                break;
            case "configOptions":
                await new ConfigurationOptionSelectedHandler(sender, e).RunAsync();
                break;
        }
    }
}