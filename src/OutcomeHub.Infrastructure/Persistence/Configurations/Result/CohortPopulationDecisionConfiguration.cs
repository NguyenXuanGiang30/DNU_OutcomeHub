using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class CohortPopulationDecisionConfiguration : IEntityTypeConfiguration<CohortPopulationDecision>
{
    public void Configure(EntityTypeBuilder<CohortPopulationDecision> builder)
    {
        builder.ToTable("cohort_population_decision", "result");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.Id })
            .HasName("pk_cohort_population_decision");

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever().IsRequired();

        builder.Property(entity => entity.BatchId)
            .HasColumnName("batch_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OrgUnitId).HasColumnName("org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ProgramId).HasColumnName("program_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ProgramVersionId).HasColumnName("program_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.MeasurementPeriodId).HasColumnName("measurement_period_id").HasColumnType("uuid").IsRequired();

        builder.Property(entity => entity.CohortId)
            .HasColumnName("cohort_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CurriculumPathId)
            .HasColumnName("curriculum_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OutcomeLevel)
            .HasColumnName("outcome_level")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.CloId)
            .HasColumnName("clo_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ProgramPiId)
            .HasColumnName("program_pi_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ProgramPloId)
            .HasColumnName("program_plo_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.Method)
            .HasColumnName("method")
            .HasColumnType("varchar(16)")
            .HasMaxLength(16)
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DecisionBucket)
            .HasColumnName("decision_bucket")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.ReasonCode)
            .HasColumnName("reason_code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_cohort_population_decision_outcome", "(outcome_level = 'CLO' AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)");
            table.HasCheckConstraint("ck_cohort_population_decision_bucket", "decision_bucket IN ('ATTAINED', 'NOT_ATTAINED_OBSERVED', 'MISSING_IN_DENOMINATOR', 'MISSING_EXCLUDED', 'POLICY_EXCLUDED')");
        });
        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.BatchId, entity.CohortId, entity.CurriculumPathId, entity.OutcomeLevel, entity.CloId, entity.ProgramPiId, entity.ProgramPloId, entity.Method, entity.StudentId }).IsUnique().AreNullsDistinct(false).HasDatabaseName("uq_cohort_population_decision_semantic");
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { BatchId = entity.Id, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_batch_scope");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.ProgramId }).HasPrincipalKey(entity => new { ProgramVersionId = entity.Id, entity.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_program_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_period");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_path");
        builder.HasOne(entity => entity.Clo).WithMany().HasForeignKey(entity => entity.CloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_clo");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_program_pi");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => entity.ProgramPloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_program_plo");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => entity.StudentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_population_decision_student");
    }
}
