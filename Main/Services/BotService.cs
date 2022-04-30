namespace Main.Services;

public class BotService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        new Bot().MainAsync().GetAwaiter().GetResult();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // TODO send shutdown message and shut down bot
        return Task.CompletedTask;
    }
}