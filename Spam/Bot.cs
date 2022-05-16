using Common.Classes;
using DSharpPlus;
using Spam.Events;

namespace Spam;

public class Bot : Leyla
{
    protected override DiscordClient InitBot()
    {
        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = Environment.GetEnvironmentVariable("TOKEN"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds |
                      DiscordIntents.GuildMessages
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompleted;
        client.MessageCreated += ClientOnMessageCreated.HandleEvent;
        return client;
    }

    protected override void RegisterCommands()
    {
        // do nothing
    }
}