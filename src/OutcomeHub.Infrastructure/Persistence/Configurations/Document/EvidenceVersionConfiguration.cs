using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Document;

public sealed class EvidenceVersionConfiguration : IEntityTypeConfiguration<EvidenceVersion>
{
    public void Configure(EntityTypeBuilder<EvidenceVersion> builder)
    {
        builder.ToTable("evidence_version", "document", table =>
            {
                table.HasCheckConstraint("ck_evidence_version_no", "version_no > 0");
                table.HasCheckConstraint("ck_evidence_version_source", "num_nonnulls(document_version_id, external_url, system_record_reference) = 1");
                table.HasCheckConstraint("ck_evidence_version_url_snapshot", "url_snapshot_file_object_id IS NULL OR external_url IS NOT NULL");
                table.HasCheckConstraint("ck_evidence_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
                table.HasCheckConstraint("ck_evidence_version_approval", "(approved_by IS NULL) = (approved_at IS NULL)");
            });
        builder.HasKey(x => x.Id).HasName("pk_evidence_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EvidenceId).HasColumnName("evidence_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.DocumentVersionId).HasColumnName("document_version_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ExternalUrl).HasColumnName("external_url").HasColumnType("varchar(2048)").HasMaxLength(2048).IsRequired(false);
        builder.Property(x => x.UrlSnapshotFileObjectId).HasColumnName("url_snapshot_file_object_id").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.SystemRecordReference).HasColumnName("system_record_reference").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired(false);
        builder.Property(x => x.CollectedAt).HasColumnName("collected_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Metadata).HasColumnName("metadata").HasColumnType("jsonb").IsRequired(false);
        builder.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ApprovedAt).HasColumnName("approved_at").HasColumnType("timestamptz").IsRequired(false);
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => x.GovernedResourceId).IsUnique().HasDatabaseName("uq_evidence_version_governed_resource");
        builder.HasIndex(x => new { x.EvidenceId, x.VersionNo }).IsUnique().HasDatabaseName("uq_evidence_version_evidence_no");
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_version_governed_resource");
        builder.HasOne(x => x.Evidence).WithMany(x => x.Versions).HasForeignKey(x => x.EvidenceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_version_evidence");
        builder.HasOne(x => x.DocumentVersion).WithMany().HasForeignKey(x => x.DocumentVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_version_document_version");
        builder.HasOne(x => x.UrlSnapshotFileObject).WithMany().HasForeignKey(x => x.UrlSnapshotFileObjectId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_version_url_snapshot_file");
        builder.HasOne(x => x.Approver).WithMany().HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_version_approver");
        builder.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_evidence_version_creator");
    }
}

