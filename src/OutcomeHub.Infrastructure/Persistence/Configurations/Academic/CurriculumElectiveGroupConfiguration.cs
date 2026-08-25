using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Academic;

public sealed class CurriculumElectiveGroupConfiguration : IEntityTypeConfiguration<CurriculumElectiveGroup>
{
    public void Configure(EntityTypeBuilder<CurriculumElectiveGroup> builder)
    {
        builder.ToTable("curriculum_elective_group", "academic", table =>
        {
            table.HasCheckConstraint("ck_curriculum_elective_group_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_curriculum_elective_group_count", "minimum_course_count >= 0 AND (maximum_course_count IS NULL OR maximum_course_count >= minimum_course_count)");
            table.HasCheckConstraint("ck_curriculum_elective_group_credits", "minimum_credits >= 0 AND (maximum_credits IS NULL OR maximum_credits >= minimum_credits)");
        });
        builder.HasKey(x => x.Id).HasName("pk_curriculum_elective_group");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CurriculumBlockId).HasColumnName("curriculum_block_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.MinimumCourseCount).HasColumnName("minimum_course_count").HasColumnType("integer").IsRequired();
        builder.Property(x => x.MaximumCourseCount).HasColumnName("maximum_course_count").HasColumnType("integer").IsRequired(false);
        builder.Property(x => x.MinimumCredits).HasColumnName("minimum_credits").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.MaximumCredits).HasColumnName("maximum_credits").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired(false);
        builder.HasIndex(x => new { x.CurriculumPathId, x.Code }).IsUnique().HasDatabaseName("uq_curriculum_elective_group_path_code");
        builder.HasOne(x => x.CurriculumPath).WithMany().HasForeignKey(x => x.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_elective_group_path");
        builder.HasOne(x => x.CurriculumBlock).WithMany().HasForeignKey(x => x.CurriculumBlockId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_curriculum_elective_group_block");
    }
}
