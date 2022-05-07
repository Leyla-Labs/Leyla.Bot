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

        if (info[0].Equals("configOptionValueGiven"))
        {
            if (secondaryInfo == null)
            {
                throw new NullReferenceException(nameof(secondaryInfo));
            }

            await new ConfigurationOptionValueGivenHandler(sender, e, secondaryInfo.Value).RunAsync();
        }
    }
}