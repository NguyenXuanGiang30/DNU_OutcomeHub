using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class PeriodPopulationEnrollmentConfiguration : IEntityTypeConfiguration<PeriodPopulationEnrollment>
{
    public void Configure(EntityTypeBuilder<PeriodPopulationEnrollment> builder)
    {
        builder.ToTable("period_population_enrollment", "measurement");

        builder.HasKey(entity => new { entity.MeasurementPeriodId, entity.StudentId, entity.EnrollmentRevisionId })
            .HasName("pk_period_population_enrollment");

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.StudentId)
            .HasColumnName("student_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EnrollmentRevisionId)
            .HasColumnName("enrollment_revision_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SelectionRole)
            .HasColumnName("selection_role")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.HasOne(entity => entity.PopulationMember).WithMany().HasForeignKey(entity => new { entity.MeasurementPeriodId, entity.StudentId }).HasPrincipalKey(entity => new { entity.MeasurementPeriodId, entity.StudentId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_enrollment_member");
        builder.HasOne(entity => entity.EnrollmentRevision).WithMany().HasForeignKey(entity => entity.EnrollmentRevisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_period_population_enrollment_revision");
    }
}
