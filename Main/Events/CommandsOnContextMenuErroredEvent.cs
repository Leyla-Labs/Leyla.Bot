using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Main.Handler;

namespace Main.Events;

public static class CommandsOnContextMenuErroredEvent
{
    public static async Task CommandsOnContextMenuErrored(SlashCommandsExtension sender,
        ContextMenuErrorEventArgs e)
    {
        if (e.Exception is ContextMenuExecutionChecksFailedException ex)
        {
            await new ContextMenuExecutionChecksFailedExceptionHandler(e, ex).HandleException();
        }
    }
}