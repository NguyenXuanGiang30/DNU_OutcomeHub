using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class AiSourceSnapshotConfiguration : IEntityTypeConfiguration<AiSourceSnapshot>
{
    public void Configure(EntityTypeBuilder<AiSourceSnapshot> builder)
    {
        builder.ToTable("ai_source_snapshot", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_ai_source_snapshot");

        builder.HasAlternateKey(entity => new { entity.Id, entity.SourceChecksum })
            .HasName("uq_ai_source_snapshot_id_checksum");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SourceKind)
            .HasColumnName("source_kind")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.SourceGovernedResourceId)
            .HasColumnName("source_governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DocumentVersionId)
            .HasColumnName("document_version_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ResultBatchId)
            .HasColumnName("result_batch_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ExportManifestId)
            .HasColumnName("export_manifest_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.ImprovementPlanId)
            .HasColumnName("improvement_plan_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.SourceChecksum)
            .HasColumnName("source_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.DataAsOf)
            .HasColumnName("data_as_of")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.ScopeSnapshotChecksum)
            .HasColumnName("scope_snapshot_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.PermissionSnapshotChecksum)
            .HasColumnName("permission_snapshot_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.SnapshotPayloadReference)
            .HasColumnName("snapshot_payload_reference")
            .HasColumnType("varchar(512)")
            .HasMaxLength(512)
            .IsRequired(true);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_ai_source_snapshot_governed_resource");

        builder.HasIndex(entity => new { entity.SourceKind, entity.DataAsOf })
            .HasDatabaseName("ix_ai_source_snapshot_kind_data_as_of");

        builder.HasIndex(entity => entity.SourceGovernedResourceId)
            .HasDatabaseName("ix_ai_source_snapshot_source_resource");

        builder.HasOne(entity => entity.GovernedResource)
            .WithMany()
            .HasForeignKey(entity => entity.GovernedResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_source_snapshot_governed_resource");

        builder.HasOne(entity => entity.SourceGovernedResource)
            .WithMany()
            .HasForeignKey(entity => entity.SourceGovernedResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_source_snapshot_source_governed_resource");

        builder.HasOne(entity => entity.DocumentVersion)
            .WithMany()
            .HasForeignKey(entity => entity.DocumentVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_source_snapshot_document_version");

        builder.HasOne(entity => entity.ResultBatch)
            .WithMany()
            .HasForeignKey(entity => entity.ResultBatchId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_source_snapshot_result_batch");

        builder.HasOne(entity => entity.ExportManifest)
            .WithMany()
            .HasForeignKey(entity => entity.ExportManifestId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_source_snapshot_export_manifest");

        builder.HasOne(entity => entity.ImprovementPlan)
            .WithMany()
            .HasForeignKey(entity => entity.ImprovementPlanId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_source_snapshot_improvement_plan");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_ai_source_snapshot_distinct_resource", "governed_resource_id <> source_governed_resource_id");
            tableBuilder.HasCheckConstraint(
                "ck_ai_source_snapshot_source",
                "(source_kind = 'DOCUMENT_VERSION' AND document_version_id IS NOT NULL AND num_nonnulls(result_batch_id, export_manifest_id, improvement_plan_id) = 0) OR " +
                "(source_kind = 'RESULT_BATCH' AND result_batch_id IS NOT NULL AND num_nonnulls(document_version_id, export_manifest_id, improvement_plan_id) = 0) OR " +
                "(source_kind = 'EXPORT_MANIFEST' AND export_manifest_id IS NOT NULL AND num_nonnulls(document_version_id, result_batch_id, improvement_plan_id) = 0) OR " +
                "(source_kind = 'IMPROVEMENT_PLAN' AND improvement_plan_id IS NOT NULL AND num_nonnulls(document_version_id, result_batch_id, export_manifest_id) = 0)");
            tableBuilder.HasCheckConstraint("ck_ai_source_snapshot_checksums", "source_checksum ~ '^[0-9a-f]{64}$' AND scope_snapshot_checksum ~ '^[0-9a-f]{64}$' AND permission_snapshot_checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
