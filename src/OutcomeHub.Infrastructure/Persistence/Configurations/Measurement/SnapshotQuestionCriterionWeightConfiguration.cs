using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotQuestionCriterionWeightConfiguration : IEntityTypeConfiguration<SnapshotQuestionCriterionWeight>
{
    public void Configure(EntityTypeBuilder<SnapshotQuestionCriterionWeight> builder)
    {
        builder.ToTable("snapshot_question_criterion_weight", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.AssessmentQuestionId, entity.RubricCriterionId })
            .HasName("pk_snapshot_question_criterion_weight");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AssessmentQuestionId)
            .HasColumnName("assessment_question_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RubricCriterionId)
            .HasColumnName("rubric_criterion_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SourceMode)
            .HasColumnName("source_mode")
            .HasColumnType("varchar(16)")
            .HasMaxLength(16)
            .IsRequired(true);

        builder.Property(entity => entity.CriterionWeightRatio)
            .HasColumnName("criterion_weight_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_snapshot_question_criterion_source_mode", "source_mode IN ('QUESTION','CRITERION')");
            table.HasCheckConstraint("ck_snapshot_question_criterion_weight", "criterion_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND criterion_weight_ratio > 0 AND criterion_weight_ratio <= 1");
        });
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_question_criterion_input_snapshot");
        builder.HasOne(entity => entity.AssessmentQuestion).WithMany().HasForeignKey(entity => entity.AssessmentQuestionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_question_criterion_question");
        builder.HasOne(entity => entity.RubricCriterion).WithMany().HasForeignKey(entity => entity.RubricCriterionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_question_criterion_criterion");
    }
}
