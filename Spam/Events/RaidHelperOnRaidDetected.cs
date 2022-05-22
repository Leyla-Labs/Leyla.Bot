using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;
using Spam.Extensions;

namespace Spam.Events;

internal static class RaidHelperOnRaidDetected
{
    public static async void HandleEvent(DiscordClient sender, RaidDetectedEventArgs args)
    {
        var guild = args.RaidMembers[0].Guild;

        var modChannel = await ConfigHelper.Instance.GetChannel(Config.Channels.Mod.Name, guild);

        if (modChannel == null)
        {
            // TODO handle this better, possibly DM server owner
            // should be part of error sending method that'll be implemented at a later date
            return;
        }

        var embed = GetEmbed(args.RaidMembers);
        var button = GetRaidModeButton();

        await modChannel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(button));
    }

    private static DiscordEmbed GetEmbed(List<DiscordMember> raidMembers)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Possible Raid Detected");
        embed.WithDescription(string.Join(Environment.NewLine, raidMembers.Select(x => x.GetMemberRaidString())));
        return embed.Build();
    }


    private static DiscordButtonComponent GetRaidModeButton()
    {
        var customId = ModalHelper.GetModalName(1, "raidMode", new[] {"True"});
        return new DiscordButtonComponent(ButtonStyle.Danger, customId, "Enable Raid Mode");
    }
}