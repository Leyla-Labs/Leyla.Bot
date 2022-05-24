using Common.Classes;
using Common.Extensions;
using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.Moderation;

public class Silence : SlashCommand
{
    private readonly DiscordMember _member;

    public Silence(InteractionContext ctx, DiscordMember member) : base(ctx)
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

        if (_member.Roles.Select(x => x.Id).Contains(silenceRole.Id))
        {
            await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddErrorEmbed($"{_member.DisplayName} is already silenced."));
            return;
        }

        await _member.GrantRoleAsync(silenceRole);

        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed()));
    }

    #region Instance methods

    private DiscordEmbed GetEmbed()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Member Silenced");
        embed.WithDescription($"{_member.DisplayName} was silenced.");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}