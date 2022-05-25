using System.Text;
using Common.Classes;
using Common.Extensions;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Spam.Extensions;
using Spam.Helper;

namespace Spam.Commands.Raid;

internal sealed class Ban : SlashCommand
{
    public Ban(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var raidMembers = await RaidHelper.Instance.GetRaidMembers(Ctx.Guild.Id);

        if (raidMembers == null)
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("No active raid."));
            return;
        }

        // ask user if they are certain they want to ban everyone
        var embed = GetWarningEmbed(raidMembers);
        var button = GetConfirmationButton();
        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AddComponents(button));

        // wait for button press
        var interactivity = Ctx.Client.GetInteractivity();
        var message = await Ctx.GetOriginalResponseAsync();
        var userResponse = await interactivity.WaitForButtonAsync(message, button.CustomId);
        if (userResponse.TimedOut || userResponse.Result.User.Id != Ctx.User.Id)
        {
            // TODO check if this behaves as code suggests
            return;
        }

        // show user the bot is working on the bans
        await userResponse.Result.Interaction.CreateResponseAsync(InteractionResponseType
            .DeferredChannelMessageWithSource);

        // ban everyone from the raid
        foreach (var raidMember in raidMembers)
        {
            await raidMember.BanAsync(1, $"Ban raid command ran by {Ctx.User.Username}#{Ctx.User.Discriminator}");
        }

        // show confirmation
        var confirmationEmbed = GetConfirmationEmbed(raidMembers);
        await userResponse.Result.Interaction.EditOriginalResponseAsync(
            new DiscordWebhookBuilder().AddEmbed(confirmationEmbed));
    }

    #region Instance methods

    private DiscordButtonComponent GetConfirmationButton()
    {
        var buttonName = ModalHelper.GetModalName(Ctx.User.Id, "banRaid");
        return new DiscordButtonComponent(ButtonStyle.Danger, buttonName, "Ban everyone from this list");
    }

    #endregion

    #region Static methods

    private static DiscordEmbed GetWarningEmbed(IEnumerable<DiscordMember> raidMembers)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Ban Raid Members");
        var sb = new StringBuilder();
        sb.Append("Are you **absolutely sure** you want to ban everyone from this list?");
        sb.Append($"{Environment.NewLine}{Environment.NewLine}");
        sb.Append(string.Join(Environment.NewLine, raidMembers.Select(x => x.GetMemberRaidString())));
        embed.WithDescription(sb.ToString());
        embed.WithColor(DiscordColor.Red);
        return embed.Build();
    }

    private static DiscordEmbed GetConfirmationEmbed(IEnumerable<DiscordMember> raidMembers)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Raid Members Banned");
        var sb = new StringBuilder();
        sb.Append("The following members have been banned:");
        sb.Append($"{Environment.NewLine}{Environment.NewLine}");
        sb.Append(string.Join(Environment.NewLine, raidMembers.Select(x => x.GetMemberRaidString())));
        embed.WithDescription(sb.ToString());
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}