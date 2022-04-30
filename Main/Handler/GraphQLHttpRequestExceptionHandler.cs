using System.Net;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands.EventArgs;
using GraphQL.Client.Http;
using Main.Handler.BaseClasses;

namespace Main.Handler;

public class GraphQlHttpRequestExceptionHandler : SlashCommandErrorHandler
{
    private readonly GraphQLHttpRequestException _ex;

    public GraphQlHttpRequestExceptionHandler(SlashCommandErrorEventArgs e, GraphQLHttpRequestException ex) :
        base(e)
    {
        _ex = ex;
    }

    public override async Task HandleException()
    {
        switch (_ex.StatusCode)
        {
            case HttpStatusCode.NotFound:
                var embed = GetNotFoundEmbed();
                await Args.Context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                break;
            default:
                return;
        }
    }

    private DiscordEmbed GetNotFoundEmbed()
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Not found"
        };
        // TODO show more details?

        embed.WithColor(DiscordColor.IndianRed);

        return embed.Build();
    }
}