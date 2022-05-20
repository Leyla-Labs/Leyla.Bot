using Common.Enums;

namespace Common.Db.Models;

public class UserLog
{
    public int Id { get; set; }

    public ulong MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public string Reason { get; set; } = null!;
    public string AdditionalDetails { get; set; } = null!;

    public DateTime Date { get; set; }
    public UserLogType Type { get; set; }

    public ulong AuthorId { get; set; }
    public Member Author { get; set; } = null!;
}