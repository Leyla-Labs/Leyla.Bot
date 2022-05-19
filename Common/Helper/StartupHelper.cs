using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Common.Helper;

public class StartupHelper
{
    private readonly DiscordClient _bot;

    public StartupHelper(DiscordClient bot)
    {
        _bot = bot;
    }

    public async Task SendStartupMessage()
    {
        var id = Convert.ToUInt64(Environment.GetEnvironmentVariable("MAIN_CHANNEL"));
        var channel = await _bot.GetChannelAsync(id);

        if (channel == null)
        {
            Console.Write("Main channel not found. Exiting.");
            Environment.Exit(0);
        }

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(_bot.CurrentApplication.Name);

        if (Assembly.GetExecutingAssembly().GetName().Version is { } v)
        {
            embed.AddField("Version", $"{v?.Major}.{v?.Minor}.{v?.Build}");
        }

        await channel.SendMessageAsync(embed.Build());
    }
}