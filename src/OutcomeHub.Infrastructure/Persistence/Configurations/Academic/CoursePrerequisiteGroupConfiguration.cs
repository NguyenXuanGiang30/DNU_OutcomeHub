using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CoursePrerequisiteGroupConfiguration : IEntityTypeConfiguration<CoursePrerequisiteGroup>
{
    public void Configure(EntityTypeBuilder<CoursePrerequisiteGroup> builder)
    {
        builder.ToTable("course_prerequisite_group", "academic", table =>
        {
            table.HasCheckConstraint("ck_course_prerequisite_group_no", "group_no > 0");
            table.HasCheckConstraint("ck_course_prerequisite_group_minimum", "minimum_items_satisfied > 0");
            table.HasCheckConstraint("ck_course_prerequisite_group_relation", "relation_type IN ('ALL','ANY','AT_LEAST')");
        });
        builder.HasKey(x => x.Id).HasName("pk_course_prerequisite_group");
        builder.HasAlternateKey(x => new { x.Id, x.ProgramVersionId }).HasName("uq_course_prerequisite_group_id_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.TargetProgramCourseId).HasColumnName("target_program_course_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.GroupNo).HasColumnName("group_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.MinimumItemsSatisfied).HasColumnName("minimum_items_satisfied").HasColumnType("integer").IsRequired();
        builder.Property(x => x.RelationType).HasColumnName("relation_type").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.HasIndex(x => new { x.TargetProgramCourseId, x.GroupNo }).IsUnique().HasDatabaseName("uq_course_prerequisite_group_target_no");
        builder.HasOne(x => x.ProgramVersion).WithMany().HasForeignKey(x => x.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_prerequisite_group_version");
        builder.HasOne(x => x.TargetProgramCourse).WithMany().HasForeignKey(x => new { x.TargetProgramCourseId, x.ProgramVersionId }).HasPrincipalKey(x => new { x.Id, x.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_prerequisite_group_target_version");
    }
}
