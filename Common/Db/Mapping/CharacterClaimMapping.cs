using Common.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Db.Mapping;

public class CharacterClaimMapping : IEntityTypeConfiguration<CharacterClaim>
{
    public void Configure(EntityTypeBuilder<CharacterClaim> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.CharacterId).IsUnique();

        builder.HasIndex(x => x.Code).IsUnique();
    }
}