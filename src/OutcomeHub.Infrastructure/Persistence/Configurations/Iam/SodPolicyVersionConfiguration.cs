using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class SodPolicyVersionConfiguration : IEntityTypeConfiguration<SodPolicyVersion>
{
    public void Configure(EntityTypeBuilder<SodPolicyVersion> builder)
    {
        builder.ToTable("sod_policy_version", "iam", table =>
        {
            table.HasCheckConstraint("ck_sod_policy_version_number", "version_no > 0");
            table.HasCheckConstraint("ck_sod_policy_version_status", "status IN ('DRAFT', 'IN_REVIEW', 'APPROVED', 'ACTIVE', 'EXPIRED', 'REJECTED')");
            table.HasCheckConstraint("ck_sod_policy_version_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_sod_policy_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_sod_policy_version");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");
        builder.Property(entity => entity.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();

        builder.HasOne(entity => entity.WorkflowInstance).WithOne().HasForeignKey<SodPolicyVersion>(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sod_policy_version_workflow_instance");
        builder.HasIndex(entity => entity.VersionNo).IsUnique().HasDatabaseName("uq_sod_policy_version_version_no");
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_sod_policy_version_workflow_instance");
    }
}
