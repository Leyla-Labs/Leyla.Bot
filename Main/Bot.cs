using Common.Classes;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Main.Events;
using Main.Modules;

namespace Main;

public class Bot : Leyla
{
    protected override async Task<DiscordClient> InitBot()
    {
        await ConfigHelper.Instance.Initialise();

        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = Environment.GetEnvironmentVariable("TOKEN_MAIN"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompleted;
        client.ComponentInteractionCreated +=
            ClientOnComponentInteractionCreatedEvent.ClientOnComponentInteractionCreated;
        client.ModalSubmitted += ClientOnModalSubmittedEvent.ClientOnModalSubmitted;
        return client;
    }

    protected override SlashCommandsExtension RegisterCommands()
    {
        var commands = base.RegisterCommands();

        commands.RegisterCommands<Configuration>();
        commands.RegisterCommands<Moderation>();
        commands.RegisterCommands<UserLogs>();
        commands.RegisterCommands<Quotes>();
        commands.RegisterCommands<Stashes>();
        commands.RegisterCommands<AniList>();

        return commands;
    }
}