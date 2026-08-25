using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotOfferingConfiguration : IEntityTypeConfiguration<SnapshotOffering>
{
    public void Configure(EntityTypeBuilder<SnapshotOffering> builder)
    {
        builder.ToTable("snapshot_offering", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.CourseOfferingId })
            .HasName("pk_snapshot_offering");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramCourseId)
            .HasColumnName("program_course_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseVersionId)
            .HasColumnName("course_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SyllabusVersionId)
            .HasColumnName("syllabus_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CurriculumPathId)
            .HasColumnName("curriculum_path_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.SourceRole)
            .HasColumnName("source_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_offering_input_snapshot");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_offering_course_offering");
        builder.HasOne(entity => entity.ProgramCourse).WithMany().HasForeignKey(entity => entity.ProgramCourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_offering_program_course");
        builder.HasOne(entity => entity.CourseVersion).WithMany().HasForeignKey(entity => entity.CourseVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_offering_course_version");
        builder.HasOne(entity => entity.SyllabusVersion).WithMany().HasForeignKey(entity => new { entity.SyllabusVersionId, entity.ProgramCourseId, entity.CourseVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramCourseId, entity.CourseVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_offering_syllabus_binding");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_offering_curriculum_path");
    }
}
