using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class SyllabusEvidenceConfiguration : IEntityTypeConfiguration<SyllabusEvidence>
{
    public void Configure(EntityTypeBuilder<SyllabusEvidence> builder)
    {
        builder.ToTable("syllabus_evidence", "portfolio");
        builder.HasKey(x => new { x.SyllabusVersionId, x.EvidenceVersionId, x.LinkRole }).HasName("pk_syllabus_evidence");
        builder.Property(x => x.SyllabusVersionId).HasColumnName("syllabus_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EvidenceVersionId).HasColumnName("evidence_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LinkRole).HasColumnName("link_role").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();

        builder.HasOne(x => x.SyllabusVersion).WithMany().HasForeignKey(x => x.SyllabusVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_evidence_syllabus_version");
        builder.HasOne(x => x.EvidenceVersion).WithMany().HasForeignKey(x => x.EvidenceVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_syllabus_evidence_evidence_version");
    }
}

