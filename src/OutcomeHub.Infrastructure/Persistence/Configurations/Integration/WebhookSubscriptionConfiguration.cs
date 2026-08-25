using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
{
    public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
    {
        // DNS resolution, SSRF allow-list and HTTPS certificate validation remain application/runtime checks.
        builder.ToTable("webhook_subscription", "integration", table =>
        {
            table.HasCheckConstraint("ck_webhook_subscription_key_version", "key_version > 0");
            table.HasCheckConstraint("ck_webhook_subscription_expiry", "expires_at IS NULL OR expires_at > created_at");
            table.HasCheckConstraint("ck_webhook_subscription_endpoint", "endpoint_url ~ '^https://' AND char_length(endpoint_url) <= 2048");
        });
        builder.HasKey(x => x.Id).HasName("pk_webhook_subscription");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.PrincipalId).HasColumnName("principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.AccessScopeId).HasColumnName("access_scope_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EndpointUrl).HasColumnName("endpoint_url").HasColumnType("varchar(2048)").HasMaxLength(2048).IsRequired();
        builder.Property(x => x.SecretReference).HasColumnName("secret_reference").HasColumnType("varchar(512)").HasMaxLength(512).IsRequired();
        builder.Property(x => x.SigningAlgorithm).HasColumnName("signing_algorithm").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.KeyVersion).HasColumnName("key_version").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.VerifiedAt).HasColumnName("verified_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamptz");
        builder.HasOne(x => x.Principal).WithMany().HasForeignKey(x => x.PrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_webhook_subscription_principal");
        builder.HasOne(x => x.AccessScope).WithMany().HasForeignKey(x => x.AccessScopeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_webhook_subscription_access_scope");
        builder.HasIndex(x => new { x.PrincipalId, x.Status }).HasDatabaseName("ix_webhook_subscription_principal_status");
    }
}
