using Common.Classes;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Main.Events;
using Main.Modules;

namespace Main;

public sealed class Bot : Leyla
{
    protected override async Task<DiscordClient> InitBotAsync()
    {
        await ConfigHelper.Instance.Initialise();

        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = Environment.GetEnvironmentVariable("TOKEN_MAIN"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompletedAsync;
        client.ComponentInteractionCreated +=
            ClientOnComponentInteractionCreatedEvent.ClientOnComponentInteractionCreatedAsync;
        client.ModalSubmitted += ClientOnModalSubmittedEvent.ClientOnModalSubmittedAsync;
        client.GuildRoleDeleted += ClientOnGuildRoleDeleted.HandleEventAsync;
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
        commands.RegisterCommands<QuotesM>();
        commands.RegisterCommands<SelfAssignMenus>();
        commands.RegisterCommands<Stashes>();
        commands.RegisterCommands<StashesM>();
        commands.RegisterCommands<AniList>();
        commands.RegisterCommands<Ffxiv>();

        return commands;
    }

    public override async Task StopAsync()
    {
        await CommandLogHelper.Instance.TransferToDbAsync();
        await base.StopAsync();
    }
}