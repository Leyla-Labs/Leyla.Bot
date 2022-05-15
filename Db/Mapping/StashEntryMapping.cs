using Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Db.Mapping;

public class StashEntryMapping : IEntityTypeConfiguration<StashEntry>
{
    public void Configure(EntityTypeBuilder<StashEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Stash)
            .WithMany(y => y.StashEntries)
            .HasForeignKey(x => x.StashId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}