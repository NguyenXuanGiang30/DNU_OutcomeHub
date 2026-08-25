using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
{
    public void Configure(EntityTypeBuilder<WebhookDelivery> builder)
    {
        builder.ToTable("webhook_delivery", "integration", table =>
        {
            table.HasCheckConstraint("ck_webhook_delivery_checksum", "payload_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_webhook_delivery_attempt_count", "attempt_count >= 0");
        });
        builder.HasKey(x => x.Id).HasName("pk_webhook_delivery");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.SubscriptionId).HasColumnName("subscription_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.OutboxMessageId).HasColumnName("outbox_message_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.PayloadChecksum).HasColumnName("payload_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.AttemptCount).HasColumnName("attempt_count").HasColumnType("integer").IsRequired();
        builder.Property(x => x.NextRetryAt).HasColumnName("next_retry_at").HasColumnType("timestamptz");
        builder.Property(x => x.DeliveredAt).HasColumnName("delivered_at").HasColumnType("timestamptz");
        builder.HasOne(x => x.Subscription).WithMany(x => x.Deliveries).HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_webhook_delivery_subscription");
        builder.HasOne(x => x.OutboxMessage).WithMany().HasForeignKey(x => x.OutboxMessageId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_webhook_delivery_outbox_message");
        builder.HasIndex(x => new { x.SubscriptionId, x.OutboxMessageId }).IsUnique().HasDatabaseName("uq_webhook_delivery_subscription_outbox");
        builder.HasIndex(x => new { x.Status, x.NextRetryAt }).HasDatabaseName("ix_webhook_delivery_retry");
    }
}
