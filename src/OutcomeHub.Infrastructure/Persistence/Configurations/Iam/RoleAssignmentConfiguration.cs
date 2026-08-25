using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class RoleAssignmentConfiguration : IEntityTypeConfiguration<RoleAssignment>
{
    public void Configure(EntityTypeBuilder<RoleAssignment> builder)
    {
        builder.ToTable("role_assignment", "iam", table =>
        {
            table.HasCheckConstraint("ck_role_assignment_effective_range", "effective_to > effective_from");
            table.HasCheckConstraint("ck_role_assignment_status", "status IN ('PENDING', 'ACTIVE', 'SUSPENDED', 'REVOKED')");
            table.HasCheckConstraint("ck_role_assignment_source", "source IN ('MANUAL', 'IDP_GROUP', 'IMPORT')");
            table.HasCheckConstraint("ck_role_assignment_authorization_checksum", "authorization_snapshot_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_role_assignment_approval_time", "approved_at IS NULL OR approved_at >= requested_at");
            table.HasCheckConstraint("ck_role_assignment_revocation_time", "revoked_at IS NULL OR revoked_at >= requested_at");
            table.HasCheckConstraint("ck_role_assignment_revoke_reason", "revoked_at IS NULL OR char_length(btrim(revoke_reason)) > 0");
            table.HasCheckConstraint("ck_role_assignment_reason", "char_length(btrim(reason)) > 0");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_role_assignment");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.PrincipalId).HasColumnName("principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.RoleId).HasColumnName("role_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.RoleVersionId).HasColumnName("role_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.AccessScopeId).HasColumnName("access_scope_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(entity => entity.Source).HasColumnName("source").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(entity => entity.SourceReference).HasColumnName("source_reference").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(entity => entity.GrantedBy).HasColumnName("granted_by").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ApprovedBy).HasColumnName("approved_by").HasColumnType("uuid");
        builder.Property(entity => entity.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.SodPolicyVersionId).HasColumnName("sod_policy_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.AuthorizationSnapshotChecksum).HasColumnName("authorization_snapshot_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(entity => entity.RequestedBy).HasColumnName("requested_by").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.RequestedAt).HasColumnName("requested_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.ApprovedAt).HasColumnName("approved_at").HasColumnType("timestamptz");
        builder.Property(entity => entity.RevokedAt).HasColumnName("revoked_at").HasColumnType("timestamptz");
        builder.Property(entity => entity.Reason).HasColumnName("reason").HasColumnType("text").IsRequired();
        builder.Property(entity => entity.RevokeReason).HasColumnName("revoke_reason").HasColumnType("text");

        builder.HasOne(entity => entity.Principal).WithMany().HasForeignKey(entity => entity.PrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_principal");
        builder.HasOne(entity => entity.Role).WithMany().HasForeignKey(entity => entity.RoleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_role");
        builder.HasOne(entity => entity.RoleVersion).WithMany().HasForeignKey(entity => new { entity.RoleVersionId, entity.RoleId }).HasPrincipalKey(entity => new { entity.Id, entity.RoleId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_role_version");
        builder.HasOne(entity => entity.AccessScope).WithMany().HasForeignKey(entity => entity.AccessScopeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_access_scope");
        builder.HasOne(entity => entity.GrantedByPrincipal).WithMany().HasForeignKey(entity => entity.GrantedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_granted_by");
        builder.HasOne(entity => entity.ApprovedByPrincipal).WithMany().HasForeignKey(entity => entity.ApprovedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_approved_by");
        builder.HasOne(entity => entity.WorkflowInstance).WithOne().HasForeignKey<RoleAssignment>(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_workflow_instance");
        builder.HasOne(entity => entity.SodPolicyVersion).WithMany().HasForeignKey(entity => entity.SodPolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_sod_policy_version");
        builder.HasOne(entity => entity.RequestedByPrincipal).WithMany().HasForeignKey(entity => entity.RequestedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_assignment_requested_by");

        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_role_assignment_workflow_instance");
        builder.HasIndex(entity => new { entity.PrincipalId, entity.RoleId, entity.AccessScopeId, entity.Status, entity.EffectiveFrom }).HasDatabaseName("ix_role_assignment_active_range");
        builder.HasIndex(entity => new { entity.PrincipalId, entity.Status, entity.EffectiveTo }).HasDatabaseName("ix_role_assignment_principal_status_expiry");
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_role_assignment_active_range", "principal_id WITH =, role_id WITH =, access_scope_id WITH =, tstzrange(effective_from, effective_to, '[)') WITH && WHERE (status = 'ACTIVE')");
    }
}
