using Common.Classes;
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
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompleted;
        client.MessageDeleted += ClientOnMessageDeleted.HandleEvent;
        return client;
    }

    protected override void RegisterCommands()
    {
        // TODO
    }
}