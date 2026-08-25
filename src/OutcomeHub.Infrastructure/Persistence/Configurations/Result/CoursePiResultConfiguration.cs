using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class CoursePiResultConfiguration : IEntityTypeConfiguration<CoursePiResult>
{
    public void Configure(EntityTypeBuilder<CoursePiResult> builder)
    {
        builder.ToTable("course_pi_result", "result");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.Id })
            .HasName("pk_course_pi_result");

        builder.HasAlternateKey(entity => new { entity.AcademicYearStart, entity.Id, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPiId, entity.CourseOfferingId })
            .HasName("uq_course_pi_result_covering");

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

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CoursePiScore)
            .HasColumnName("course_pi_score")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.ThetaInd)
            .HasColumnName("theta_ind")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.AttainmentStatus)
            .HasColumnName("attainment_status")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
            .IsRequired(true);

        builder.Property(entity => entity.CoreGateStatus)
            .HasColumnName("core_gate_status")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
            .IsRequired(true);

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

        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.BatchId, entity.StudentId, entity.CourseOfferingId, entity.ProgramPiId })
            .IsUnique()
            .HasDatabaseName("uq_course_pi_result_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_course_pi_result_score", "course_pi_score IS NULL OR (course_pi_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND course_pi_score >= 0 AND course_pi_score <= 100)");
            table.HasCheckConstraint("ck_course_pi_result_theta", "theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind >= 0 AND theta_ind <= 100");
            table.HasCheckConstraint("ck_course_pi_result_fraction", "num_nonnulls(numerator, denominator) IN (0, 2) AND (denominator IS NULL OR denominator > 0)");
        });
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { BatchId = entity.Id, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_batch_scope");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.ProgramId }).HasPrincipalKey(entity => new { ProgramVersionId = entity.Id, entity.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_program_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_period");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_path");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_student");
        builder.HasOne(entity => entity.StudentPath).WithMany().HasForeignKey(entity => entity.StudentPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_student_path");
        builder.HasOne(entity => entity.Course).WithMany().HasForeignKey(entity => entity.CourseId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_course");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_offering");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_pi_result_program_pi");
    }
}
