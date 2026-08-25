using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Quality;

public sealed class ImprovementFindingConfiguration : IEntityTypeConfiguration<ImprovementFinding>
{
    public void Configure(EntityTypeBuilder<ImprovementFinding> builder)
    {
        builder.ToTable("improvement_finding", "quality");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_improvement_finding");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.ImprovementPlanId)
            .HasColumnName("improvement_plan_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.FindingType)
            .HasColumnName("finding_type")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(false);

        builder.Property(entity => entity.CohortOutcomeResultId)
            .HasColumnName("cohort_outcome_result_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ResultAlertId)
            .HasColumnName("result_alert_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.SourceChecksum)
            .HasColumnName("source_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_improvement_finding_source", "cohort_outcome_result_id IS NOT NULL OR result_alert_id IS NOT NULL OR char_length(btrim(description)) > 0");
            table.HasCheckConstraint("ck_improvement_finding_source_year", "(cohort_outcome_result_id IS NULL AND result_alert_id IS NULL) OR academic_year_start IS NOT NULL");
            table.HasCheckConstraint("ck_improvement_finding_checksum", "source_checksum IS NULL OR source_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasOne(entity => entity.ImprovementPlan).WithMany().HasForeignKey(entity => entity.ImprovementPlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_finding_plan");
        builder.HasOne(entity => entity.CohortOutcomeResult).WithMany().HasForeignKey(entity => new { entity.AcademicYearStart, entity.CohortOutcomeResultId }).HasPrincipalKey(entity => new { entity.AcademicYearStart, CohortOutcomeResultId = entity.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_finding_cohort_result_year");
        builder.HasOne(entity => entity.ResultAlert).WithMany().HasForeignKey(entity => new { entity.AcademicYearStart, entity.ResultAlertId }).HasPrincipalKey(entity => new { entity.AcademicYearStart, ResultAlertId = entity.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_improvement_finding_alert_year");
        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.CohortOutcomeResultId }).HasDatabaseName("ix_improvement_finding_cohort_result");
        builder.HasIndex(entity => new { entity.AcademicYearStart, entity.ResultAlertId }).HasDatabaseName("ix_improvement_finding_result_alert");
    }
}
