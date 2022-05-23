using Common.Classes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Spam.Extensions;
using Spam.Helper;

namespace Spam.Commands.Raid;

public class List : SlashCommand
{
    public List(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var raidMembers = await RaidHelper.Instance.GetRaidMembers(Ctx.Guild.Id);
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed(raidMembers)));
    }

    #region Static members

    private static DiscordEmbed GetEmbed(List<DiscordMember>? raidMembers)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Active Raid");
        embed.WithDescription(raidMembers != null
            ? string.Join(Environment.NewLine, raidMembers.Select(x => x.GetMemberRaidString()))
            : "No active raid.");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}