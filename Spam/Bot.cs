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
        client.MessageCreated += ClientOnMessageCreated.HandleEvent;
        client.GuildMemberAdded += ClientOnGuildMemberAdded.HandleEvent;
        client.GuildMemberRemoved += ClientOnGuildMemberRemoved.HandleEvent;
        client.ComponentInteractionCreated +=
            ClientOnComponentInteractionCreatedEvent.HandleEvent;
        SpamHelper.MaxPressureExceeded += SpamHelperOnMaxPressureExceeded.HandleEvent;
        RaidHelper.RaidDetected += RaidHelperOnRaidDetected.HandleEvent;
        return Task.FromResult(client);
    }

    protected override SlashCommandsExtension RegisterCommands()
    {
        var commands = base.RegisterCommands();

        commands.RegisterCommands<Raid>();

        return commands;
    }
}