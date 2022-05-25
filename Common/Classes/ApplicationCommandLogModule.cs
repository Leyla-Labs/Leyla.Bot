using Common.Helper;
using DSharpPlus.SlashCommands;

namespace Common.Classes;

public class ApplicationCommandLogModule : ApplicationCommandModule
{
    public override Task<bool> BeforeSlashExecutionAsync(InteractionContext ctx)
    {
        CommandLogHelper.Instance.AddLog(ctx);
        return base.BeforeSlashExecutionAsync(ctx);
    }
}