using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Common.Extensions;

public static class BaseContextExtensions
{
    public static async Task<DiscordMember?> GetMemberAsync(this BaseContext ctx, ulong userId)
    {
        var member = ctx.Guild.Members.FirstOrDefault(x => x.Key == userId).Value;

        return member == null
            ? await ctx.Guild.GetMemberAsync(userId)
            : member;
    }

    public static async Task<string> GetDisplayNameAsync(this BaseContext ctx, ulong userId)
    {
        var member = await ctx.GetMemberAsync(userId);
        return member?.DisplayName ?? userId.ToString();
    }
}