using Common.Classes;
using DSharpPlus;
using Logs.Events;

namespace Logs;

public class Bot : Leyla
{
    protected override Task<DiscordClient> InitBotAsync()
    {
        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = Environment.GetEnvironmentVariable("TOKEN_LOGS"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds
                      | DiscordIntents.GuildMessages
                      | DiscordIntents.GuildMembers
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompletedAsync;
        client.GuildMemberAdded += ClientOnGuildMemberAdded.HandleEventAsync;
        client.GuildMemberRemoved += ClientOnGuildMemberRemoved.HandleEventAsync;
        client.MessageDeleted += ClientOnMessageDeleted.HandleEventAsync;
        client.MessageUpdated += ClientOnMessageUpdated.HandleEventAsync;
        return Task.FromResult(client);
    }
}