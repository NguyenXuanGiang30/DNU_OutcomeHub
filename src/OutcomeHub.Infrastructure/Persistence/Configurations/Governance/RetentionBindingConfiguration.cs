using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class RetentionBindingConfiguration : IEntityTypeConfiguration<RetentionBinding>
{
    public void Configure(EntityTypeBuilder<RetentionBinding> builder)
    {
        builder.ToTable("retention_binding", "governance", table =>
            {
                table.HasCheckConstraint("ck_retention_binding_range", "calculated_until >= trigger_event_at");
                table.HasCheckConstraint("ck_retention_binding_status", "status IN ('ACTIVE','SUPERSEDED','ON_HOLD','ELIGIBLE','DISPOSED')");
            });
        builder.HasKey(x => x.Id).HasName("pk_retention_binding");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.RetentionPolicyVersionId).HasColumnName("retention_policy_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.TriggerEventAt).HasColumnName("trigger_event_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CalculatedUntil).HasColumnName("calculated_until").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.SourceReason).HasColumnName("source_reason").HasColumnType("text").IsRequired();

        builder.HasIndex(x => new { x.GovernedResourceId, x.RetentionPolicyVersionId, x.TriggerEventAt }).IsUnique().HasDatabaseName("uq_retention_binding_resource_policy_trigger");
        builder.HasIndex(x => new { x.Status, x.CalculatedUntil }).HasDatabaseName("ix_retention_binding_status_until");
        builder.HasOne(x => x.GovernedResource).WithMany(x => x.RetentionBindings).HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_retention_binding_resource");
        builder.HasOne(x => x.RetentionPolicyVersion).WithMany().HasForeignKey(x => x.RetentionPolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_retention_binding_policy_version");
    }
}

