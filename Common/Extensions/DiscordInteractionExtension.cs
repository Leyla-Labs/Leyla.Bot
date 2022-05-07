using DSharpPlus.Entities;

namespace Common.Extensions;

public static class DiscordInteractionExtension
{
    public static async Task<DiscordMember?> GetMember(this DiscordInteraction interaction, ulong userId)
    {
        var member = interaction.Guild.Members.FirstOrDefault(x => x.Key == userId).Value;

        return member == null
            ? await interaction.Guild.GetMemberAsync(userId)
            : member;
    }
}