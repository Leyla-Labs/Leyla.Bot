using Common.Classes;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Spam.Events;
using Spam.Helper;
using Spam.Modules;

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
                      DiscordIntents.GuildMessages |
                      DiscordIntents.GuildMembers
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompleted;
        client.MessageCreated += ClientOnMessageCreated.HandleEventAsync;
        client.GuildMemberAdded += ClientOnGuildMemberAdded.HandleEventAsync;
        client.GuildMemberRemoved += ClientOnGuildMemberRemoved.HandleEventAsync;
        client.ComponentInteractionCreated +=
            ClientOnComponentInteractionCreatedEvent.HandleEventAsync;
        SpamHelper.MaxPressureExceeded += SpamHelperOnMaxPressureExceeded.HandleEventAsync;
        RaidHelper.RaidDetected += RaidHelperOnRaidDetected.HandleEventAsync;
        return Task.FromResult(client);
    }

    protected override SlashCommandsExtension RegisterCommands()
    {
        var commands = base.RegisterCommands();

        commands.RegisterCommands<Raid>();
        commands.RegisterCommands<Silence>();

        return commands;
    }
}