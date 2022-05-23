using System.Text;
using Common.Helper;
using Common.Strings;
using DSharpPlus;
using DSharpPlus.Entities;
using Spam.Classes;
using Spam.Extensions;
using Spam.Helper;

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
        var components = new List<DiscordButtonComponent> {GetRaidModeButton()};

        var lockdownDuration = await ConfigHelper.Instance.GetInt(Config.Raid.LockdownDuration.Name, guild.Id);

        if (lockdownDuration > 0)
        {
            components.Add(GetLockdownButton(guild));
            await RaidHelper.Instance.EnableLockdown(guild, lockdownDuration.Value);
            embed = AddLockdownToDescription(embed, guild, lockdownDuration.Value);
        }

        await modChannel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(components));
    }

    private static DiscordEmbed GetEmbed(IEnumerable<DiscordMember> raidMembers)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Possible Raid Detected");
        embed.WithDescription(string.Join(Environment.NewLine, raidMembers.Select(x => x.GetMemberRaidString())));
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }


    private static DiscordButtonComponent GetRaidModeButton()
    {
        var customId = ModalHelper.GetModalName(1, "raidMode", new[] {"True"});
        return new DiscordButtonComponent(ButtonStyle.Danger, customId, "Enable Raid Mode");
    }

    private static DiscordEmbed AddLockdownToDescription(DiscordEmbed embed, DiscordGuild guild, int lockdownDuration)
    {
        var embedBuilder = new DiscordEmbedBuilder(embed);
        var sb = new StringBuilder(embedBuilder.Description);
        sb.Append($"{Environment.NewLine}{Environment.NewLine}");
        sb.Append("Lockdown has been enabled. ");
        sb.Append($"For the next {lockdownDuration} minutes, ");
        sb.Append($"the verification level will be set to {guild.VerificationLevel}.");
        embedBuilder.WithDescription(sb.ToString());
        return embedBuilder.Build();
    }

    private static DiscordButtonComponent GetLockdownButton(DiscordGuild guild)
    {
        var vLvl = ((int) guild.VerificationLevel).ToString();
        var customId = ModalHelper.GetModalName(1, "disableLockdown", new[] {vLvl});
        return new DiscordButtonComponent(ButtonStyle.Secondary, customId, "Disable Lockdown");
    }
}