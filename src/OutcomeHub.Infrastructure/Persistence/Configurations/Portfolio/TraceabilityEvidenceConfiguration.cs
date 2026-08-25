using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Portfolio;

public sealed class TraceabilityEvidenceConfiguration : IEntityTypeConfiguration<TraceabilityEvidence>
{
    public void Configure(EntityTypeBuilder<TraceabilityEvidence> builder)
    {
        builder.ToTable("traceability_evidence", "portfolio");
        builder.HasKey(x => new { x.SyllabusTraceabilityId, x.EvidenceVersionId, x.LinkRole }).HasName("pk_traceability_evidence");
        builder.Property(x => x.SyllabusTraceabilityId).HasColumnName("syllabus_traceability_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EvidenceVersionId).HasColumnName("evidence_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LinkRole).HasColumnName("link_role").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();

        builder.HasOne(x => x.SyllabusTraceability).WithMany().HasForeignKey(x => x.SyllabusTraceabilityId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_traceability_evidence_traceability");
        builder.HasOne(x => x.EvidenceVersion).WithMany().HasForeignKey(x => x.EvidenceVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_traceability_evidence_evidence_version");
    }
}

