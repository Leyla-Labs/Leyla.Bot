using Db.Enums;

namespace Db.Models;

public class UserLog
{
    public int Id { get; set; }

    public ulong MemberId { get; set; }
    public Member Member { get; set; } = null!;
    
    public string Text { get; set; } = null!;
    public DateTime Date { get; set; }
    public UserLogType Type { get; set; }

    public ulong AuthorId { get; set; }
    public Member Author { get; set; } = null!;
}