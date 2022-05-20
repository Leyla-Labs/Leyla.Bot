namespace Common.Db.Models;

public class Quote
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;
    public DateTime Date { get; set; }

    // TODO add FK once Message is its own entity
    public ulong MessageId { get; set; }

    public ulong MemberId { get; set; }
    public Member Member { get; set; } = null!;
}