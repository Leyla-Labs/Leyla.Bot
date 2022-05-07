using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Handler;

namespace Main.Events;

public static class ClientOnModalSubmittedEvent
{
    public static async Task ClientOnModalSubmitted(DiscordClient sender, ModalSubmitEventArgs e)
    {
        if (e.Interaction.Data.CustomId == null)
        {
            throw new ArgumentNullException(nameof(e.Interaction.Data.CustomId));
        }

        var info = e.Interaction.Data.CustomId.Split("-");
        ulong? secondaryInfo = info.Length > 1 && ulong.TryParse(info[1], out var result) ? result : null;
        ulong? tertiaryInfo = info.Length > 2 && ulong.TryParse(info[2], out var result2) ? result2 : null;

        switch (info[0])
        {
            case "configOptionValueGiven" when secondaryInfo == null:
                throw new NullReferenceException(nameof(secondaryInfo));
            case "configOptionValueGiven":
                await new ConfigurationOptionValueGivenHandler(sender, e, secondaryInfo.Value).RunAsync();
                break;
            case "addUserLog" when secondaryInfo == null:
                throw new NullReferenceException(nameof(secondaryInfo));
            case "addUserLog" when tertiaryInfo == null:
                throw new NullReferenceException(nameof(tertiaryInfo));
            case "addUserLog":
            {
                await new UserLogReasonGivenHandler(sender, e, secondaryInfo.Value, tertiaryInfo.Value).RunAsync();
                break;
            }
        }
    }
}