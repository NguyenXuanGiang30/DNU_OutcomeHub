using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotPiSourceWeightConfiguration : IEntityTypeConfiguration<SnapshotPiSourceWeight>
{
    public void Configure(EntityTypeBuilder<SnapshotPiSourceWeight> builder)
    {
        builder.ToTable("snapshot_pi_source_weight", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.StudentPathId, entity.ProgramPiId, entity.CourseOfferingId })
            .HasName("pk_snapshot_pi_source_weight");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentPathId)
            .HasColumnName("student_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SourceWeightRatio)
            .HasColumnName("source_weight_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.SourceRole)
            .HasColumnName("source_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.AnchorAssessmentId)
            .HasColumnName("anchor_assessment_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_snapshot_pi_source_weight", "source_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND source_weight_ratio > 0 AND source_weight_ratio <= 1"));
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_source_weight_input_snapshot");
        builder.HasOne(entity => entity.StudentPath).WithMany().HasForeignKey(entity => entity.StudentPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_source_weight_student_path");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_source_weight_program_pi");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_source_weight_offering");
        builder.HasOne(entity => entity.AnchorAssessment).WithMany().HasForeignKey(entity => entity.AnchorAssessmentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_pi_source_weight_anchor");
    }
}
