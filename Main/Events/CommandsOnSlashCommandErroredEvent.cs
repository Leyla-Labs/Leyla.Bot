using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Main.Handler;

namespace Main.Events;

public static class CommandsOnSlashCommandErroredEvent
{
    public static async Task CommandsOnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
    {
        switch (e.Exception)
        {
            case SlashExecutionChecksFailedException ex:
                // TODO
                throw new NotImplementedException();
            case GraphQL.Client.Http.GraphQLHttpRequestException ex:
                await new GraphQlHttpRequestExceptionHandler(e, ex).HandleException();
                break;
        }
    }
}