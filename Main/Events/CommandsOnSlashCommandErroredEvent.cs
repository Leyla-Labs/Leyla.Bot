using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using GraphQL.Client.Http;
using Main.Handler;

namespace Main.Events;

public static class CommandsOnSlashCommandErroredEvent
{
    public static async Task CommandsOnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
    {
        switch (e.Exception)
        {
            case SlashExecutionChecksFailedException ex:
                await new SlashExecutionChecksFailedExceptionHandler(e, ex).HandleException();
                break;
            case GraphQLHttpRequestException ex:
                await new GraphQlHttpRequestExceptionHandler(e, ex).HandleException();
                break;
        }
    }
}