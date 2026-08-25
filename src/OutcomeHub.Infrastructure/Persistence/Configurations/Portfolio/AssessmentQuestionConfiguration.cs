using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class AssessmentQuestionConfiguration : IEntityTypeConfiguration<AssessmentQuestion>
{
    public void Configure(EntityTypeBuilder<AssessmentQuestion> builder)
    {
        builder.ToTable("assessment_question", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_assessment_question_code", "question_code = upper(btrim(question_code)) AND char_length(question_code) > 0");
                table.HasCheckConstraint("ck_assessment_question_max_score", "max_score NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND max_score > 0");
                table.HasCheckConstraint("ck_assessment_question_sort_order", "sort_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_assessment_question");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AssessmentItemId).HasColumnName("assessment_item_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.QuestionCode).HasColumnName("question_code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.MaxScore).HasColumnName("max_score").HasColumnType("numeric(20,10)").HasPrecision(20, 10).IsRequired();
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusVersionId }).HasName("uq_assessment_question_id_version");
        builder.HasAlternateKey(x => new { x.Id, x.AssessmentItemId, x.SyllabusVersionId }).HasName("uq_assessment_question_full_binding");
        builder.HasIndex(x => new { x.AssessmentItemId, x.QuestionCode }).IsUnique().HasDatabaseName("uq_assessment_question_item_code");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_assessment_question_syllabus_version");
        builder.HasOne(x => x.AssessmentItem).WithMany().HasForeignKey(x => new { x.AssessmentItemId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_assessment_question_item_version");
    }
}
