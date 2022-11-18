using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace Common.Extensions;

public static class DiscordInteractionExtension
{
    public static async Task<DiscordMember?> GetMember(this DiscordInteraction interaction, ulong userId)
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

    /// <summary>Creates a deferred response to this interaction.</summary>
    /// <param name="ctx">The InteractionContext</param>
    /// <param name="ephemeral">Whether the response should be ephemeral.</param>
    public static Task DeferAsync(this DiscordInteraction ctx, bool ephemeral = false)
    {
        return ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral(ephemeral));
    }
}