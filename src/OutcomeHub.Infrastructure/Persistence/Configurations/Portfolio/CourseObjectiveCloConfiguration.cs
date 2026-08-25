using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class CourseObjectiveCloConfiguration : IEntityTypeConfiguration<CourseObjectiveClo>
{
    public void Configure(EntityTypeBuilder<CourseObjectiveClo> builder)
    {
        builder.ToTable("course_objective_clo", "portfolio");
        builder.HasKey(x => new { x.CourseObjectiveId, x.CloId }).HasName("pk_course_objective_clo");
        builder.Property(x => x.CourseObjectiveId).HasColumnName("course_objective_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CloId).HasColumnName("clo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();

        builder.HasOne(x => x.CourseObjective).WithMany(x => x.CloMappings).HasForeignKey(x => new { x.CourseObjectiveId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_objective_clo_objective_version");
        builder.HasOne(x => x.Clo).WithMany().HasForeignKey(x => new { x.CloId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_course_objective_clo_clo_version");
    }
}

