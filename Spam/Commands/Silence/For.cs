using Common.Classes;
using Common.Extensions;
using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Spam.Enums;

namespace Spam.Commands.Silence;

public class For : SlashCommand
{
    private readonly SilenceDurationKind _kind;
    private readonly DiscordMember _member;
    private readonly long _n;

    public For(InteractionContext ctx, DiscordMember member, long n, SilenceDurationKind kind) : base(ctx)
    {
        _member = member;
        _n = n;
        _kind = kind;
    }

    public override async Task RunAsync()
    {
        var until = GetDateTime();

        var silenceRole = await ConfigHelper.Instance.GetRole(Config.Roles.Silence.Name, Ctx.Guild);

        if (silenceRole == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("No silence role.",
                    "Please make sure to configure the silence role using /configure"));
            return;
        }

        if (_member.Roles.Select(x => x.Id).Contains(silenceRole.Id))
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed($"{_member.DisplayName} is already silenced."));
            return;
        }

        await _member.GrantRoleAsync(silenceRole);
        until = SilenceHelper.Instance.AddTimedSilence(_member, until);

        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed(until)));
    }

    #region Instance methods

    private DateTime GetDateTime()
    {
        return _kind switch
        {
            SilenceDurationKind.Minutes => DateTime.Now.AddMinutes(_n),
            SilenceDurationKind.Hours => DateTime.Now.AddHours(_n),
            SilenceDurationKind.Days => DateTime.Now.AddDays(_n),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private DiscordEmbed GetEmbed(DateTime until)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Member Silenced");
        var tsFull = Formatter.Timestamp(until, TimestampFormat.ShortDateTime);
        var tsRel = Formatter.Timestamp(until);
        embed.WithDescription($"{_member.DisplayName} was silenced until {tsFull}, {tsRel}.");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}