using Common.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Db.Mapping;

public class CommandLogMapping : IEntityTypeConfiguration<CommandLog>
{
    public void Configure(EntityTypeBuilder<CommandLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Member)
            .WithMany(x => x.CommandLogs)
            .HasForeignKey(x => new {x.UserId, x.GuildId})
            .OnDelete(DeleteBehavior.Cascade);
    }
}