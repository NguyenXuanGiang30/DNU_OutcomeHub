using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class PeriodPopulationMemberConfiguration : IEntityTypeConfiguration<PeriodPopulationMember>
{
    public void Configure(EntityTypeBuilder<PeriodPopulationMember> builder)
    {
        builder.ToTable("period_population_member", "measurement");

        builder.HasKey(entity => new { entity.MeasurementPeriodId, entity.StudentId })
            .HasName("pk_period_population_member");

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

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentPathId)
            .HasColumnName("student_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CurriculumPathId)
            .HasColumnName("curriculum_path_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Decision)
            .HasColumnName("decision")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.ExclusionReasonCode)
            .HasColumnName("exclusion_reason_code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.DecisionSource)
            .HasColumnName("decision_source")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.DecidedBy)
            .HasColumnName("decided_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DecidedAt)
            .HasColumnName("decided_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_period_population_member_decision", "decision IN ('PENDING','INCLUDED','EXCLUDED')");
            table.HasCheckConstraint("ck_period_population_member_exclusion", "(decision = 'EXCLUDED' AND exclusion_reason_code IS NOT NULL) OR (decision <> 'EXCLUDED' AND exclusion_reason_code IS NULL)");
        });
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_member_period_program");
        builder.HasOne<MeasurementPeriodCohort>().WithMany().HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.ProgramVersionId, entity.CohortId }).HasPrincipalKey(entity => new { entity.MeasurementPeriodId, entity.ProgramVersionId, entity.CohortId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_member_period_cohort");
        builder.HasOne(entity => entity.Cohort).WithMany().HasForeignKey(entity => entity.CohortId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_member_cohort");
        builder.HasOne(entity => entity.Student).WithMany().HasForeignKey(entity => new { entity.StudentId, entity.CohortId }).HasPrincipalKey(entity => new { entity.PersonId, entity.AdmissionCohortId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_member_student_cohort");
        builder.HasOne(entity => entity.StudentPath).WithMany().HasForeignKey(entity => new { entity.StudentPathId, entity.StudentId, entity.ProgramVersionId, entity.CurriculumPathId }).HasPrincipalKey(entity => new { entity.Id, entity.StudentId, entity.ProgramVersionId, entity.CurriculumPathId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_member_student_path");
        builder.HasOne(entity => entity.CurriculumPath).WithMany().HasForeignKey(entity => new { entity.CurriculumPathId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_member_curriculum_path");
        builder.HasOne(entity => entity.Decider).WithMany().HasForeignKey(entity => entity.DecidedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_member_decider");
    }
}
