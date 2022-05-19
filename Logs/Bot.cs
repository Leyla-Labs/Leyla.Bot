using Common.Classes;
using Db.Helper;
using DSharpPlus;
using Logs.Events;

namespace Logs;

public class Bot : Leyla
{
    protected override DiscordClient InitBot()
    {
        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = Environment.GetEnvironmentVariable("TOKEN"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds
                      | DiscordIntents.GuildMessages
                      | DiscordIntents.GuildMembers
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompleted;
        client.GuildMemberAdded += ClientOnGuildMemberAdded.HandleEvent;
        client.GuildMemberRemoved += ClientOnGuildMemberRemoved.HandleEvent;
        client.MessageDeleted += ClientOnMessageDeleted.HandleEvent;
        client.MessageUpdated += ClientOnMessageUpdated.HandleEvent;
        return client;
    }

    protected override void RegisterCommands()
    {
        // do nothing
    }

    protected override async Task LoadConfig()
    {
        await ConfigHelper.Instance.Initialise();
    }
}