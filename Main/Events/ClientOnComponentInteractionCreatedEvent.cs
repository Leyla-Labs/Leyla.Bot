using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Handler;

namespace Main.Events;

public static class ClientOnComponentInteractionCreatedEvent
{
    public static async Task ClientOnComponentInteractionCreated(DiscordClient sender,
        ComponentInteractionCreateEventArgs e)
    {
        if (e.Id.Equals("configCategories"))
        {
            await new ConfigurationCategorySelectedHandler(sender, e).RunAsync();
        }
    }
}