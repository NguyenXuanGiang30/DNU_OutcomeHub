using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class RoleVersionConfiguration : IEntityTypeConfiguration<RoleVersion>
{
    public void Configure(EntityTypeBuilder<RoleVersion> builder)
    {
        builder.ToTable("role_version", "iam", table =>
        {
            table.HasCheckConstraint("ck_role_version_version_no", "version_no > 0");
            table.HasCheckConstraint("ck_role_version_status", "status IN ('DRAFT', 'IN_REVIEW', 'APPROVED', 'ACTIVE', 'EXPIRED', 'REJECTED')");
            table.HasCheckConstraint("ck_role_version_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_role_version_permission_set_checksum", "permission_set_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_role_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_role_version");
        builder.HasAlternateKey(entity => new { entity.Id, entity.RoleId }).HasName("uq_role_version_id_role_id");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.RoleId).HasColumnName("role_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");
        builder.Property(entity => entity.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.DecisionId).HasColumnName("decision_id").HasColumnType("uuid");
        builder.Property(entity => entity.PermissionSetChecksum).HasColumnName("permission_set_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(entity => entity.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(entity => entity.CreatedBy).HasColumnName("created_by").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasOne(entity => entity.Role).WithMany(entity => entity.Versions).HasForeignKey(entity => entity.RoleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_version_role");
        builder.HasOne(entity => entity.WorkflowInstance).WithOne().HasForeignKey<RoleVersion>(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_version_workflow_instance");
        builder.HasOne(entity => entity.Decision).WithMany().HasForeignKey(entity => entity.DecisionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_version_decision");
        builder.HasOne(entity => entity.CreatedByPrincipal).WithMany().HasForeignKey(entity => entity.CreatedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_version_created_by");

        builder.HasIndex(entity => new { entity.RoleId, entity.VersionNo }).IsUnique().HasDatabaseName("uq_role_version_role_version_no");
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_role_version_workflow_instance");
        builder.HasIndex(entity => entity.DecisionId).HasDatabaseName("ix_role_version_decision");
    }
}
