using System.ComponentModel.DataAnnotations.Schema;
using Db.Statics;

namespace Db.Models;

public class Config
{
    public int Id { get; set; }

    public int ConfigOptionId { get; set; } // statics
    [NotMapped] private ConfigOption ConfigOption => ConfigOptions.Instance.Get(ConfigOptionId);

    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;

    public string Value { get; set; } = null!;
}