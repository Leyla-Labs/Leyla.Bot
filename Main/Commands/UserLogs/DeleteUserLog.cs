using Common.Extensions;
using Db;
using Db.Classes;
using Db.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.UserLogs;

public class DeleteUserLog : SlashCommand
{
    private readonly DiscordMember _member;
    private readonly long _n;

    public DeleteUserLog(InteractionContext ctx, DiscordMember member, long n) : base(ctx)
    {
        _member = member;
        _n = n;
    }

    public override async Task RunAsync()
    {
        await using var context = new DatabaseContext();

        var userLog = await GetUserLog(context);

        if (userLog == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Entry not found").AsEphemeral());
            return;
        }

        context.Entry(userLog).State = EntityState.Deleted;
        await context.SaveChangesAsync();

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(GetConfirmationEmbed(userLog)).AsEphemeral());
    }

    #region Instance methods

    private async Task<UserLog?> GetUserLog(DatabaseContext context)
    {
        return await context.UserLogs.Where(x =>
                x.Member.GuildId == Ctx.Guild.Id &&
                x.MemberId == _member.Id)
            .Skip(Convert.ToInt32(_n - 1))
            .FirstOrDefaultAsync();
    }

    private DiscordEmbed GetConfirmationEmbed(UserLog log)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("User Log Deleted");
        embed.WithDescription($"The {log.Type} log for {_member.DisplayName} has been deleted.");
        embed.WithColor(DiscordColor.DarkRed);
        return embed.Build();
    }

    #endregion
}