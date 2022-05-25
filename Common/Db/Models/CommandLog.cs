using Common.Classes;

namespace Common.Db.Models;

public class CommandLog : CommandLogBase
{
    public int Id { get; set; }

    public Member Member { get; set; } = null!;
}