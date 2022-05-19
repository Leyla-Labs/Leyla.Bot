using Common.Classes;
using Db.Helper;
using DSharpPlus;
using Spam.Events;
using Spam.Helper;

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
        SpamHelper.MaxPressureExceeded += SpamHelperOnMaxPressureExceeded.HandleEvent;
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