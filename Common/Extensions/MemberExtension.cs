using Db.Helper;
using DSharpPlus.Entities;

namespace Common.Extensions;

public static class MemberExtension
{
    public static async Task CreateInDbIfNotExist(this DiscordMember member)
    {
        await MemberHelper.CreateIfNotExist(member.Id, member.Guild.Id);
    }
}