using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class IngestionBatchConfiguration : IEntityTypeConfiguration<IngestionBatch>
{
    public void Configure(EntityTypeBuilder<IngestionBatch> builder)
    {
        builder.ToTable("ingestion_batch", "integration", table =>
        {
            table.HasCheckConstraint("ck_ingestion_batch_schema_version", "schema_version > 0");
            table.HasCheckConstraint("ck_ingestion_batch_payload_checksum", "payload_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_ingestion_batch_classification", "classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')");
            table.HasCheckConstraint("ck_ingestion_batch_counts", "total_count >= 0 AND accepted_count >= 0 AND rejected_count >= 0 AND accepted_count + rejected_count <= total_count");
            table.HasCheckConstraint("ck_ingestion_batch_completion", "completed_at IS NULL OR completed_at >= received_at");
        });
        builder.HasKey(x => x.Id).HasName("pk_ingestion_batch");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SourceSystemId).HasColumnName("source_system_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.DataType).HasColumnName("data_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.SourceBatchId).HasColumnName("source_batch_id").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(x => x.IdempotencyKey).HasColumnName("idempotency_key").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.SchemaVersion).HasColumnName("schema_version").HasColumnType("integer").IsRequired();
        builder.Property(x => x.PayloadChecksum).HasColumnName("payload_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.FileObjectId).HasColumnName("file_object_id").HasColumnType("uuid");
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.ReceivedAt).HasColumnName("received_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CompletedAt).HasColumnName("completed_at").HasColumnType("timestamptz");
        builder.Property(x => x.TotalCount).HasColumnName("total_count").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.AcceptedCount).HasColumnName("accepted_count").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.RejectedCount).HasColumnName("rejected_count").HasColumnType("bigint").IsRequired();
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_ingestion_batch_governed_resource");
        builder.HasOne(x => x.SourceSystem).WithMany().HasForeignKey(x => x.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_ingestion_batch_source_system");
        builder.HasOne(x => x.FileObject).WithMany().HasForeignKey(x => x.FileObjectId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_ingestion_batch_file_object");
        builder.HasIndex(x => new { x.SourceSystemId, x.IdempotencyKey }).IsUnique().HasDatabaseName("uq_ingestion_batch_source_idempotency");
        builder.HasIndex(x => new { x.SourceSystemId, x.ReceivedAt }).HasDatabaseName("ix_ingestion_batch_source_received");
    }
}
