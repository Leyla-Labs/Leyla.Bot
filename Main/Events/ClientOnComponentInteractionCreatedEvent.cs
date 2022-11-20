using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Handler;

namespace Main.Events;

internal static class ClientOnComponentInteractionCreatedEvent
{
    public static async Task ClientOnComponentInteractionCreated(DiscordClient sender,
        ComponentInteractionCreateEventArgs e)
    {
        // info consists of userId, name, and any further information after that
        var info = e.Id.Split("_");
        var userId = Convert.ToUInt64(info[0]);

        if (userId != 1 && userId != e.User.Id)
        {
            // 1 if anyone can use component
            return; // TODO handle this
        }

        var additionalInfo = info.Skip(2).ToArray();

        switch (info[1])
        {
            case "configOptionValueSelected" when additionalInfo.Length < 1:
            case "addToStash" when additionalInfo.Length < 1:
            case "userLogType" when additionalInfo.Length < 1:
            case "manageMenu" when additionalInfo.Length < 1:
            case "selfAssignMenu" when additionalInfo.Length < 1:
            case "selfAssignMenuSelected" when additionalInfo.Length < 1:
            case "configCategories" when additionalInfo.Length < 1:
            case "configOptionAction" when additionalInfo.Length < 2:
                throw new NullReferenceException(nameof(additionalInfo));
            case "configCategories":
                await new ConfigurationCategorySelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "configOptions":
                await new ConfigurationOptionSelectedHandler(sender, e).RunAsync();
                break;
            case "configOptionAction":
                await new ConfigurationOptionActionSelectedHandler(sender, e, additionalInfo[0], additionalInfo[1])
                    .RunAsync();
                break;
            case "configOptionValueSelected":
                await new ConfigurationOptionValueSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "addToStash":
                await new AddToStashSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "stashSelected":
                await new PickStashSelectedHandler(sender, e).RunAsync();
                break;
            case "userLogType":
                await new UserLogTypeSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "manageMenu":
                await new SelfAssignMenuManageHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "selfAssignMenu":
                await new SelfAssignMenuButtonPressedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "selfAssignMenuSelected":
                await new SelfAssignMenuRolesSelectedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            default:
                return;
        }
    }
}