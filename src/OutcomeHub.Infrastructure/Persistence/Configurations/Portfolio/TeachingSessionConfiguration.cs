using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class TeachingSessionConfiguration : IEntityTypeConfiguration<TeachingSession>
{
    public void Configure(EntityTypeBuilder<TeachingSession> builder)
    {
        builder.ToTable("teaching_session", "portfolio", table =>
            {
                table.HasCheckConstraint("ck_teaching_session_session_no", "session_no > 0");
                table.HasCheckConstraint("ck_teaching_session_planned_hours", "planned_hours NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND planned_hours > 0");
                table.HasCheckConstraint("ck_teaching_session_sort_order", "sort_order >= 0");
            });
        builder.HasKey(x => x.Id).HasName("pk_teaching_session");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SessionNo).HasColumnName("session_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.PlannedHours).HasColumnName("planned_hours").HasColumnType("numeric(10,2)").HasPrecision(10, 2).IsRequired();
        builder.Property(x => x.TeachingMethod).HasColumnName("teaching_method").HasColumnType("text").IsRequired();
        builder.Property(x => x.AssessmentMethod).HasColumnName("assessment_method").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.SelfStudyTask).HasColumnName("self_study_task").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("integer").IsRequired();

        builder.HasAlternateKey(x => new { x.Id, x.SyllabusVersionId }).HasName("uq_teaching_session_id_version");
        builder.HasIndex(x => new { x.SyllabusVersionId, x.SessionNo }).IsUnique().HasDatabaseName("uq_teaching_session_version_no");
        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_syllabus_version");
    }
}
