using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Handler;

namespace Main.Events;

internal static class ClientOnModalSubmittedEvent
{
    public static async Task ClientOnModalSubmittedAsync(DiscordClient sender, ModalSubmitEventArgs e)
    {
        // info consists of userId, name, and any further information after that
        var info = e.Interaction.Data.CustomId.Split("_");
        var userId = Convert.ToUInt64(info[0]);

        if (userId != e.Interaction.User.Id)
        {
            return; // TODO handle this
        }

        var additionalInfo = info.Skip(2).ToArray();

        switch (info[1])
        {
            case "configOptionValueGiven" when additionalInfo.Length < 1:
            case "addToStash" when additionalInfo.Length < 1:
            case "addUserLog" when additionalInfo.Length < 2:
            case "editUserLog" when additionalInfo.Length < 1:
            case "renameMenu" when additionalInfo.Length < 1:
                throw new ArgumentNullException(nameof(e), nameof(additionalInfo));
            case "configOptionValueGiven":
                await new ConfigurationOptionValueGivenHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "addToStash":
                await new StashEntryValueGivenHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "addUserLog":
                await new UserLogReasonGivenHandler(sender, e, additionalInfo[0], additionalInfo[1]).RunAsync();
                break;
            case "editUserLog":
                await new UserLogEditedHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "renameMenu":
                await new SelfAssignMenuRenameHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            default:
                return;
        }
    }
}