using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class StudentCriterionScoreLineageConfiguration : IEntityTypeConfiguration<StudentCriterionScoreLineage>
{
    public void Configure(EntityTypeBuilder<StudentCriterionScoreLineage> builder)
    {
        builder.ToTable("student_criterion_score_lineage", "result");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.StudentCriterionResultId, entity.ScoreRecordId })
            .HasName("pk_student_criterion_score_lineage");

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.BatchId)
            .HasColumnName("batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OrgUnitId).HasColumnName("org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ProgramId).HasColumnName("program_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.MeasurementPeriodId).HasColumnName("measurement_period_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.CohortId).HasColumnName("cohort_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.CurriculumPathId).HasColumnName("curriculum_path_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.CourseId).HasColumnName("course_id").HasColumnType("uuid").IsRequired();

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RubricCriterionId)
            .HasColumnName("rubric_criterion_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentCriterionResultId)
            .HasColumnName("student_criterion_result_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ScoreRecordId)
            .HasColumnName("score_record_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AssessmentQuestionId)
            .HasColumnName("assessment_question_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.SourceWeightRatio)
            .HasColumnName("source_weight_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.WeightedContribution)
            .HasColumnName("weighted_contribution")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_student_criterion_score_lineage_weight", "source_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND source_weight_ratio > 0 AND source_weight_ratio <= 1 AND weighted_contribution NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)"));
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { BatchId = entity.Id, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_batch_snapshot_scope");
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_snapshot");
        builder.HasOne(entity => entity.StudentCriterionResult).WithMany()
            .HasForeignKey(entity => new { entity.AcademicYearStart, entity.StudentCriterionResultId, entity.BatchId, entity.StudentId, entity.CourseOfferingId, entity.RubricCriterionId })
            .HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id, entity.BatchId, entity.StudentId, entity.CourseOfferingId, entity.RubricCriterionId })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_result_scope");
        builder.HasOne(entity => entity.SnapshotScore).WithMany()
            .HasForeignKey(entity => new { entity.InputSnapshotId, entity.AcademicYearStart, entity.ScoreRecordId, entity.StudentId, entity.CourseOfferingId })
            .HasPrincipalKey(entity => new { entity.InputSnapshotId, entity.AcademicYearStart, entity.ScoreRecordId, entity.StudentId, entity.CourseOfferingId })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_snapshot_score");
        builder.HasOne(entity => entity.QuestionCriterionWeight).WithMany()
            .HasForeignKey(entity => new { entity.InputSnapshotId, entity.AssessmentQuestionId, entity.RubricCriterionId })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_question_weight");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.ProgramId }).HasPrincipalKey(entity => new { ProgramVersionId = entity.Id, entity.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_program_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_period");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_path");
        builder.HasOne(entity => entity.Course).WithMany().HasForeignKey(entity => entity.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_lineage_course");
    }
}
