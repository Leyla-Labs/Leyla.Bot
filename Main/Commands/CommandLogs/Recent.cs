using Common.Classes;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.CommandLogs;

internal sealed class Recent : SlashCommand
{
    private readonly int _n;

    public Recent(InteractionContext ctx, long n) : base(ctx)
    {
        _n = n <= 100 ? Convert.ToInt32(n) : 100;
    }

    public override async Task RunAsync()
    {
        var recent = await CommandLogHelper.Instance.GetRecent(Ctx.Guild.Id, _n);
        var embed = await GetEmbed(recent);

        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    #region Instance methods

    private async Task<DiscordEmbed> GetEmbed(IReadOnlyCollection<CommandLogLocal> logs)
    {
        var members = new List<DiscordMember>();
        foreach (var userId in logs.Select(x => x.UserId).Distinct())
        {
            members.Add(await Ctx.Guild.GetMemberAsync(userId));
        }

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Recent Command Logs");
        embed.WithDescription(string.Join(Environment.NewLine, logs.Select(x => GetCommandLogStr(members, x))));
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    private string GetCommandLogStr(List<DiscordMember> members, CommandLogBase log)
    {
        var member = members.FirstOrDefault(x => x.Id == log.UserId);
        var runAt = Formatter.Timestamp(log.RunAt, TimestampFormat.ShortDateTime);
        var memberStr = member != null ? member.Mention : log.UserId.ToString();
        return $"{runAt} • {memberStr} • {log.Command}";
    }

    #endregion
}