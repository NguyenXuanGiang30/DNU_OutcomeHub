using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_message", "integration", table =>
        {
            table.HasCheckConstraint("ck_outbox_message_versions", "aggregate_version >= 0 AND event_schema_version > 0");
            table.HasCheckConstraint("ck_outbox_message_classification", "classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')");
            table.HasCheckConstraint("ck_outbox_message_attempt_count", "attempt_count >= 0");
            table.HasCheckConstraint("ck_outbox_message_lock", "num_nonnulls(locked_by, locked_until) IN (0, 2)");
            table.HasCheckConstraint("ck_outbox_message_published_at", "published_at IS NULL OR published_at >= occurred_at");
        });
        builder.HasKey(x => x.Id).HasName("pk_outbox_message");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.AggregateType).HasColumnName("aggregate_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.AggregateId).HasColumnName("aggregate_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AggregateVersion).HasColumnName("aggregate_version").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.EventType).HasColumnName("event_type").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired();
        builder.Property(x => x.EventSchemaVersion).HasColumnName("event_schema_version").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.Headers).HasColumnName("headers").HasColumnType("jsonb");
        builder.Property(x => x.Classification).HasColumnName("classification").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.CorrelationId).HasColumnName("correlation_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.CausationId).HasColumnName("causation_id").HasColumnType("uuid");
        builder.Property(x => x.TraceId).HasColumnName("trace_id").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.OccurredAt).HasColumnName("occurred_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.AvailableAt).HasColumnName("available_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.PublishedAt).HasColumnName("published_at").HasColumnType("timestamptz");
        builder.Property(x => x.AttemptCount).HasColumnName("attempt_count").HasColumnType("integer").IsRequired();
        builder.Property(x => x.LockedBy).HasColumnName("locked_by").HasColumnType("uuid");
        builder.Property(x => x.LockedUntil).HasColumnName("locked_until").HasColumnType("timestamptz");
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.LastErrorCode).HasColumnName("last_error_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.HasIndex(x => new { x.AvailableAt, x.OccurredAt })
            .HasFilter("published_at IS NULL")
            .HasDatabaseName("ix_outbox_message_claim");
        builder.HasIndex(x => new { x.AggregateType, x.AggregateId, x.AggregateVersion }).HasDatabaseName("ix_outbox_message_aggregate");
        builder.HasIndex(x => x.CorrelationId).HasDatabaseName("ix_outbox_message_correlation_id");
    }
}
