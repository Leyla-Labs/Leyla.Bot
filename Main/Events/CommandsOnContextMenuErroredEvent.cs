using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

namespace Main.Events;

public static class CommandsOnContextMenuErroredEvent
{
    public static Task CommandsOnContextMenuErrored(SlashCommandsExtension sender,
        ContextMenuErrorEventArgs e)
    {
        return Task.CompletedTask;
    }
}