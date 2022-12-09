using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace Common.Extensions;

public static class DiscordInteractionExtension
{
    public static async Task<DiscordMember?> GetMemberAsync(this DiscordInteraction interaction, ulong userId)
    {
        var member = interaction.Guild.Members.FirstOrDefault(x => x.Key == userId).Value;

        if (member != null)
        {
            return member;
        }

        try
        {
            return await interaction.Guild.GetMemberAsync(userId);
        }
        catch (Exception e)
        {
            if (e is NotFoundException)
            {
                return null;
            }

            throw;
        }
    }
}