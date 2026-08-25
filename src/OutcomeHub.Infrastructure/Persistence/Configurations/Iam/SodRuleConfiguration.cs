using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class SodRuleConfiguration : IEntityTypeConfiguration<SodRule>
{
    public void Configure(EntityTypeBuilder<SodRule> builder)
    {
        builder.ToTable("sod_rule", "iam", table =>
        {
            table.HasCheckConstraint("ck_sod_rule_permissions", "permission_a_id <> permission_b_id");
            table.HasCheckConstraint("ck_sod_rule_conflict_mode", "conflict_mode IN ('SAME_RESOURCE', 'SAME_WORKFLOW_INSTANCE')");
            table.HasCheckConstraint("ck_sod_rule_severity", "severity IN ('LOW', 'MEDIUM', 'HIGH', 'CRITICAL')");
            table.HasCheckConstraint("ck_sod_rule_resource_type", "resource_type = btrim(resource_type) AND char_length(resource_type) > 0");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_sod_rule");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.PolicyVersionId).HasColumnName("policy_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ResourceType).HasColumnName("resource_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.PermissionAId).HasColumnName("permission_a_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.PermissionBId).HasColumnName("permission_b_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ConflictMode).HasColumnName("conflict_mode").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.Severity).HasColumnName("severity").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();

        builder.HasOne(entity => entity.PolicyVersion).WithMany(entity => entity.Rules).HasForeignKey(entity => entity.PolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_rule_policy_version");
        builder.HasOne(entity => entity.PermissionA).WithMany().HasForeignKey(entity => entity.PermissionAId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_rule_permission_a");
        builder.HasOne(entity => entity.PermissionB).WithMany().HasForeignKey(entity => entity.PermissionBId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_rule_permission_b");
        builder.HasIndex(entity => new { entity.PolicyVersionId, entity.ResourceType, entity.PermissionAId, entity.PermissionBId, entity.ConflictMode }).IsUnique().HasDatabaseName("uq_sod_rule_semantic");
    }
}
