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

        switch (info[0])
        {
            case "configOptionValueGiven" when secondaryInfo == null:
                throw new NullReferenceException(nameof(secondaryInfo));
            case "configOptionValueGiven":
                await new ConfigurationOptionValueGivenHandler(sender, e, secondaryInfo.Value).RunAsync();
                break;
            case "addToStash" when secondaryInfo == null:
                throw new NullReferenceException(nameof(secondaryInfo));
            case "addToStash" when info.Length < 3:
                throw new NullReferenceException(nameof(info));
            case "addToStash":
                await new StashEntryValueGivenHandler(sender, e, info[2]).RunAsync();
                break;
        }
    }
}