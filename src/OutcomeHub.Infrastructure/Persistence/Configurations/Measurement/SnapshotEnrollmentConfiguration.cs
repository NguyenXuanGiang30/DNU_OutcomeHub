using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotEnrollmentConfiguration : IEntityTypeConfiguration<SnapshotEnrollment>
{
    public void Configure(EntityTypeBuilder<SnapshotEnrollment> builder)
    {
        builder.ToTable("snapshot_enrollment", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.EnrollmentRevisionId })
            .HasName("pk_snapshot_enrollment");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EnrollmentRevisionId)
            .HasColumnName("enrollment_revision_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AttemptNo)
            .HasColumnName("attempt_no")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.RevisionNo)
            .HasColumnName("revision_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_snapshot_enrollment_attempt", "attempt_no > 0");
            table.HasCheckConstraint("ck_snapshot_enrollment_revision", "revision_no > 0");
        });
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_enrollment_input_snapshot");
        builder.HasOne(entity => entity.EnrollmentRevision).WithMany().HasForeignKey(entity => entity.EnrollmentRevisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_enrollment_revision");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_enrollment_student");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_enrollment_course_offering");
    }
}
