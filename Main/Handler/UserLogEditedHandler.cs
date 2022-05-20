using Common.Classes;
using Common.Db;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;

namespace Main.Handler;

public class UserLogEditedHandler : ModalHandler
{
    private readonly string _userLogId;

    public UserLogEditedHandler(DiscordClient sender, ModalSubmitEventArgs eventArgs, string userLogId) : base(sender,
        eventArgs)
    {
        _userLogId = userLogId;
    }

    public override async Task RunAsync()
    {
        var reason = EventArgs.Values["reason"];
        var additionalDetails = EventArgs.Values["additionalDetails"];
        var dateStr = EventArgs.Values["date"];
        var date = dateStr.GetDateTimeFromDisplayString();

        if (date == null)
        {
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Could not parse date",
                        "Please make sure the date and time you entered matches the format shown in the modal.")
                    .AsEphemeral());
            return;
        }

        await EditInDatabase(reason, additionalDetails, date.Value);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        // TODO post entry in logs
    }

    private async Task EditInDatabase(string reason, string additionalDetails, DateTime date)
    {
        var userLogId = Convert.ToInt32(_userLogId);

        await using var context = new DatabaseContext();
        var log = await context.UserLogs.FirstAsync(x => x.Id == userLogId);
        log.Reason = reason;
        log.AdditionalDetails = additionalDetails;
        log.Date = date;
        context.Entry(log).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}