using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class IndirectInstrumentConfiguration : IEntityTypeConfiguration<IndirectInstrument>
{
    public void Configure(EntityTypeBuilder<IndirectInstrument> builder)
    {
        builder.ToTable("indirect_instrument", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_indirect_instrument");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.Code)
            .HasColumnName("code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.Property(entity => entity.OwnerOrgUnitId)
            .HasColumnName("owner_org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.HasIndex(entity => entity.Code)
            .IsUnique()
            .HasDatabaseName("uq_indirect_instrument_1");

        builder.ToTable(table => table.HasCheckConstraint("ck_indirect_instrument_code", "code = upper(btrim(code)) AND char_length(code) > 0"));
        builder.HasOne(entity => entity.OwnerOrgUnit).WithMany().HasForeignKey(entity => entity.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_instrument_owner_org_unit");
    }
}
