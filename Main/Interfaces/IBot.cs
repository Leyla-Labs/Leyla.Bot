using DSharpPlus;

namespace Main.Interfaces;

public interface IBot
{
    Task MainAsync();
    DiscordClient GetBot();
}