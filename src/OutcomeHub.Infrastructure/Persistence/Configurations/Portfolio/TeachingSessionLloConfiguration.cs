using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class TeachingSessionLloConfiguration : IEntityTypeConfiguration<TeachingSessionLlo>
{
    public void Configure(EntityTypeBuilder<TeachingSessionLlo> builder)
    {
        builder.ToTable("teaching_session_llo", "portfolio");
        builder.HasKey(x => new { x.TeachingSessionId, x.LloId }).HasName("pk_teaching_session_llo");
        builder.Property(x => x.TeachingSessionId).HasColumnName("teaching_session_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LloId).HasColumnName("llo_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();

        builder.HasOne(x => x.TeachingSession).WithMany().HasForeignKey(x => new { x.TeachingSessionId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_llo_session_version");
        builder.HasOne(x => x.Llo).WithMany().HasForeignKey(x => new { x.LloId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_llo_llo_version");
    }
}

