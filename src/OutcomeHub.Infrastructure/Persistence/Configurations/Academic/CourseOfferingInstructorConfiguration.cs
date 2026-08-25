using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CourseOfferingInstructorConfiguration : IEntityTypeConfiguration<CourseOfferingInstructor>
{
    public void Configure(EntityTypeBuilder<CourseOfferingInstructor> builder)
    {
        builder.ToTable("course_offering_instructor", "academic", table =>
            table.HasCheckConstraint("ck_course_offering_instructor_range", "effective_to IS NULL OR effective_to > effective_from"));
        builder.HasKey(x => x.Id).HasName("pk_course_offering_instructor");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.CourseOfferingId).HasColumnName("course_offering_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.StaffId).HasColumnName("staff_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AssignmentRole).HasColumnName("assignment_role").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").HasColumnType("boolean").IsRequired();
        builder.HasIndex(x => new { x.CourseOfferingId, x.StaffId, x.AssignmentRole, x.EffectiveFrom }).IsUnique().HasDatabaseName("uq_course_offering_instructor_assignment");
        builder.HasOne(x => x.CourseOffering).WithMany(x => x.Instructors).HasForeignKey(x => x.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_offering_instructor_offering");
        builder.HasOne(x => x.Staff).WithMany().HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_offering_instructor_staff");
    }
}
