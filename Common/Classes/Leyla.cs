using Common.Helper;
using Common.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Common.Classes;

public abstract class Leyla : IBot
{
    protected DiscordClient Client { get; private set; } = null!;

    public async Task StartAsync()
    {
        Configuration.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;
        await LoadConfig();
        Client = InitBot();
        RegisterCommands();
        RegisterInteractivity();
        await Client.ConnectAsync();
    }

    public async Task StopAsync()
    {
        await Client.DisconnectAsync();
    }

    public void Dispose()
    {
        Client.Dispose();
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
    protected abstract Task LoadConfig();

    #endregion
}