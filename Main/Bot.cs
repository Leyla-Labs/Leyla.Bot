using Db;
using Db.Helper;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Main.Interfaces;

namespace Main;

public class Bot : IBot
{
    private DiscordClient _bot = null!;

    public async Task MainAsync()
    {
        Configuration.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;
        await ConfigHelper.LoadGuildConfigs();
        _bot = InitBot();
        RegisterCommands();
        RegisterInteractivity();

        await _bot.ConnectAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    public DiscordClient GetBot()
    {
        return _bot;
    }

    private DiscordClient InitBot()
    {
        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = Environment.GetEnvironmentVariable("TOKEN"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds
        });
        client.GuildDownloadCompleted += ClientOnGuildDownloadCompleted;
        return client;
    }

    private void RegisterCommands()
    {
        var commands = _bot.UseSlashCommands();
    }

    private void RegisterInteractivity()
    {
        _bot.UseInteractivity(new InteractivityConfiguration
        {
            Timeout = new TimeSpan(0, 0, 0, 30)
        });
    }

    private async Task ClientOnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
    {
        await new StartupHelper(_bot).SendStartupMessage();
    }
}