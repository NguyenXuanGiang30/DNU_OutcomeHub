using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Document;

public sealed class EvidenceLinkConfiguration : IEntityTypeConfiguration<EvidenceLink>
{
    public void Configure(EntityTypeBuilder<EvidenceLink> builder)
    {
        builder.ToTable("evidence_link", "document");
        builder.HasKey(x => new { x.EvidenceVersionId, x.ResourceType, x.ResourceId, x.LinkRole }).HasName("pk_evidence_link");
        builder.Property(x => x.EvidenceVersionId).HasColumnName("evidence_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ResourceType).HasColumnName("resource_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ResourceId).HasColumnName("resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LinkRole).HasColumnName("link_role").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => new { x.ResourceType, x.ResourceId }).HasDatabaseName("ix_evidence_link_resource");
        builder.HasOne(x => x.EvidenceVersion).WithMany().HasForeignKey(x => x.EvidenceVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_link_evidence_version");
    }
}

