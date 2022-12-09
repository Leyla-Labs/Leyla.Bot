using Common.Events;
using Common.Helper;
using Common.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace Common.Classes;

public abstract class Leyla : IBot
{
    private DiscordClient? Client { get; set; }

    public async Task StartAsync()
    {
        Client = await InitBotAsync();
        RegisterCommands();
        RegisterInteractivity();
        await Client.ConnectAsync();
    }

    public virtual async Task StopAsync()
    {
        if (Client != null)
        {
            await Client.DisconnectAsync();
        }
    }

    public void Dispose()
    {
        Client?.Dispose();
    }

    protected virtual SlashCommandsExtension RegisterCommands()
    {
        var commands = Client.UseSlashCommands();
        commands.SlashCommandErrored += CommandsOnSlashCommandErroredEvent.CommandsOnSlashCommandErroredAsync;
        commands.ContextMenuErrored += CommandsOnContextMenuErroredEvent.CommandsOnContextMenuErroredAsync;
        return commands;
    }

    private void RegisterInteractivity()
    {
        Client.UseInteractivity(new InteractivityConfiguration
        {
            Timeout = new TimeSpan(0, 0, 0, 30)
        });
    }

    protected async Task ClientOnGuildDownloadCompletedAsync(DiscordClient sender, GuildDownloadCompletedEventArgs e)
    {
        if (Client != null)
        {
            await new StartupHelper(Client).SendStartupMessageAsync();
        }
    }

    #region Abstract methods

    protected abstract Task<DiscordClient> InitBotAsync();

    #endregion
}