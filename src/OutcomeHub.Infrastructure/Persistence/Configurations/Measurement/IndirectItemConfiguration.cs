using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class IndirectItemConfiguration : IEntityTypeConfiguration<IndirectItem>
{
    public void Configure(EntityTypeBuilder<IndirectItem> builder)
    {
        builder.ToTable("indirect_item", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_indirect_item");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.InstrumentVersionId)
            .HasColumnName("instrument_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Code)
            .HasColumnName("code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Prompt)
            .HasColumnName("prompt")
            .HasColumnType("text")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ProgramPloId)
            .HasColumnName("program_plo_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.WeightRatio)
            .HasColumnName("weight_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.Id, entity.InstrumentVersionId, entity.ProgramVersionId })
            .IsUnique()
            .HasDatabaseName("uq_indirect_item_1");

        builder.HasIndex(entity => new { entity.InstrumentVersionId, entity.ProgramVersionId, entity.Code })
            .IsUnique()
            .HasDatabaseName("uq_indirect_item_2");

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint("ck_indirect_item_outcome", "num_nonnulls(program_pi_id, program_plo_id) = 1"));

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_indirect_item_weight", "weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND weight_ratio > 0 AND weight_ratio <= 1");
            table.HasCheckConstraint("ck_indirect_item_outcome_level_binding", "(program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (program_pi_id IS NULL AND program_plo_id IS NOT NULL)");
        });
        builder.HasOne(entity => entity.InstrumentVersion).WithMany(entity => entity.Items).HasForeignKey(entity => entity.InstrumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_item_instrument_version");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_item_program_version");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => new { entity.ProgramPiId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_item_program_pi");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => new { entity.ProgramPloId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_indirect_item_program_plo");
    }
}
