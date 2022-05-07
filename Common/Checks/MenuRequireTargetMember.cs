using DSharpPlus.SlashCommands;

namespace Common.Checks;

public class MenuRequireTargetMember : ContextMenuCheckBaseAttribute
{
    public override Task<bool> ExecuteChecksAsync(ContextMenuContext ctx)
    {
        return Task.FromResult(ctx.TargetMember != null);
    }
}