using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollment", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_enrollment");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AttemptNo)
            .HasColumnName("attempt_no")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.SourceSystemId)
            .HasColumnName("source_system_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SourceRecordId)
            .HasColumnName("source_record_id")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.CourseOfferingId, entity.StudentId, entity.AttemptNo })
            .IsUnique()
            .HasDatabaseName("uq_enrollment_1");

        builder.HasIndex(entity => new { entity.Id, entity.StudentId, entity.CourseOfferingId, entity.AttemptNo })
            .IsUnique()
            .HasDatabaseName("uq_enrollment_2");

        builder.HasIndex(entity => new { entity.SourceSystemId, entity.SourceRecordId })
            .IsUnique()
            .HasDatabaseName("uq_enrollment_3");

        builder.ToTable(table => table.HasCheckConstraint("ck_enrollment_attempt_no", "attempt_no > 0"));
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_enrollment_course_offering");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_enrollment_student");
        builder.HasOne(entity => entity.SourceSystem).WithMany().HasForeignKey(entity => entity.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_enrollment_source_system");
    }
}
