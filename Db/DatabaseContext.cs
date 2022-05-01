using Db.Enums;
using Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Db;

public class DatabaseContext : DbContext
{
    // constructor for API
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        MapEnums();
    }

    // constructor for EntityRepository
    public DatabaseContext()
    {
        MapEnums();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(Configuration.ConnectionString).UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresEnum<DiscordEntityType>();
    }

    private static void MapEnums()
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<DiscordEntityType>();
    }

    #region DbSets

    public DbSet<Config> Configs => Set<Config>();
    public DbSet<DiscordEntity> DiscordEntities => Set<DiscordEntity>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<SelfAssignMenu> SelfAssignMenus => Set<SelfAssignMenu>();

    public DbSet<SelfAssignMenuDiscordEntityAssignment> SelfAssignMenuDiscordEntityAssignments =>
        Set<SelfAssignMenuDiscordEntityAssignment>();

    #endregion
}

// Necessary for using EF migrations in Db project
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var b = new DbContextOptionsBuilder<DatabaseContext>();
        b.UseNpgsql(Configuration.ConnectionString).UseSnakeCaseNamingConvention();
        return new DatabaseContext(b.Options);
    }
}