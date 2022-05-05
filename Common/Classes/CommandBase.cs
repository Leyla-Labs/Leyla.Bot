using Db;

namespace Common.Classes;

public abstract class CommandBase
{
    protected readonly DatabaseContext DbCtx;

    internal CommandBase()
    {
        DbCtx = new DatabaseContext();
    }

    public abstract Task RunAsync();
}