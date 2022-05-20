using Common.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Db.Mapping;

public class ConfigMapping : IEntityTypeConfiguration<Config>
{
    public void Configure(EntityTypeBuilder<Config> b)
    {
        b.HasKey(x => x.Id);

        b.HasOne(x => x.Guild)
            .WithMany(y => y.Configs)
            .HasForeignKey(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}