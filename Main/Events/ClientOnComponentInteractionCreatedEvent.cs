using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Handler;

namespace Main.Events;

public static class ClientOnComponentInteractionCreatedEvent
{
    public static async Task ClientOnComponentInteractionCreated(DiscordClient sender,
        ComponentInteractionCreateEventArgs e)
    {
        // info consists of userId, name, and any further information after that
        var info = e.Id.Split("-");
        var userId = Convert.ToUInt64(info[0]);

        if (userId != e.User.Id)
        {
            return; // TODO handle this
        }

        var additionalInfo = info.Skip(2).ToArray();

        switch (info[1])
        {
            case "configCategories":
                await new ConfigurationCategorySelectedHandler(sender, e).RunAsync();
                break;
            case "configOptions":
                await new ConfigurationOptionSelectedHandler(sender, e).RunAsync();
                break;
            case "configOptionValueSelected" when additionalInfo.Length < 1:
                throw new NullReferenceException(nameof(additionalInfo));
            case "configOptionValueSelected":
                await new ConfigurationOptionValueSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "addToStash" when additionalInfo.Length < 1:
                throw new NullReferenceException(nameof(additionalInfo));
            case "addToStash":
                await new AddToStashSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "stashSelected":
                await new PickStashSelectedHandler(sender, e).RunAsync();
                break;
            case "userLogType" when additionalInfo.Length < 1:
                throw new NullReferenceException(nameof(additionalInfo));
            case "userLogType":
                await new UserLogTypeSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
        }
    }
}
