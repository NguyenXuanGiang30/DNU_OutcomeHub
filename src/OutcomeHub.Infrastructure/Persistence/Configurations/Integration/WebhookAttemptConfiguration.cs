using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class WebhookAttemptConfiguration : IEntityTypeConfiguration<WebhookAttempt>
{
    public void Configure(EntityTypeBuilder<WebhookAttempt> builder)
    {
        // The signed canonical string and nonce/timestamp verification need integration tests and runtime validation.
        builder.ToTable("webhook_attempt", "integration", table =>
        {
            table.HasCheckConstraint("ck_webhook_attempt_number", "attempt_no > 0");
            table.HasCheckConstraint("ck_webhook_attempt_response_status", "response_status IS NULL OR response_status BETWEEN 100 AND 599");
            table.HasCheckConstraint("ck_webhook_attempt_response_time", "response_at IS NULL OR response_at >= requested_at");
        });
        builder.HasKey(x => new { x.DeliveryId, x.AttemptNo }).HasName("pk_webhook_attempt");
        builder.Property(x => x.DeliveryId).HasColumnName("delivery_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AttemptNo).HasColumnName("attempt_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Nonce).HasColumnName("nonce").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Signature).HasColumnName("signature").HasColumnType("varchar(512)").HasMaxLength(512).IsRequired();
        builder.Property(x => x.RequestedAt).HasColumnName("requested_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ResponseStatus).HasColumnName("response_status").HasColumnType("integer");
        builder.Property(x => x.ResponseAt).HasColumnName("response_at").HasColumnType("timestamptz");
        builder.Property(x => x.ErrorCode).HasColumnName("error_code").HasColumnType("varchar(64)").HasMaxLength(64);
        builder.Property(x => x.ResponseExcerpt).HasColumnName("response_excerpt").HasColumnType("varchar(2048)").HasMaxLength(2048);
        builder.HasOne(x => x.Delivery).WithMany(x => x.Attempts).HasForeignKey(x => x.DeliveryId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_webhook_attempt_delivery");
        builder.HasIndex(x => new { x.DeliveryId, x.Nonce }).IsUnique().HasDatabaseName("uq_webhook_attempt_delivery_nonce");
    }
}
