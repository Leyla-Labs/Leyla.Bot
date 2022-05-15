using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Handler;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Main.Events;

public static class ClientOnModalSubmittedEvent
{
    public static async Task ClientOnModalSubmitted(DiscordClient sender, ModalSubmitEventArgs e)
    {
        // info consists of userId, name, and any further information after that
        var info = e.Interaction.Data.CustomId.Split("-");
        var userId = Convert.ToUInt64(info[0]);

        if (userId != e.Interaction.User.Id)
        {
            return; // TODO handle this
        }

        var additionalInfo = info.Skip(2).ToArray();

        switch (info[1])
        {
            case "configOptionValueGiven" when additionalInfo.Length < 1:
                throw new NullReferenceException(nameof(additionalInfo));
            case "configOptionValueGiven":
                await new ConfigurationOptionValueGivenHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
            case "addToStash" when additionalInfo.Length < 1:
                throw new NullReferenceException(nameof(additionalInfo));
            case "addToStash":
                await new StashEntryValueGivenHandler(sender, e, additionalInfo[0]).RunAsync();
                break;
			case "addUserLog" when additionalInfo.Length < 2:
                throw new NullReferenceException(nameof(additionalInfo));
            case "addUserLog":
                await new UserLogReasonGivenHandler(sender, e, additionalInfo[0], additionalInfo[1]).RunAsync();
                break;
        }
    }
}
