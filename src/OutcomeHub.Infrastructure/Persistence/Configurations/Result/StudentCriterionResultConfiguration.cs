using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class StudentCriterionResultConfiguration : IEntityTypeConfiguration<StudentCriterionResult>
{
    public void Configure(EntityTypeBuilder<StudentCriterionResult> builder)
    {
        builder.ToTable("student_criterion_result", "result");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.Id })
            .HasName("pk_student_criterion_result");

        builder.HasAlternateKey(entity => new { entity.AcademicYearStart, entity.Id, entity.BatchId, entity.StudentId, entity.CourseOfferingId, entity.RubricCriterionId })
            .HasName("uq_student_criterion_result_covering");

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

        builder.Property(entity => entity.SourceMode)
            .HasColumnName("source_mode")
            .HasColumnType("varchar(16)")
            .HasMaxLength(16)
            .IsRequired(true);

        builder.Property(entity => entity.Score)
            .HasColumnName("score")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.MaxScore)
            .HasColumnName("max_score")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.NormalizedScore)
            .HasColumnName("normalized_score")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.DataStatus)
            .HasColumnName("data_status")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
            .IsRequired(true);

        builder.Property(entity => entity.Numerator)
            .HasColumnName("numerator")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.Denominator)
            .HasColumnName("denominator")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.BatchId, entity.StudentId, entity.CourseOfferingId, entity.RubricCriterionId })
            .IsUnique()
            .HasDatabaseName("uq_student_criterion_result_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_student_criterion_result_source_mode", "source_mode IN ('CRITERION', 'QUESTION')");
            table.HasCheckConstraint("ck_student_criterion_result_scores", "max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0 AND (score IS NULL OR score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (normalized_score IS NULL OR (normalized_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND normalized_score >= 0 AND normalized_score <= 100))");
            table.HasCheckConstraint("ck_student_criterion_result_fraction", "num_nonnulls(numerator, denominator) IN (0, 2) AND (denominator IS NULL OR denominator > 0)");
        });
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { BatchId = entity.Id, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_batch_scope");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.ProgramId }).HasPrincipalKey(entity => new { ProgramVersionId = entity.Id, entity.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_program_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_period");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_path");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_student");
        builder.HasOne(entity => entity.StudentPath).WithMany().HasForeignKey(entity => entity.StudentPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_student_path");
        builder.HasOne(entity => entity.Course).WithMany().HasForeignKey(entity => entity.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_course");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_offering");
        builder.HasOne(entity => entity.AssessmentItem).WithMany().HasForeignKey(entity => entity.AssessmentItemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_assessment");
        builder.HasOne(entity => entity.RubricCriterion).WithMany().HasForeignKey(entity => entity.RubricCriterionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_criterion_result_criterion");
    }
}
