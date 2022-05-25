using Common.Helper;
using Common.Strings;
using DNTCommon.Web.Core;

namespace Main.BackgroundTasks;

public class CheckSilences : IScheduledTask
{
    public bool IsShuttingDown { get; set; }

    public async Task RunAsync()
    {
        if (IsShuttingDown)
        {
            return;
        }

        var membersToUnsilence = SilenceHelper.Instance.GetMembersToUnsilence();

        if (!membersToUnsilence.Any())
        {
            return;
        }

        foreach (var member in membersToUnsilence)
        {
            var role = await ConfigHelper.Instance.GetRole(Config.Roles.Silence.Name, member.Guild);

            var updatedMember = await member.Guild.GetMemberAsync(member.Id);
            if (role != null && updatedMember != null && updatedMember.Roles.Select(x => x.Id).Contains(role.Id))
            {
                await member.RevokeRoleAsync(role);
            }

            SilenceHelper.Instance.RemoveTimedSilence(member.Guild.Id, member.Id);
        }
    }
}