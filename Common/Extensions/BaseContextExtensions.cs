using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Common.Extensions;

public static class InteractionContextExtensions
{
    public static async Task<DiscordMember?> GetMember(this BaseContext ctx, ulong userId)
    {
        var member = ctx.Guild.Members.FirstOrDefault(x => x.Key == userId).Value;

        return member == null
            ? await ctx.Guild.GetMemberAsync(userId)
            : member;
    }
}