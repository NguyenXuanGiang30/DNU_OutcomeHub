using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class StudentPloPiContributionConfiguration : IEntityTypeConfiguration<StudentPloPiContribution>
{
    public void Configure(EntityTypeBuilder<StudentPloPiContribution> builder)
    {
        builder.ToTable("student_plo_pi_contribution", "result");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.StudentPloResultId, entity.StudentPiResultId })
            .HasName("pk_student_plo_pi_contribution");

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

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentPathId)
            .HasColumnName("student_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Method)
            .HasColumnName("method")
            .HasColumnType("varchar(16)")
            .HasMaxLength(16)
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPloId)
            .HasColumnName("program_plo_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentPloResultId)
            .HasColumnName("student_plo_result_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentPiResultId)
            .HasColumnName("student_pi_result_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PiWeightRatio)
            .HasColumnName("pi_weight_ratio")
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

        builder.Property(entity => entity.GateFailureReason)
            .HasColumnName("gate_failure_reason")
            .HasColumnType("text")
            .IsRequired(false);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_student_plo_pi_contribution_method", "method IN ('DIRECT', 'INDIRECT', 'COMBINED')");
            table.HasCheckConstraint("ck_student_plo_pi_contribution_weight", "pi_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND pi_weight_ratio > 0 AND pi_weight_ratio <= 1 AND weighted_contribution NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)");
            table.HasCheckConstraint("ck_student_plo_pi_contribution_gate_reason", "is_core OR gate_failure_reason IS NULL");
        });
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { BatchId = entity.Id, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_batch_snapshot_scope");
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_snapshot");
        builder.HasOne(entity => entity.StudentPloResult).WithMany()
            .HasForeignKey(entity => new { entity.AcademicYearStart, entity.StudentPloResultId, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPloId, entity.Method })
            .HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPloId, entity.Method })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_plo_result");
        builder.HasOne(entity => entity.StudentPiResult).WithMany()
            .HasForeignKey(entity => new { entity.AcademicYearStart, entity.StudentPiResultId, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPiId, entity.Method })
            .HasPrincipalKey(entity => new { entity.AcademicYearStart, entity.Id, entity.BatchId, entity.StudentId, entity.StudentPathId, entity.ProgramPiId, entity.Method })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_pi_result");
        builder.HasOne(entity => entity.SnapshotPiPloWeight).WithMany().HasForeignKey(entity => new { entity.InputSnapshotId, entity.ProgramPiId, entity.ProgramPloId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_snapshot_weight");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.ProgramId }).HasPrincipalKey(entity => new { ProgramVersionId = entity.Id, entity.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_program_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_period");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_path");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_student");
        builder.HasOne(entity => entity.StudentPath).WithMany().HasForeignKey(entity => entity.StudentPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_student_path");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => entity.ProgramPloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_program_plo");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_student_plo_pi_contribution_program_pi");
    }
}
