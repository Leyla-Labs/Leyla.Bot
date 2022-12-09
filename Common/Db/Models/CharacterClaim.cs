namespace Common.Db.Models;

public class CharacterClaim
{
    public int Id { get; set; }

    public ulong UserId { get; set; }

    public int CharacterId { get; set; }

    /// <summary>
    ///     Guid with 'L-' prefix
    /// </summary>
    public string Code { get; set; } = null!;

    public bool Confirmed { get; set; }

    /// <summary>
    ///     Expiry date and time for unconfirmed claims. Confirmed claims are valid forever.
    /// </summary>
    public DateTime ValidUntil { get; set; }
}