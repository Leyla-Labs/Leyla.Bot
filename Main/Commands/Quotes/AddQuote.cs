using Db;
using Db.Helper;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public static class AddQuote
{
    public static async Task RunMenu(ContextMenuContext ctx)
    {
        var msg = ctx.TargetMessage;

        // check if quote already exists
        if (await CheckDuplicate(msg) is { } duplicateEmbed)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(duplicateEmbed));
            return;
        }

        var displayName = await GetDisplayName(ctx);

        // show modal
        var responseBuilder = GetModal(ctx, displayName);
        await ctx.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);

        // wait for user response to modal
        var interactivity = ctx.Client.GetInteractivity();
        var userResponse = await interactivity.WaitForModalAsync($"modal-addquote-{ctx.User.Id}", ctx.User);
        if (userResponse.TimedOut) return;

        // get value from modal and add to database
        var modalInteraction = userResponse.Result.Interaction;
        var text = userResponse.Result.Values["text"];
        await AddToDatabase(msg, ctx.TargetMessage.Author.Id, ctx.Guild.Id, text);
        
        // show confirmation embed
        var embed = GetConfirmationEmbed(msg, displayName, text);
        await modalInteraction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed));
    }

    private static async Task<DiscordEmbed?> CheckDuplicate(SnowflakeObject msg)
    {
        await using var context = new DatabaseContext();
        if (await context.Quotes.AnyAsync(x => x.MessageId == msg.Id))
            return new DiscordEmbedBuilder
            {
                Title = "Duplicate Quote",
                Description = "That message has already been quoted."
            }.WithColor(DiscordColor.IndianRed).Build();

        return null;
    }

    private static async Task<string> GetDisplayName(ContextMenuContext ctx)
    {
        var member = ctx.Guild.Members.FirstOrDefault(x => x.Value.Id == ctx.TargetMessage.Author.Id).Value;
        if (member == null) member = await ctx.Guild.GetMemberAsync(ctx.TargetMessage.Author.Id);
        return member?.DisplayName ?? ctx.TargetMessage.Author.Username;
    }

    private static DiscordInteractionResponseBuilder GetModal(ContextMenuContext ctx, string displayName)
    {
        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle($"Add Quote: {displayName}")
            .WithCustomId($"modal-addquote-{ctx.User.Id}")
            .AddComponents(new TextInputComponent("Quote", "text", max_length: 2000, min_length: 1,
                style: TextInputStyle.Paragraph, value: ctx.TargetMessage.Content));
        return response;
    }

    private static async Task AddToDatabase(DiscordMessage m, ulong userId, ulong guildId,
        string text)
    {
        await MemberHelper.CreateIfNotExist(userId, guildId);

        await using var context = new DatabaseContext();

        await context.Quotes.AddAsync(new Quote
        {
            Text = text,
            Date = m.CreationTimestamp.DateTime.ToUniversalTime(),
            MessageId = m.Id,
            MemberId = m.Author.Id
        });
        await context.SaveChangesAsync();
    }

    private static DiscordEmbed GetConfirmationEmbed(SnowflakeObject m, string displayName, string text)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("New Quote");
        embed.WithDescription(
            $"**\"{text}\"**{Environment.NewLine}- {displayName}, {m.CreationTimestamp.Date.Year}");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }
}