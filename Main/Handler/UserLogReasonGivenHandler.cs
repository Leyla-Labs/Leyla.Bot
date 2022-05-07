using Common.Classes;
using Db;
using Db.Enums;
using Db.Helper;
using Db.Models;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Main.Handler;

public class UserLogReasonGivenHandler : ModalHandler
{
    private readonly ulong _userId;
    private readonly UserLogType _userLogType;
    
    public UserLogReasonGivenHandler(DiscordClient sender, ModalSubmitEventArgs eventArgs, ulong userId, ulong userLogType) : base(sender, eventArgs)
    {
        _userId = userId;
        _userLogType = (UserLogType) userLogType;
    }

    public override async Task RunAsync()
    {
        var reason = EventArgs.Values.First().Value;
        await AddToDatabase(reason);
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        // TODO post entry in logs
    }

    private async Task AddToDatabase(string reason)
    {
        var guildId = EventArgs.Interaction.Guild.Id;
        var authorId = EventArgs.Interaction.User.Id;
        await MemberHelper.CreateIfNotExist(_userId, guildId); // create target user
        await MemberHelper.CreateIfNotExist(authorId, guildId); // create author
        
        await using var context = new DatabaseContext();
        await context.UserLogs.AddAsync(new UserLog
        {
            MemberId = _userId,
            Text = reason,
            Date = DateTime.UtcNow,
            Type = _userLogType,
            AuthorId = authorId
        });
        await context.SaveChangesAsync();
    }
}