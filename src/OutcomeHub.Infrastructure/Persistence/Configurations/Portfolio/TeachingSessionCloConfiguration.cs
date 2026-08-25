using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class TeachingSessionCloConfiguration : IEntityTypeConfiguration<TeachingSessionClo>
{
    public void Configure(EntityTypeBuilder<TeachingSessionClo> builder)
    {
        builder.ToTable("teaching_session_clo", "portfolio");
        builder.HasKey(x => new { x.TeachingSessionId, x.CloId }).HasName("pk_teaching_session_clo");
        builder.Property(x => x.TeachingSessionId).HasColumnName("teaching_session_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CloId).HasColumnName("clo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();

        builder.HasOne(x => x.TeachingSession).WithMany().HasForeignKey(x => new { x.TeachingSessionId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_clo_session_version");
        builder.HasOne(x => x.Clo).WithMany().HasForeignKey(x => new { x.CloId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_clo_clo_version");
    }
}

