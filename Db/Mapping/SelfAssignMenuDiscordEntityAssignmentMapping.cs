using Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Db.Mapping;

public class SelfAssignMenuDiscordEntityAssignmentMapping : IEntityTypeConfiguration<SelfAssignMenuDiscordEntityAssignment>
{
    public void Configure(EntityTypeBuilder<SelfAssignMenuDiscordEntityAssignment> b)
    {
        b.HasKey(x => x.Id);

        b.HasOne(x => x.SelfAssignMenu)
            .WithMany(y => y.SelfAssignMenuDiscordEntityAssignments)
            .HasForeignKey(x => x.SelfAssignMenuId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.DiscordEntity)
            .WithMany(y => y.SelfAssignMenuDiscordEntityAssignments)
            .HasForeignKey(x => x.DiscordEntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}