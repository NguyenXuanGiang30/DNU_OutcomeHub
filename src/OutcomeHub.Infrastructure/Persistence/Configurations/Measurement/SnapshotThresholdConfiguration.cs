using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotThresholdConfiguration : IEntityTypeConfiguration<SnapshotThreshold>
{
    public void Configure(EntityTypeBuilder<SnapshotThreshold> builder)
    {
        builder.ToTable("snapshot_threshold", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.OutcomeLevel, entity.OutcomeKey })
            .HasName("pk_snapshot_threshold");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OutcomeLevel)
            .HasColumnName("outcome_level")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.OutcomeKey)
            .HasColumnName("outcome_key")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CloId)
            .HasColumnName("clo_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ProgramPloId)
            .HasColumnName("program_plo_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ThetaInd)
            .HasColumnName("theta_ind")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.ThetaCoh)
            .HasColumnName("theta_coh")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.NearThreshold)
            .HasColumnName("near_threshold")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.MinSampleSize)
            .HasColumnName("min_sample_size")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.ThresholdSource)
            .HasColumnName("threshold_source")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint("ck_snapshot_threshold_outcome", "num_nonnulls(clo_id, program_pi_id, program_plo_id) = 1"));

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_snapshot_threshold_level", "outcome_level IN ('CLO','PI','PLO')");
            table.HasCheckConstraint("ck_snapshot_threshold_shape", "(outcome_level = 'CLO' AND outcome_key = clo_id AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND outcome_key = program_pi_id AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND outcome_key = program_plo_id AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)");
            table.HasCheckConstraint("ck_snapshot_threshold_values", "theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind BETWEEN 0 AND 100 AND theta_coh BETWEEN 0 AND 100 AND (near_threshold IS NULL OR near_threshold NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND near_threshold BETWEEN 0 AND 100) AND min_sample_size > 0");
        });
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_threshold_input_snapshot");
        builder.HasOne(entity => entity.Clo).WithMany().HasForeignKey(entity => entity.CloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_threshold_clo");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_threshold_program_pi");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => entity.ProgramPloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_threshold_program_plo");
    }
}
