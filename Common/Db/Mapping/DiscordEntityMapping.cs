using Common.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Db.Mapping;

public class DiscordEntityMapping : IEntityTypeConfiguration<DiscordEntity>
{
    public void Configure(EntityTypeBuilder<DiscordEntity> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.HasOne(x => x.Guild)
            .WithMany(y => y.DiscordEntities)
            .HasForeignKey(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}