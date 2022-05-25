using Common.Classes;
using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.UserLogs;

public class Edit : SlashCommand
{
    private readonly DiscordMember _member;
    private readonly long _n;

    public Edit(InteractionContext ctx, DiscordMember member, long n) : base(ctx)
    {
        _member = member;
        _n = n;
    }

    public override async Task RunAsync()
    {
        var userLog = await GetUserLog();

        if (userLog == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("Entry not found").AsEphemeral());
            return;
        }

        var modal = GetModal(userLog);
        await Ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
    }

    private async Task<UserLog?> GetUserLog()
    {
        await using var context = new DatabaseContext();
        return await context.UserLogs.Where(x =>
                x.Member.GuildId == Ctx.Guild.Id &&
                x.MemberId == _member.Id)
            .Skip(Convert.ToInt32(_n - 1))
            .FirstOrDefaultAsync();
    }

    private DiscordInteractionResponseBuilder GetModal(UserLog log)
    {
        var response = new DiscordInteractionResponseBuilder();
        response.WithTitle($"Edit {log.Type} log for {_member.DisplayName}");
        response.WithCustomId(ModalHelper.GetModalName(Ctx.User.Id, "editUserLog", new[] {log.Id.ToString()}));
        var dateStr = log.Date.GetDisplayString();
        var l = dateStr.Length;
        response.AddComponents(new TextInputComponent("Date and Time in UTC (dd.MM.yyyy HH:mm)", "date", value: dateStr,
            min_length: l,
            max_length: l));
        response.AddComponents(new TextInputComponent("Reason", "reason", style: TextInputStyle.Paragraph,
            min_length: 1, max_length: 210, value: log.Reason)); // 1.5 tweets
        response.AddComponents(new TextInputComponent("Additional Details", "additionalDetails", required: false,
            style: TextInputStyle.Paragraph, max_length: 420, value: log.AdditionalDetails)); // 3 tweets
        return response;
    }
}