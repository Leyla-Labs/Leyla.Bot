using Common.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Db.Mapping;

public class StashMapping : IEntityTypeConfiguration<Stash>
{
    public void Configure(EntityTypeBuilder<Stash> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Guild)
            .WithMany(y => y.Stashes)
            .HasForeignKey(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RequiredRole)
            .WithMany(y => y.Stashes)
            .HasForeignKey(x => x.RequiredRoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}