using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class TeachingSessionAssessmentConfiguration : IEntityTypeConfiguration<TeachingSessionAssessment>
{
    public void Configure(EntityTypeBuilder<TeachingSessionAssessment> builder)
    {
        builder.ToTable("teaching_session_assessment", "portfolio");
        builder.HasKey(x => new { x.TeachingSessionId, x.AssessmentItemId }).HasName("pk_teaching_session_assessment");
        builder.Property(x => x.TeachingSessionId).HasColumnName("teaching_session_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AssessmentItemId).HasColumnName("assessment_item_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();

        builder.HasOne(x => x.TeachingSession).WithMany().HasForeignKey(x => new { x.TeachingSessionId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_assessment_session_version");
        builder.HasOne(x => x.AssessmentItem).WithMany().HasForeignKey(x => new { x.AssessmentItemId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_assessment_item_version");
    }
}

