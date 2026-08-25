using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotPiPloWeightConfiguration : IEntityTypeConfiguration<SnapshotPiPloWeight>
{
    public void Configure(EntityTypeBuilder<SnapshotPiPloWeight> builder)
    {
        builder.ToTable("snapshot_pi_plo_weight", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.ProgramPiId, entity.ProgramPloId })
            .HasName("pk_snapshot_pi_plo_weight");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPloId)
            .HasColumnName("program_plo_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PiWeightRatio)
            .HasColumnName("pi_weight_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.IsCore)
            .HasColumnName("is_core")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.SourceProgramPiId)
            .HasColumnName("source_program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_snapshot_pi_plo_weight", "pi_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND pi_weight_ratio > 0 AND pi_weight_ratio <= 1"));
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_plo_weight_input_snapshot");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_plo_weight_program_pi");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => entity.ProgramPloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_plo_weight_program_plo");
        builder.HasOne(entity => entity.SourceProgramPi).WithMany().HasForeignKey(entity => entity.SourceProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_plo_weight_source_pi");
    }
}
