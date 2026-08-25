using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class RubricCriterionConfiguration : IEntityTypeConfiguration<RubricCriterion>
{
    public void Configure(EntityTypeBuilder<RubricCriterion> builder)
    {
        builder.ToTable("rubric_criterion", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_rubric_criterion_code", "criterion_code = upper(btrim(criterion_code)) AND char_length(criterion_code) > 0");
                table.HasCheckConstraint("ck_rubric_criterion_max_score", "max_score NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND max_score > 0");
                table.HasCheckConstraint("ck_rubric_criterion_weight", "rubric_weight_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND rubric_weight_ratio > 0 AND rubric_weight_ratio <= 1");
                table.HasCheckConstraint("ck_rubric_criterion_score_source_mode", "score_source_mode IN ('CRITERION','QUESTION')");
                table.HasCheckConstraint("ck_rubric_criterion_sort_order", "sort_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_rubric_criterion");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.RubricId).HasColumnName("rubric_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AssessmentItemId).HasColumnName("assessment_item_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CriterionCode).HasColumnName("criterion_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
        builder.Property(x => x.MaxScore).HasColumnName("max_score").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired();
        builder.Property(x => x.RubricWeightRatio).HasColumnName("rubric_weight_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired();
        builder.Property(x => x.ScoreSourceMode).HasColumnName("score_source_mode").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.IsCore).HasColumnName("is_core").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.IndividualEvidence).HasColumnName("individual_evidence").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusVersionId }).HasName("uq_rubric_criterion_id_version");
        builder.HasAlternateKey(x => new { x.Id, x.AssessmentItemId, x.SyllabusVersionId }).HasName("uq_rubric_criterion_full_binding");
        builder.HasIndex(x => new { x.RubricId, x.CriterionCode }).IsUnique().HasDatabaseName("uq_rubric_criterion_code");
        builder.HasOne(x => x.Rubric).WithMany(x => x.Criteria).HasForeignKey(x => new { x.RubricId, x.AssessmentItemId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.AssessmentItemId, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_rubric_criterion_rubric_binding");
        builder.HasOne(x => x.AssessmentItem).WithMany().HasForeignKey(x => new { x.AssessmentItemId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_rubric_criterion_assessment_version");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_rubric_criterion_syllabus_version");
    }
}
