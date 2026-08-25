using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class QuestionCriterionMappingConfiguration : IEntityTypeConfiguration<QuestionCriterionMapping>
{
    public void Configure(EntityTypeBuilder<QuestionCriterionMapping> builder)
    {
        builder.ToTable("question_criterion_mapping", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_question_criterion_mapping_weight", "criterion_weight_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND criterion_weight_ratio > 0 AND criterion_weight_ratio <= 1");
            });
        builder.HasKey(x => new { x.QuestionId, x.RubricCriterionId }).HasName("pk_question_criterion_mapping");
        builder.Property(x => x.QuestionId).HasColumnName("question_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RubricCriterionId).HasColumnName("rubric_criterion_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CriterionWeightRatio).HasColumnName("criterion_weight_ratio").HasColumnType("numeric(12,10)").HasPrecision(12, 10).IsRequired();

        builder.HasOne(x => x.Question).WithMany().HasForeignKey(x => new { x.QuestionId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_question_criterion_mapping_question_version");
        builder.HasOne(x => x.RubricCriterion).WithMany().HasForeignKey(x => new { x.RubricCriterionId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_question_criterion_mapping_criterion_version");
    }
}
