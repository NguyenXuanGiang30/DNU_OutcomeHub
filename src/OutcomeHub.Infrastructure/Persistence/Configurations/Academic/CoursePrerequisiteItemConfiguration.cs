using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CoursePrerequisiteItemConfiguration : IEntityTypeConfiguration<CoursePrerequisiteItem>
{
    public void Configure(EntityTypeBuilder<CoursePrerequisiteItem> builder)
    {
        builder.ToTable("course_prerequisite_item", "academic", table =>
            table.HasCheckConstraint("ck_course_prerequisite_item_grade", "minimum_grade IS NULL OR (minimum_grade >= 0 AND minimum_grade <= 100 AND minimum_grade <> 'NaN'::numeric AND minimum_grade NOT IN ('Infinity'::numeric, '-Infinity'::numeric))"));
        builder.HasKey(x => new { x.GroupId, x.RequiredProgramCourseId }).HasName("pk_course_prerequisite_item");
        builder.Property(x => x.GroupId).HasColumnName("group_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RequiredProgramCourseId).HasColumnName("required_program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.MinimumGrade).HasColumnName("minimum_grade").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired(false);
        builder.Property(x => x.AllowConcurrent).HasColumnName("allow_concurrent").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.Rationale).HasColumnName("rationale").HasColumnType("text").IsRequired(false);
        builder.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_prerequisite_item_group");
        builder.HasOne(x => x.RequiredProgramCourse).WithMany().HasForeignKey(x => x.RequiredProgramCourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_prerequisite_item_required_course");
    }
}
