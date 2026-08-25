using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class MeasurementPeriodOfferingConfiguration : IEntityTypeConfiguration<MeasurementPeriodOffering>
{
    public void Configure(EntityTypeBuilder<MeasurementPeriodOffering> builder)
    {
        builder.ToTable("measurement_period_offering", "measurement");

        builder.HasKey(entity => new { entity.MeasurementPeriodId, entity.CourseOfferingId })
            .HasName("pk_measurement_period_offering");

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.CourseOfferingId)
            .HasColumnName("course_offering_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PlannedSourceRole)
            .HasColumnName("planned_source_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.CollectionStatus)
            .HasColumnName("collection_status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.DueAt)
            .HasColumnName("due_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.HasOne(entity => entity.MeasurementPeriod).WithMany(entity => entity.Offerings).HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.ProgramVersionId, entity.AcademicYearStart }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId, entity.AcademicYearStart }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_offering_period_binding");
        builder.HasOne(entity => entity.CourseOffering).WithMany().HasForeignKey(entity => new { entity.CourseOfferingId, entity.ProgramVersionId, entity.AcademicYearStart }).HasPrincipalKey(entity => new { entity.Id, entity.ProgramVersionId, entity.AcademicYearStart }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_measurement_period_offering_course_offering");
    }
}
