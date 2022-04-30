using Common.Classes;
using Common.Helper;
using Db.Helper;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Main.Events;
using Main.Modules;

namespace Main;

public class Bot : Leyla
{
    protected override DiscordClient InitBot()
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

    protected override void RegisterCommands()
    {
        var commands = Client.UseSlashCommands();

#if DEBUG
        commands.RegisterCommands<AniList>(640467169733246976);
#else
        commands.RegisterCommands<AniList>();
#endif

        commands.SlashCommandErrored += CommandsOnSlashCommandErroredEvent.CommandsOnSlashCommandErrored;
    }
}