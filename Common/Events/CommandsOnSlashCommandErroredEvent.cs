using Common.Handler;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using GraphQL.Client.Http;

namespace Common.Events;

public static class CommandsOnSlashCommandErroredEvent
{
    public static async Task CommandsOnSlashCommandErroredAsync(SlashCommandsExtension sender,
        SlashCommandErrorEventArgs e)
    {
        switch (e.Exception)
        {
            case SlashExecutionChecksFailedException ex1:
                await new SlashExecutionChecksFailedExceptionHandler(e, ex1).HandleExceptionAsync();
                break;
            case GraphQLHttpRequestException ex2:
                await new GraphQlHttpRequestExceptionHandler(e, ex2).HandleExceptionAsync();
                break;
            default:
                return;
        }
    }
}