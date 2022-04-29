using Db.Models;

namespace Db;

public static class Configuration
{
    public static string ConnectionString { get; set; } = "Host=localhost;Database=leyla_dev;Username=tawmy";

    public static Dictionary<ulong, List<Config>> GuildConfigs { get; set; } = null!;
}