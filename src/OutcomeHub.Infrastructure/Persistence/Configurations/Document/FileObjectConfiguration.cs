using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Document;

public sealed class FileObjectConfiguration : IEntityTypeConfiguration<FileObject>
{
    public void Configure(EntityTypeBuilder<FileObject> builder)
    {
        builder.ToTable("file_object", "document", table =>
            {
                table.HasCheckConstraint("ck_file_object_size", "size_bytes >= 0");
                table.HasCheckConstraint("ck_file_object_sha256", "sha256 ~ '^[0-9a-f]{64}$'");
                table.HasCheckConstraint("ck_file_object_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
                table.HasCheckConstraint("ck_file_object_malware_scan_status", "malware_scan_status IN ('PENDING','SCANNING','CLEAN','INFECTED','ERROR')");
                table.HasCheckConstraint("ck_file_object_scan_metadata", "malware_scan_at IS NULL OR malware_scan_engine IS NOT NULL");
            });
        builder.HasKey(x => x.Id).HasName("pk_file_object");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.StorageProvider).HasColumnName("storage_provider").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Bucket).HasColumnName("bucket").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.ObjectKey).HasColumnName("object_key").HasColumnType("varchar(1024)").HasMaxLength(1024).IsRequired();
        builder.Property(x => x.StorageVersion).HasColumnName("storage_version").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.OriginalFilename).HasColumnName("original_filename").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.DeclaredMediaType).HasColumnName("declared_media_type").HasColumnType("varchar(127)").HasMaxLength(127).IsRequired();
        builder.Property(x => x.DetectedMediaType).HasColumnName("detected_media_type").HasColumnType("varchar(127)").HasMaxLength(127).IsRequired(false);
        builder.Property(x => x.SizeBytes).HasColumnName("size_bytes").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.Sha256).HasColumnName("sha256").HasColumnType("char(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(16)").HasMaxLength(16).IsRequired();
        builder.Property(x => x.MalwareScanStatus).HasColumnName("malware_scan_status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.MalwareScanEngine).HasColumnName("malware_scan_engine").HasColumnType("varchar(127)").HasMaxLength(127).IsRequired(false);
        builder.Property(x => x.MalwareScanVersion).HasColumnName("malware_scan_version").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired(false);
        builder.Property(x => x.MalwareScanAt).HasColumnName("malware_scan_at").HasColumnType("timestamptz").IsRequired(false);
        builder.Property(x => x.EncryptionKeyReference).HasColumnName("encryption_key_reference").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired(false);
        builder.Property(x => x.PurgedAt).HasColumnName("purged_at").HasColumnType("timestamptz").IsRequired(false);
        builder.Property(x => x.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(x => x.GovernedResourceId).IsUnique().HasDatabaseName("uq_file_object_governed_resource");
        builder.HasIndex(x => new { x.StorageProvider, x.Bucket, x.ObjectKey, x.StorageVersion }).IsUnique().HasDatabaseName("uq_file_object_storage_identity");
        builder.HasIndex(x => x.Sha256).HasDatabaseName("ix_file_object_sha256");
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_file_object_governed_resource");
        builder.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_file_object_creator");
    }
}

