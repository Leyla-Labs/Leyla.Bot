using Common.Classes;
using Microsoft.Extensions.Hosting;

namespace Common.Services;

public class BotService : IHostedService, IDisposable
{
    private readonly Leyla _bot;

    public BotService(Leyla bot)
    {
        _bot = bot;
    }

    public void Dispose() => _bot.Dispose();
    public async Task StartAsync(CancellationToken cancellationToken) => await _bot.StartAsync();
    public async Task StopAsync(CancellationToken cancellationToken) => await _bot.StopAsync();
}