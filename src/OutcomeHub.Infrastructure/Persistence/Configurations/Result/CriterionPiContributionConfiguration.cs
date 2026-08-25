using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class CriterionPiContributionConfiguration : IEntityTypeConfiguration<CriterionPiContribution>
{
    public void Configure(EntityTypeBuilder<CriterionPiContribution> builder)
    {
        builder.ToTable("criterion_pi_contribution", "result");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.Id })
            .HasName("pk_criterion_pi_contribution");

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.BatchId)
            .HasColumnName("batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OrgUnitId)
            .HasColumnName("org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramId)
            .HasColumnName("program_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CohortId)
            .HasColumnName("cohort_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CurriculumPathId)
            .HasColumnName("curriculum_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentPathId)
            .HasColumnName("student_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseId)
            .HasColumnName("course_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AssessmentItemId)
            .HasColumnName("assessment_item_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.RubricCriterionId)
            .HasColumnName("rubric_criterion_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SyllabusTraceabilityId)
            .HasColumnName("syllabus_traceability_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentCriterionResultId)
            .HasColumnName("student_criterion_result_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.NormalizedScore)
            .HasColumnName("normalized_score")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.DirectWeightRatio)
            .HasColumnName("direct_weight_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.AllocationRatio)
            .HasColumnName("allocation_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.WeightedContribution)
            .HasColumnName("weighted_contribution")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.IsCore)
            .HasColumnName("is_core")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.Included)
            .HasColumnName("included")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.Property(entity => entity.ExclusionReason)
            .HasColumnName("exclusion_reason")
            .HasColumnType("text")
            .IsRequired(false);

        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.BatchId, entity.StudentId, entity.CourseOfferingId, entity.ProgramPiId, entity.RubricCriterionId })
            .IsUnique()
            .HasDatabaseName("uq_criterion_pi_contribution_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_criterion_pi_contribution_numeric", "normalized_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND normalized_score >= 0 AND normalized_score <= 100 AND direct_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND direct_weight_ratio >= 0 AND direct_weight_ratio <= 1 AND allocation_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND allocation_ratio >= 0 AND allocation_ratio <= 1 AND weighted_contribution NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)");
            table.HasCheckConstraint("ck_criterion_pi_contribution_exclusion", "(included AND exclusion_reason IS NULL) OR (NOT included AND char_length(btrim(exclusion_reason)) > 0)");
        });
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { BatchId = entity.Id, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_batch_snapshot_scope");
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_snapshot");
        builder.HasOne(entity => entity.StudentCriterionResult).WithMany()
            .HasForeignKey(entity => new { entity.AcademicYearStart, entity.StudentCriterionResultId, entity.BatchId, entity.StudentId, entity.CourseOfferingId, entity.RubricCriterionId })
            .HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id, entity.BatchId, entity.StudentId, entity.CourseOfferingId, entity.RubricCriterionId })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_criterion_result");
        builder.HasOne(entity => entity.SnapshotDirectPiWeight).WithMany().HasForeignKey(entity => new { entity.InputSnapshotId, entity.SyllabusTraceabilityId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_snapshot_weight");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.ProgramId }).HasPrincipalKey(entity => new { ProgramVersionId = entity.Id, entity.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_program_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_period");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_path");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_student");
        builder.HasOne(entity => entity.StudentPath).WithMany().HasForeignKey(entity => entity.StudentPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_student_path");
        builder.HasOne(entity => entity.Course).WithMany().HasForeignKey(entity => entity.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_course");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_offering");
        builder.HasOne(entity => entity.AssessmentItem).WithMany().HasForeignKey(entity => entity.AssessmentItemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_assessment");
        builder.HasOne(entity => entity.RubricCriterion).WithMany().HasForeignKey(entity => entity.RubricCriterionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_criterion");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_program_pi");
        builder.HasOne(entity => entity.SyllabusTraceability).WithMany().HasForeignKey(entity => entity.SyllabusTraceabilityId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_criterion_pi_contribution_traceability");
    }
}
