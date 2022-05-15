using Common.Classes;
using Common.Extensions;
using Db;
using Db.Enums;
using Db.Helper;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Main.Handler;

public class UserLogReasonGivenHandler : ModalHandler
{
    private readonly string _userId;
    private readonly UserLogType _userLogType;

    public UserLogReasonGivenHandler(DiscordClient sender, ModalSubmitEventArgs eventArgs, string userId,
        string userLogType) : base(sender, eventArgs)
    {
        _userId = userId;
        _userLogType = (UserLogType) Convert.ToInt32(userLogType);
    }

    public override async Task RunAsync()
    {
        var reason = EventArgs.Values["reason"];
        var additionalDetails = EventArgs.Values["additionalDetails"];
        var dateStr = EventArgs.Values["date"];
        var date = ParseDateStr(dateStr);

        if (date == null)
        {
            await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Could not parse date",
                        "Please make sure the date and time you entered matches the format shown in the modal.")
                    .AsEphemeral());
            return;
        }

        await AddToDatabase(reason, additionalDetails, date.Value);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        // TODO post entry in logs
    }

    private async Task AddToDatabase(string reason, string additionalDetails, DateTime date)
    {
        var guildId = EventArgs.Interaction.Guild.Id;
        var authorId = EventArgs.Interaction.User.Id;
        var userId = Convert.ToUInt64(_userId);
        await MemberHelper.CreateIfNotExist(userId, guildId); // create target user
        await MemberHelper.CreateIfNotExist(authorId, guildId); // create author

        await using var context = new DatabaseContext();
        await context.UserLogs.AddAsync(new UserLog
        {
            MemberId = userId,
            Reason = reason,
            AdditionalDetails = additionalDetails,
            Date = date,
            Type = _userLogType,
            AuthorId = authorId
        });
        await context.SaveChangesAsync();
    }

    private static DateTime? ParseDateStr(string dateStr)
    {
        try
        {
            // i hate this so much
            var day = Convert.ToInt32(dateStr[..2]);
            var month = Convert.ToInt32(dateStr.Substring(3, 2));
            var year = Convert.ToInt32(dateStr.Substring(6, 4));
            var hour = Convert.ToInt32(dateStr.Substring(11, 2));
            var minute = Convert.ToInt32(dateStr.Substring(14, 2));
            return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
        }
        catch
        {
            return null;
        }
    }
}