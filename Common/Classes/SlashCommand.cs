using DSharpPlus.SlashCommands;

namespace Common.Classes;

public abstract class SlashCommand : CommandBase
{
    protected readonly InteractionContext Ctx;

    protected SlashCommand(InteractionContext ctx)
    {
        Ctx = ctx;
    }
}