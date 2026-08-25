using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotIndirectObservationConfiguration : IEntityTypeConfiguration<SnapshotIndirectObservation>
{
    public void Configure(EntityTypeBuilder<SnapshotIndirectObservation> builder)
    {
        builder.ToTable("snapshot_indirect_observation", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.IndirectObservationId })
            .HasName("pk_snapshot_indirect_observation");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.IndirectObservationId)
            .HasColumnName("indirect_observation_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ItemId)
            .HasColumnName("item_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ProgramPloId)
            .HasColumnName("program_plo_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.RawValue)
            .HasColumnName("raw_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.MaxValue)
            .HasColumnName("max_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.NormalizedValue)
            .HasColumnName("normalized_value")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.SourceChecksum)
            .HasColumnName("source_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_snapshot_indirect_observation_outcome", "num_nonnulls(program_pi_id, program_plo_id) = 1");
            table.HasCheckConstraint("ck_snapshot_indirect_observation_values", "raw_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND normalized_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_value > 0 AND raw_value >= 0 AND raw_value <= max_value AND normalized_value BETWEEN 0 AND 100");
            table.HasCheckConstraint("ck_snapshot_indirect_observation_checksum", "source_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_indirect_observation_input_snapshot");
        builder.HasOne(entity => entity.IndirectObservation).WithMany().HasForeignKey(entity => entity.IndirectObservationId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_indirect_observation_observation");
        builder.HasOne(entity => entity.Item).WithMany().HasForeignKey(entity => entity.ItemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_indirect_observation_item");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_indirect_observation_program_pi");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => entity.ProgramPloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_indirect_observation_program_plo");
    }
}
