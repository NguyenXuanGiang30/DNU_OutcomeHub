using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class WebhookSubscriptionEventConfiguration : IEntityTypeConfiguration<WebhookSubscriptionEvent>
{
    public void Configure(EntityTypeBuilder<WebhookSubscriptionEvent> builder)
    {
        builder.ToTable("webhook_subscription_event", "integration", table => table.HasCheckConstraint("ck_webhook_subscription_event_type", "event_type = btrim(event_type) AND char_length(event_type) > 0"));
        builder.HasKey(x => new { x.SubscriptionId, x.EventType }).HasName("pk_webhook_subscription_event");
        builder.Property(x => x.SubscriptionId).HasColumnName("subscription_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EventType).HasColumnName("event_type").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired();
        builder.HasOne(x => x.Subscription).WithMany(x => x.Events).HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_webhook_subscription_event_subscription");
    }
}
