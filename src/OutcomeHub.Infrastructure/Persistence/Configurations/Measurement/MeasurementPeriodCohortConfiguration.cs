using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class MeasurementPeriodCohortConfiguration : IEntityTypeConfiguration<MeasurementPeriodCohort>
{
    public void Configure(EntityTypeBuilder<MeasurementPeriodCohort> builder)
    {
        builder.ToTable("measurement_period_cohort", "measurement");

        builder.HasKey(entity => new { entity.MeasurementPeriodId, entity.CohortId })
            .HasName("pk_measurement_period_cohort");

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CohortId)
            .HasColumnName("cohort_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.MeasurementPeriodId, entity.ProgramVersionId, entity.CohortId })
            .IsUnique()
            .HasDatabaseName("uq_measurement_period_cohort_1");

        builder.HasOne(entity => entity.MeasurementPeriod).WithMany(entity => entity.Cohorts).HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_cohort_period_program");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_cohort_program_version");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_cohort_cohort");
        builder.HasOne(entity => entity.ProgramVersionCohort).WithMany().HasForeignKey(entity => new { entity.ProgramVersionId, entity.CohortId }).HasPrincipalKey(entity => new { entity.ProgramVersionId, entity.CohortId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_cohort_program_cohort");
    }
}
