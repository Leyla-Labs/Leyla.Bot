using DSharpPlus.SlashCommands;

namespace Common.Classes;

public abstract class SlashCommand
{
    protected readonly InteractionContext Ctx;

    protected SlashCommand(InteractionContext ctx)
    {
        Ctx = ctx;
    }

    public abstract Task RunAsync();
}