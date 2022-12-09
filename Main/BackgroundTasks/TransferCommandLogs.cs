using Common.Helper;
using DNTCommon.Web.Core;

namespace Main.BackgroundTasks;

public class TransferCommandLogs : IScheduledTask
{
    public bool IsShuttingDown { get; set; }

    public async Task RunAsync()
    {
        if (IsShuttingDown)
        {
            return;
        }

        await CommandLogHelper.Instance.TransferToDbAsync();
    }
}