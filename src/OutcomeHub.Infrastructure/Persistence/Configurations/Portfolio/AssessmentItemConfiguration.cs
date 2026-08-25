using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class AssessmentItemConfiguration : IEntityTypeConfiguration<AssessmentItem>
{
    public void Configure(EntityTypeBuilder<AssessmentItem> builder)
    {
        builder.ToTable("assessment_item", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_assessment_item_code", "assessment_code = upper(btrim(assessment_code)) AND char_length(assessment_code) > 0");
                table.HasCheckConstraint("ck_assessment_item_course_weight", "course_weight_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND course_weight_ratio >= 0 AND course_weight_ratio <= 1");
                table.HasCheckConstraint("ck_assessment_item_individual_ratio", "individual_component_ratio IS NULL OR (individual_component_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND individual_component_ratio >= 0 AND individual_component_ratio <= 1)");
                table.HasCheckConstraint("ck_assessment_item_max_score", "max_score NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND max_score > 0");
                table.HasCheckConstraint("ck_assessment_item_sort_order", "sort_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_assessment_item");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ParentId).HasColumnName("parent_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.AssessmentCode).HasColumnName("assessment_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.AssessmentType).HasColumnName("assessment_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CourseWeightRatio).HasColumnName("course_weight_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired();
        builder.Property(x => x.IndividualComponentRatio).HasColumnName("individual_component_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired(false);
        builder.Property(x => x.IsGroupAssessment).HasColumnName("is_group_assessment").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.CountsTowardCourseGrade).HasColumnName("counts_toward_course_grade").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.MaxScore).HasColumnName("max_score").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusVersionId }).HasName("uq_assessment_item_id_version");
        builder.HasIndex(x => new { x.SyllabusVersionId, x.AssessmentCode }).IsUnique().HasDatabaseName("uq_assessment_item_version_code");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_assessment_item_syllabus_version");
        builder.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => new { x.ParentId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_assessment_item_parent_version");
    }
}
