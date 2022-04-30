using Common.Helper;
using Common.Interfaces;
using Db;
using Db.Helper;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Common.Classes;

public abstract class Leyla : IBot
{
    protected DiscordClient Client { get; set; } = null!;

    public async Task MainAsync()
    {
        Configuration.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;
        await ConfigHelper.LoadGuildConfigs();
        Client = InitBot();
        RegisterCommands();
        RegisterInteractivity();

        await Client.ConnectAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    private void RegisterInteractivity()
    {
        Client.UseInteractivity(new InteractivityConfiguration
        {
            Timeout = new TimeSpan(0, 0, 0, 30)
        });
    }

    protected async Task ClientOnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
    {
        await new StartupHelper(Client).SendStartupMessage();
    }
    
    #region Abstract methods
    
    protected abstract DiscordClient InitBot();
    protected abstract void RegisterCommands();

    #endregion
}