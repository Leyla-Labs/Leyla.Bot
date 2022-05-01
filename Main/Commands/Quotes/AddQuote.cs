using Common.Extensions;
using Db;
using Db.Helper;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.Quotes;

public static class AddQuote
{
    public static async Task RunMenu(ContextMenuContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var msg = ctx.TargetMessage;
        
        await using var context = new DatabaseContext();
        
        if (await context.Quotes.AnyAsync(x => x.MessageId == msg.Id))
        {
            var duplicateEmbed = new DiscordEmbedBuilder()
            {
                Title = "Duplicate Quote",
                Description = "That message has already been quoted."
            }.WithColor(DiscordColor.IndianRed).Build();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(duplicateEmbed));
            return;
        }

        await AddToDatabase(context, msg, ctx.TargetMessage.Author.Id, ctx.Guild.Id);

        var member = ctx.Guild.Members.FirstOrDefault(x => x.Value.Id == ctx.TargetMessage.Author.Id).Value;
        var displayName = member?.DisplayName ?? ctx.TargetMessage.Author.Username;
        var embed = GetConfirmationEmbed(msg, displayName);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }
    
    private static async Task AddToDatabase(DatabaseContext context, DiscordMessage m, ulong userId, ulong guildId)
    {
        await MemberHelper.CreateIfNotExist(userId, guildId);
        
        await context.Quotes.AddAsync(new Quote
        {
            Text = m.Content,
            Date = m.CreationTimestamp.DateTime.ToUniversalTime(),
            MessageId = m.Id,
            MemberId = m.Author.Id
        });
        await context.SaveChangesAsync();
    }

    private static DiscordEmbed GetConfirmationEmbed(DiscordMessage m, string displayName)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("New Quote");
        embed.WithDescription(
            $"**\"{m.Content}\"**{Environment.NewLine}- {displayName}, {m.CreationTimestamp.Date.Year}");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }
}