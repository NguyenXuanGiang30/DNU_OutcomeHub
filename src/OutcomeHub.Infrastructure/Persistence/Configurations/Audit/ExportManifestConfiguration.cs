using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Audit;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Audit;

public sealed class ExportManifestConfiguration : IEntityTypeConfiguration<ExportManifest>
{
    public void Configure(EntityTypeBuilder<ExportManifest> builder)
    {
        builder.ToTable("export_manifest", "audit", table =>
        {
            table.HasCheckConstraint("ck_export_manifest_filter_checksum", "filter_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_export_manifest_permission_checksum", "permission_snapshot_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_export_manifest_checksum", "checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_export_manifest_classification", "classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')");
            table.HasCheckConstraint("ck_export_manifest_row_count", "row_count >= 0");
            table.HasCheckConstraint("ck_export_manifest_expiry", "expires_at > created_at");
            table.HasCheckConstraint("ck_export_manifest_purpose", "char_length(btrim(purpose)) > 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_export_manifest");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RequestedBy).HasColumnName("requested_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Purpose).HasColumnName("purpose").HasColumnType("text").IsRequired();
        builder.Property(x => x.CanonicalFilter).HasColumnName("canonical_filter").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.FilterChecksum).HasColumnName("filter_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.ReportDefinitionVersion).HasColumnName("report_definition_version").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AccessScopeId).HasColumnName("access_scope_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.PermissionSnapshotChecksum).HasColumnName("permission_snapshot_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.DataAsOf).HasColumnName("data_as_of").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RowCount).HasColumnName("row_count").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.FileObjectId).HasColumnName("file_object_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Watermark).HasColumnName("watermark").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(x => x.GeneratorVersion).HasColumnName("generator_version").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_export_manifest_governed_resource");
        builder.HasOne(x => x.RequestedByPrincipal).WithMany().HasForeignKey(x => x.RequestedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_export_manifest_requested_by");
        builder.HasOne(x => x.AccessScope).WithMany().HasForeignKey(x => x.AccessScopeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_export_manifest_access_scope");
        builder.HasOne(x => x.FileObject).WithMany().HasForeignKey(x => x.FileObjectId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_export_manifest_file_object");
        builder.HasIndex(x => new { x.RequestedBy, x.CreatedAt }).HasDatabaseName("ix_export_manifest_requested_by_created_at");
        builder.HasIndex(x => x.ExpiresAt).HasDatabaseName("ix_export_manifest_expires_at");
    }
}
