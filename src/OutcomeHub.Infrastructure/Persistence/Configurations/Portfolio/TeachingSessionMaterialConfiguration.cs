using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class TeachingSessionMaterialConfiguration : IEntityTypeConfiguration<TeachingSessionMaterial>
{
    public void Configure(EntityTypeBuilder<TeachingSessionMaterial> builder)
    {
        builder.ToTable("teaching_session_material", "portfolio");
        builder.HasKey(x => new { x.TeachingSessionId, x.LearningMaterialId }).HasName("pk_teaching_session_material");
        builder.Property(x => x.TeachingSessionId).HasColumnName("teaching_session_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LearningMaterialId).HasColumnName("learning_material_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();

        builder.HasOne(x => x.TeachingSession).WithMany().HasForeignKey(x => new { x.TeachingSessionId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_material_session_version");
        builder.HasOne(x => x.LearningMaterial).WithMany().HasForeignKey(x => new { x.LearningMaterialId, x.SyllabusVersionId }).HasPrincipalKey(x => new { x.Id, x.SyllabusVersionId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_teaching_session_material_material_version");
    }
}

