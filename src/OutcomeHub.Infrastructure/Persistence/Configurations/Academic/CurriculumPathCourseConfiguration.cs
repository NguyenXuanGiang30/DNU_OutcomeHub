using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CurriculumPathCourseConfiguration : IEntityTypeConfiguration<CurriculumPathCourse>
{
    public void Configure(EntityTypeBuilder<CurriculumPathCourse> builder)
    {
        builder.ToTable("curriculum_path_course", "academic", table =>
        {
            table.HasCheckConstraint("ck_curriculum_path_course_term", "planned_term IS NULL OR planned_term > 0");
            table.HasCheckConstraint("ck_curriculum_path_course_requirement", "requirement_type IN ('REQUIRED','ELECTIVE','OPTIONAL','SUBSTITUTE')");
            table.HasCheckConstraint("ck_curriculum_path_course_elective_group", "requirement_type = 'ELECTIVE' OR elective_group_id IS NULL");
            table.HasCheckConstraint("ck_curriculum_path_course_sort_order", "sort_order >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_curriculum_path_course");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ProgramCourseId).HasColumnName("program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.PlannedTerm).HasColumnName("planned_term").HasColumnType("integer").IsRequired(false);
        builder.Property(x => x.RequirementType).HasColumnName("requirement_type").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.ElectiveGroupId).HasColumnName("elective_group_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();
        builder.HasIndex(x => new { x.CurriculumPathId, x.ProgramCourseId, x.ElectiveGroupId }).IsUnique().AreNullsDistinct(false).HasDatabaseName("uq_curriculum_path_course_member");
        builder.HasOne(x => x.CurriculumPath).WithMany().HasForeignKey(x => x.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_path_course_path");
        builder.HasOne(x => x.ProgramCourse).WithMany().HasForeignKey(x => x.ProgramCourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_path_course_program_course");
        builder.HasOne(x => x.ElectiveGroup).WithMany().HasForeignKey(x => x.ElectiveGroupId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_path_course_elective_group");
    }
}
