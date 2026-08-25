using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Governance;

public sealed class RetentionPolicyVersionConfiguration : IEntityTypeConfiguration<RetentionPolicyVersion>
{
    public void Configure(EntityTypeBuilder<RetentionPolicyVersion> builder)
    {
        builder.ToTable("retention_policy_version", "governance", table =>
            {
                table.HasCheckConstraint("ck_retention_policy_version_code", "code = upper(btrim(code)) AND char_length(code) > 0");
                table.HasCheckConstraint("ck_retention_policy_version_no", "version_no > 0");
                table.HasCheckConstraint("ck_retention_policy_version_days", "retention_days >= 0");
                table.HasCheckConstraint("ck_retention_policy_version_effective_range", "effective_to IS NULL OR effective_to > effective_from");
                table.HasCheckConstraint("ck_retention_policy_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
                table.HasCheckConstraint("ck_retention_policy_version_approval", "(approved_by IS NULL) = (approved_at IS NULL)");
            });
        builder.HasKey(x => x.Id).HasName("pk_retention_policy_version");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.ResourceType).HasColumnName("resource_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.TriggerEvent).HasColumnName("trigger_event").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.RetentionDays).HasColumnName("retention_days").HasColumnType("integer").IsRequired();
        builder.Property(x => x.DispositionAction).HasColumnName("disposition_action").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(x => x.LegalBasis).HasColumnName("legal_basis").HasColumnType("text").IsRequired();
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to").HasColumnType("date").IsRequired(false);
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasColumnType("uuid").IsRequired(false);
        builder.Property(x => x.ApprovedAt).HasColumnName("approved_at").HasColumnType("timestamptz").IsRequired(false);

        builder.HasIndex(x => new { x.Code, x.VersionNo }).IsUnique().HasDatabaseName("uq_retention_policy_version_code_no");
        builder.HasIndex(x => new { x.ResourceType, x.Status }).HasDatabaseName("ix_retention_policy_version_resource_status");
        builder.HasOne(x => x.Approver).WithMany().HasForeignKey(x => x.ApprovedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_retention_policy_version_approver");
    }
}

