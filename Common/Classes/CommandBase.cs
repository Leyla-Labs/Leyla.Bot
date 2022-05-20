using Common.Db;

namespace Common.Classes;

public abstract class CommandBase
{
    protected readonly DatabaseContext DbCtx;

    protected CommandBase()
    {
        DbCtx = new DatabaseContext();
    }

    public abstract Task RunAsync();
}