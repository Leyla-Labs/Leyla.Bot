using Common.Classes;
using DSharpPlus;
using Spam.Events;
using Spam.Helper;

namespace Spam;

public class Bot : Leyla
{
    protected override Task<DiscordClient> InitBot()
    {
        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = Environment.GetEnvironmentVariable("TOKEN_SPAM"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds |
                      DiscordIntents.GuildMessages
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompleted;
        client.MessageCreated += ClientOnMessageCreated.HandleEvent;
        SpamHelper.MaxPressureExceeded += SpamHelperOnMaxPressureExceeded.HandleEvent;
        return Task.FromResult(client);
    }
}