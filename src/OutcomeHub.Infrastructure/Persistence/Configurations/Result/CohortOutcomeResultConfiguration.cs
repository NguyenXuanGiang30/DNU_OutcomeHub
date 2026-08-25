using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class CohortOutcomeResultConfiguration : IEntityTypeConfiguration<CohortOutcomeResult>
{
    public void Configure(EntityTypeBuilder<CohortOutcomeResult> builder)
    {
        builder.ToTable("cohort_outcome_result", "result");

        builder.HasKey(entity => new { entity.AcademicYearStart, entity.Id })
            .HasName("pk_cohort_outcome_result");

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

        builder.Property(entity => entity.PopulationCount)
            .HasColumnName("population_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.DenominatorCount)
            .HasColumnName("denominator_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.AttainedCount)
            .HasColumnName("attained_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.NotAttainedObservedCount)
            .HasColumnName("not_attained_observed_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.MissingInDenominatorCount)
            .HasColumnName("missing_in_denominator_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.NotAttainedCount)
            .HasColumnName("not_attained_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.MissingExcludedCount)
            .HasColumnName("missing_excluded_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.PolicyExcludedCount)
            .HasColumnName("policy_excluded_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.AttainmentRate)
            .HasColumnName("attainment_rate")
            .HasColumnType("numeric(20,10)")
            .IsRequired(false);

        builder.Property(entity => entity.ThetaCoh)
            .HasColumnName("theta_coh")
            .HasColumnType("numeric(20,10)")
            .IsRequired(true);

        builder.Property(entity => entity.OutcomeStatus)
            .HasColumnName("outcome_status")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
            .IsRequired(true);

        builder.Property(entity => entity.PrivacySuppressed)
            .HasColumnName("privacy_suppressed")
            .HasColumnType("boolean")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.BatchId, entity.CohortId, entity.CurriculumPathId, entity.OutcomeLevel, entity.CloId, entity.ProgramPiId, entity.ProgramPloId, entity.Method })
            .IsUnique()
            .AreNullsDistinct(false)
            .HasDatabaseName("uq_cohort_outcome_result_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_cohort_outcome_result_outcome", "(outcome_level = 'CLO' AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)");
            table.HasCheckConstraint("ck_cohort_outcome_result_counts", "population_count >= 0 AND denominator_count >= 0 AND attained_count >= 0 AND not_attained_observed_count >= 0 AND missing_in_denominator_count >= 0 AND not_attained_count >= 0 AND missing_excluded_count >= 0 AND policy_excluded_count >= 0 AND not_attained_count = not_attained_observed_count + missing_in_denominator_count AND denominator_count = attained_count + not_attained_observed_count + missing_in_denominator_count AND population_count = denominator_count + missing_excluded_count + policy_excluded_count");
            table.HasCheckConstraint("ck_cohort_outcome_result_rate", "(denominator_count = 0 AND attainment_rate IS NULL AND outcome_status = 'INSUFFICIENT_DATA') OR (denominator_count > 0 AND attainment_rate IS NOT NULL AND attainment_rate NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND attainment_rate >= 0 AND attainment_rate <= 100 AND attainment_rate = round((100::numeric * attained_count::numeric / denominator_count::numeric), 10))");
            table.HasCheckConstraint("ck_cohort_outcome_result_theta", "theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh >= 0 AND theta_coh <= 100");
        });
        builder.HasOne(entity => entity.Batch).WithMany().HasForeignKey(entity => new { entity.BatchId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).HasPrincipalKey(entity => new { BatchId = entity.Id, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_batch_scope");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_org_unit");
        builder.HasOne(entity => entity.Program).WithMany().HasForeignKey(entity => entity.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.ProgramId }).HasPrincipalKey(entity => new { ProgramVersionId = entity.Id, entity.ProgramId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_program_version");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_period");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_cohort");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => entity.CurriculumPathId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_path");
        builder.HasOne(entity => entity.Clo).WithMany().HasForeignKey(entity => entity.CloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_clo");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => entity.ProgramPiId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_program_pi");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => entity.ProgramPloId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_cohort_outcome_result_program_plo");
    }
}
