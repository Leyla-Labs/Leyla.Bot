using Common.Classes;
using Common.Extensions;
using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.Moderation;

internal sealed class Unsilence : SlashCommand
{
    private readonly DiscordMember _member;

    public Unsilence(InteractionContext ctx, DiscordMember member) : base(ctx)
    {
        _member = member;
    }

    public override async Task RunAsync()
    {
        var silenceRole = await ConfigHelper.Instance.GetRole(Config.Roles.Silence.Name, Ctx.Guild);

        if (silenceRole == null)
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed("No silence role.",
                    "Please make sure to configure the silence role using /configure"));
            return;
        }

        if (!_member.Roles.Select(x => x.Id).Contains(silenceRole.Id))
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed($"{_member.DisplayName} is not silenced."));
            return;
        }

        await _member.RevokeRoleAsync(silenceRole);

        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed()));
    }

    #region Instance methods

    private DiscordEmbed GetEmbed()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Member Unsilenced");
        embed.WithDescription($"{_member.DisplayName} was unsilenced.");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}