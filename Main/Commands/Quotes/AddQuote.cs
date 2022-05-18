using Common.Extensions;
using Db.Classes;
using Db.Helper;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public sealed class AddQuote : ContextMenuCommand
{
    public AddQuote(ContextMenuContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var msg = Ctx.TargetMessage;

        // check if quote already exists
        if (await DbCtx.Quotes.AnyAsync(x => x.MessageId == msg.Id))
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Duplicate Quote",
                    "That message has already been quoted."));
            return;
        }

        var displayName = await Ctx.GetDisplayName(Ctx.TargetMessage.Author.Id);

        // show modal
        var responseBuilder = GetModal(displayName);
        await Ctx.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);

        // wait for user response to modal
        var interactivity = Ctx.Client.GetInteractivity();
        var userResponse = await interactivity.WaitForModalAsync($"modal-addquote-{Ctx.User.Id}", Ctx.User);
        if (userResponse.TimedOut)
        {
            return;
        }

        // get value from modal and add to database
        var modalInteraction = userResponse.Result.Interaction;
        var text = userResponse.Result.Values["text"];
        await AddToDatabase(msg, Ctx.TargetMessage.Author.Id, Ctx.Guild.Id, text);

        // show confirmation embed
        var embed = GetConfirmationEmbed(msg, displayName, text);
        await modalInteraction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }

    #region Static methods

    private static DiscordEmbed GetConfirmationEmbed(SnowflakeObject m, string displayName, string text)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("New Quote");
        embed.WithDescription(
            $"**\"{text}\"**{Environment.NewLine}- {displayName}, {m.CreationTimestamp.Date.Year}");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion

    #region Instance methods

    private DiscordInteractionResponseBuilder GetModal(string displayName)
    {
        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle($"Add Quote: {displayName}")
            .WithCustomId($"modal-addquote-{Ctx.User.Id}")
            .AddComponents(new TextInputComponent("Quote", "text", max_length: 2000, min_length: 1,
                style: TextInputStyle.Paragraph, value: Ctx.TargetMessage.Content));
        return response;
    }

    private async Task AddToDatabase(DiscordMessage m, ulong userId, ulong guildId,
        string text)
    {
        await MemberHelper.CreateIfNotExist(userId, guildId);

        await DbCtx.Quotes.AddAsync(new Quote
        {
            Text = text,
            Date = m.CreationTimestamp.DateTime.ToUniversalTime(),
            MessageId = m.Id,
            MemberId = m.Author.Id
        });
        await DbCtx.SaveChangesAsync();
    }

    #endregion
}