using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class RawRecordConfiguration : IEntityTypeConfiguration<RawRecord>
{
    public void Configure(EntityTypeBuilder<RawRecord> builder)
    {
        // Monthly partitioning by received_at and append-only UPDATE/DELETE/TRUNCATE protection
        // require operational SQL in the migration runner once the volume threshold is reached.
        builder.ToTable("raw_record", "integration", table =>
        {
            table.HasCheckConstraint("ck_raw_record_row_no", "row_no > 0");
            table.HasCheckConstraint("ck_raw_record_payload_checksum", "payload_checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasKey(x => x.Id).HasName("pk_raw_record");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint").ValueGeneratedOnAdd();
        builder.Property(x => x.IngestionBatchId).HasColumnName("ingestion_batch_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RowNo).HasColumnName("row_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.SourceRecordId).HasColumnName("source_record_id").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(x => x.SourceUpdatedAt).HasColumnName("source_updated_at").HasColumnType("timestamptz");
        builder.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.PayloadChecksum).HasColumnName("payload_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.ReceivedAt).HasColumnName("received_at").HasColumnType("timestamptz").IsRequired();
        builder.HasOne(x => x.IngestionBatch).WithMany().HasForeignKey(x => x.IngestionBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_raw_record_ingestion_batch");
        builder.HasIndex(x => new { x.IngestionBatchId, x.RowNo }).IsUnique().HasDatabaseName("uq_raw_record_batch_row");
        builder.HasIndex(x => x.ReceivedAt).HasDatabaseName("ix_raw_record_received_at");
    }
}
