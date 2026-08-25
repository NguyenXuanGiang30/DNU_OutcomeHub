using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_message", "integration", table =>
        {
            table.HasCheckConstraint("ck_inbox_message_schema_version", "event_schema_version > 0");
            table.HasCheckConstraint("ck_inbox_message_checksum", "payload_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_inbox_message_classification", "classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')");
            table.HasCheckConstraint("ck_inbox_message_attempt_count", "attempt_count >= 0");
            table.HasCheckConstraint("ck_inbox_message_lock", "num_nonnulls(locked_by, locked_until) IN (0, 2)");
            table.HasCheckConstraint("ck_inbox_message_processed_at", "processed_at IS NULL OR processed_at >= received_at");
        });
        builder.HasKey(x => x.Id).HasName("pk_inbox_message");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.SourceSystemId).HasColumnName("source_system_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.MessageId).HasColumnName("message_id").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.MessageType).HasColumnName("message_type").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired();
        builder.Property(x => x.EventSchemaVersion).HasColumnName("event_schema_version").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.PayloadChecksum).HasColumnName("payload_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.SignatureKeyVersion).HasColumnName("signature_key_version").HasColumnType("integer").IsRequired();
        builder.Property(x => x.SignatureValid).HasColumnName("signature_valid").HasColumnType("boolean").IsRequired();
        builder.Property(x => x.Nonce).HasColumnName("nonce").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.SourceTimestamp).HasColumnName("source_timestamp").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ReceivedAt).HasColumnName("received_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ProcessedAt).HasColumnName("processed_at").HasColumnType("timestamptz");
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.AttemptCount).HasColumnName("attempt_count").HasColumnType("integer").IsRequired();
        builder.Property(x => x.LockedBy).HasColumnName("locked_by").HasColumnType("uuid");
        builder.Property(x => x.LockedUntil).HasColumnName("locked_until").HasColumnType("timestamptz");
        builder.Property(x => x.ErrorCode).HasColumnName("error_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.HasOne(x => x.SourceSystem).WithMany().HasForeignKey(x => x.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_inbox_message_source_system");
        builder.HasIndex(x => new { x.SourceSystemId, x.MessageId }).IsUnique().HasDatabaseName("uq_inbox_message_source_message");
        builder.HasIndex(x => new { x.SourceSystemId, x.Nonce }).IsUnique().HasDatabaseName("uq_inbox_message_source_nonce");
        builder.HasIndex(x => new { x.Status, x.LockedUntil, x.ReceivedAt }).HasDatabaseName("ix_inbox_message_claim");
    }
}
