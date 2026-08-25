using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class StudentPiSourceContributionConfiguration : IEntityTypeConfiguration<StudentPiSourceContribution>
{
    public void Configure(EntityTypeBuilder<StudentPiSourceContribution> builder)
    {
        builder.ToTable("student_pi_source_contribution", "result");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.StudentPiResultId, entity.CoursePiResultId })
            .HasName("pk_student_pi_source_contribution");

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

        builder.Property(entity => entity.StudentPathId)
            .HasColumnName("student_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Method)
            .HasColumnName("method")
            .HasColumnType("varchar(16)")
            .HasMaxLength(16)
            .IsRequired(true);

        builder.Property(entity => entity.StudentPiResultId)
            .HasColumnName("student_pi_result_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CoursePiResultId)
            .HasColumnName("course_pi_result_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SourceWeightRatio)
            .HasColumnName("source_weight_ratio")
            .HasColumnType("numeric(12,10)")
            .IsRequired(true);

        builder.Property(entity => entity.WeightedContribution)
            .HasColumnName("weighted_contribution")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.SourceRole)
            .HasColumnName("source_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.AnchorAssessmentId)
            .HasColumnName("anchor_assessment_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_student_pi_source_contribution_method", "method IN ('DIRECT', 'INDIRECT', 'COMBINED')");
            table.HasCheckConstraint("ck_student_pi_source_contribution_weight", "source_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND source_weight_ratio > 0 AND source_weight_ratio <= 1 AND weighted_contribution NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)");
        });
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { BatchId = entity.Id, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_batch_snapshot_scope");
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_snapshot");
        builder.HasOne(entity => entity.StudentPiResult).WithMany()
            .HasForeignKey(entity => new { entity.AcademicYearStart, entity.StudentPiResultId, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPiId, entity.Method })
            .HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPiId, entity.Method })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_pi_result");
        builder.HasOne(entity => entity.CoursePiResult).WithMany()
            .HasForeignKey(entity => new { entity.AcademicYearStart, entity.CoursePiResultId, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPiId, entity.CourseOfferingId })
            .HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPiId, entity.CourseOfferingId })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_course_result");
        builder.HasOne(entity => entity.SnapshotPiSourceWeight).WithMany().HasForeignKey(entity => new { entity.InputSnapshotId, entity.StudentPathId, entity.ProgramPiId, entity.CourseOfferingId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_snapshot_weight");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.ProgramId }).HasPrincipalKey(entity => new { ProgramVersionId = entity.Id, entity.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_program_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_period");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_path");
        builder.HasOne(entity => entity.Course).WithMany().HasForeignKey(entity => entity.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_course");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_student");
        builder.HasOne(entity => entity.StudentPath).WithMany().HasForeignKey(entity => entity.StudentPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_student_path");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_program_pi");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_offering");
        builder.HasOne(entity => entity.AnchorAssessment).WithMany().HasForeignKey(entity => entity.AnchorAssessmentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_pi_source_contribution_anchor");
    }
}
