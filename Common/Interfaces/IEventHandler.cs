using DSharpPlus;

namespace Common.Interfaces;

public interface IEventHandler<in T>
{
    static abstract Task HandleEventAsync(DiscordClient sender, T e);
}