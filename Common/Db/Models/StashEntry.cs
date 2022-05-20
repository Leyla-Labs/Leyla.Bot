namespace Common.Db.Models;

public class StashEntry
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public int StashId { get; set; }
    public Stash Stash { get; set; } = null!;
}