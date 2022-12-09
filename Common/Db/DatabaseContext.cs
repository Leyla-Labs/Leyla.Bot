using Common.Db.Models;
using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Common.Db;

public class DatabaseContext : DbContext
{
    static DatabaseContext()
    {
        MapEnums();
    }

    // constructor for API
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    // constructor for EntityRepository
    public DatabaseContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var conn = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;
            optionsBuilder.UseNpgsql(conn).UseSnakeCaseNamingConvention();
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresEnum<DiscordEntityType>();
        builder.HasPostgresEnum<UserLogType>();

        builder.HasPostgresExtension("uuid-ossp");
        builder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }

    private static void MapEnums()
    {
#pragma warning disable CS0618
        // marked as obsolete, but no alternative for use with EFCore provided yet.
        // https://github.com/npgsql/efcore.pg/issues/2435#issuecomment-1311920314
        NpgsqlConnection.GlobalTypeMapper.MapEnum<DiscordEntityType>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<UserLogType>();
#pragma warning restore CS0618
    }

    #region DbSets

    public DbSet<CharacterClaim> CharacterClaims => Set<CharacterClaim>();
    public DbSet<CommandLog> CommandLogs => Set<CommandLog>();
    public DbSet<Config> Configs => Set<Config>();
    public DbSet<DiscordEntity> DiscordEntities => Set<DiscordEntity>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<SelfAssignMenu> SelfAssignMenus => Set<SelfAssignMenu>();
    public DbSet<UserLog> UserLogs => Set<UserLog>();

    public DbSet<SelfAssignMenuDiscordEntityAssignment> SelfAssignMenuDiscordEntityAssignments =>
        Set<SelfAssignMenuDiscordEntityAssignment>();

    public DbSet<Stash> Stashes => Set<Stash>();
    public DbSet<StashEntry> StashEntries => Set<StashEntry>();

    #endregion
}

// Necessary for using EF migrations in Db project
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var b = new DbContextOptionsBuilder<DatabaseContext>();
        var conn = "Host=localhost;Database=leyla_dev;Username=tawmy";
        b.UseNpgsql(conn).UseSnakeCaseNamingConvention();
        return new DatabaseContext(b.Options);
    }
}