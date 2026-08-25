using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotDirectPiWeightConfiguration : IEntityTypeConfiguration<SnapshotDirectPiWeight>
{
    public void Configure(EntityTypeBuilder<SnapshotDirectPiWeight> builder)
    {
        builder.ToTable("snapshot_direct_pi_weight", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.SyllabusTraceabilityId })
            .HasName("pk_snapshot_direct_pi_weight");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SyllabusTraceabilityId)
            .HasColumnName("syllabus_traceability_id")
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

        builder.Property(entity => entity.RubricCriterionId)
            .HasColumnName("rubric_criterion_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DirectWeightRatio)
            .HasColumnName("direct_weight_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.AllocationRatio)
            .HasColumnName("allocation_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(false);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_snapshot_direct_pi_weight", "direct_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND direct_weight_ratio > 0 AND direct_weight_ratio <= 1");
            table.HasCheckConstraint("ck_snapshot_direct_pi_allocation", "allocation_ratio IS NULL OR allocation_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND allocation_ratio > 0 AND allocation_ratio <= 1");
        });
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_direct_pi_weight_input_snapshot");
        builder.HasOne(entity => entity.SyllabusTraceability).WithMany().HasForeignKey(entity => entity.SyllabusTraceabilityId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_direct_pi_weight_traceability");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_direct_pi_weight_program_pi");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_direct_pi_weight_offering");
        builder.HasOne(entity => entity.RubricCriterion).WithMany().HasForeignKey(entity => entity.RubricCriterionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_direct_pi_weight_criterion");
    }
}
