using Common.Classes;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Main.Events;
using Main.Modules;

namespace Main;

public sealed class Bot : Leyla
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
        client.GuildRoleDeleted += ClientOnGuildRoleDeleted.HandleEvent;
        return client;
    }

    protected override SlashCommandsExtension RegisterCommands()
    {
        var commands = base.RegisterCommands();

        commands.RegisterCommands<Configuration>();
        commands.RegisterCommands<Moderation>();
        commands.RegisterCommands<UserLogs>();
        commands.RegisterCommands<CommandLogs>();
        commands.RegisterCommands<Quotes>();
        commands.RegisterCommands<Stashes>();
        commands.RegisterCommands<SelfAssignMenus>();
        commands.RegisterCommands<AniList>();

        return commands;
    }
}