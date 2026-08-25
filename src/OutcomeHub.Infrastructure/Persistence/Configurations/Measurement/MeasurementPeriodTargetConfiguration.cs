using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class MeasurementPeriodTargetConfiguration : IEntityTypeConfiguration<MeasurementPeriodTarget>
{
    public void Configure(EntityTypeBuilder<MeasurementPeriodTarget> builder)
    {
        builder.ToTable("measurement_period_target", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_measurement_period_target");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OutcomeLevel)
            .HasColumnName("outcome_level")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.SyllabusVersionId)
            .HasColumnName("syllabus_version_id")
            .HasColumnType("uuid")
            .IsRequired(false);

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

        builder.Property(entity => entity.TargetRole)
            .HasColumnName("target_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.MeasurementPeriodId, entity.OutcomeLevel, entity.CourseOfferingId, entity.SyllabusVersionId, entity.CloId, entity.ProgramPiId, entity.ProgramPloId })
            .IsUnique()
            .AreNullsDistinct(false)
            .HasDatabaseName("uq_measurement_period_target_1");

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint("ck_measurement_period_target_outcome", "num_nonnulls(clo_id, program_pi_id, program_plo_id) = 1"));

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_measurement_period_target_level", "outcome_level IN ('CLO','PI','PLO')");
            table.HasCheckConstraint("ck_measurement_period_target_shape", "(outcome_level = 'CLO' AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL AND course_offering_id IS NOT NULL AND syllabus_version_id IS NOT NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL AND course_offering_id IS NULL AND syllabus_version_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL AND course_offering_id IS NULL AND syllabus_version_id IS NULL)");
        });
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_target_period_program");
        builder.HasOne(entity => entity.PeriodOffering).WithMany().HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.CourseOfferingId }).HasPrincipalKey(entity => new { entity.MeasurementPeriodId, entity.CourseOfferingId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_target_period_offering");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => entity.CourseOfferingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_target_course_offering");
        builder.HasOne(entity => entity.SyllabusVersion).WithMany().HasForeignKey(entity => new { entity.SyllabusVersionId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_target_syllabus_program");
        builder.HasOne(entity => entity.Clo).WithMany().HasForeignKey(entity => new { entity.CloId, entity.SyllabusVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_target_clo_syllabus");
        builder.HasOne(entity => entity.ProgramPi).WithMany().HasForeignKey(entity => new { entity.ProgramPiId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_target_program_pi");
        builder.HasOne(entity => entity.ProgramPlo).WithMany().HasForeignKey(entity => new { entity.ProgramPloId, entity.ProgramVersionId }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_target_program_plo");
    }
}
