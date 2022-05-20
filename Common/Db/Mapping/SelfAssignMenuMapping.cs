using Common.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Db.Mapping;

public class SelfAssignMenuMapping : IEntityTypeConfiguration<SelfAssignMenu>
{
    public void Configure(EntityTypeBuilder<SelfAssignMenu> b)
    {
        b.HasKey(x => x.Id);

        b.HasOne(x => x.Guild)
            .WithMany(y => y.SelfAssignMenus)
            .HasForeignKey(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.RequiredRole)
            .WithMany(y => y.SelfAssignMenus)
            .HasForeignKey(x => x.RequiredRoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}