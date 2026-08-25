using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class CalculationPolicyVersionConfiguration : IEntityTypeConfiguration<CalculationPolicyVersion>
{
    public void Configure(EntityTypeBuilder<CalculationPolicyVersion> builder)
    {
        builder.ToTable("calculation_policy_version", "measurement");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_calculation_policy_version");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.PolicyId)
            .HasColumnName("policy_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.VersionNo)
            .HasColumnName("version_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveFrom)
            .HasColumnName("effective_from")
            .HasColumnType("date")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveTo)
            .HasColumnName("effective_to")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.FormulaFamily)
            .HasColumnName("formula_family")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.EngineContractVersion)
            .HasColumnName("engine_contract_version")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.DirectSourceMin)
            .HasColumnName("direct_source_min")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.DirectSourceMax)
            .HasColumnName("direct_source_max")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.MissingDataRule)
            .HasColumnName("missing_data_rule")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.RepeatAttemptRule)
            .HasColumnName("repeat_attempt_rule")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.WithdrawalRule)
            .HasColumnName("withdrawal_rule")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.RecognitionRule)
            .HasColumnName("recognition_rule")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.DirectIndirectMode)
            .HasColumnName("direct_indirect_mode")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.Alpha)
            .HasColumnName("alpha")
            .HasColumnType("numeric(12,10)")
            .IsRequired(false);

        builder.Property(entity => entity.CoreGateMode)
            .HasColumnName("core_gate_mode")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.DefaultMinSampleSize)
            .HasColumnName("default_min_sample_size")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.Definition)
            .HasColumnName("definition")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.SchemaVersion)
            .HasColumnName("schema_version")
            .HasColumnType("varchar(32)")
            .HasMaxLength(32)
            .IsRequired(true);

        builder.Property(entity => entity.WorkflowInstanceId)
            .HasColumnName("workflow_instance_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.SupersedesId)
            .HasColumnName("supersedes_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(entity => new { entity.PolicyId, entity.VersionNo })
            .IsUnique()
            .HasDatabaseName("uq_calculation_policy_version_1");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_calculation_policy_version_no", "version_no > 0");
            table.HasCheckConstraint("ck_calculation_policy_version_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_calculation_policy_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            table.HasCheckConstraint("ck_calculation_policy_version_direct_indirect_mode", "direct_indirect_mode IN ('DIRECT','INDIRECT','COMBINED')");
            table.HasCheckConstraint("ck_calculation_policy_version_direct_sources", "direct_source_min >= 0 AND direct_source_max >= direct_source_min");
            table.HasCheckConstraint("ck_calculation_policy_version_alpha", "(direct_indirect_mode = 'COMBINED' AND alpha IS NOT NULL AND alpha NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND alpha >= 0 AND alpha <= 1) OR (direct_indirect_mode <> 'COMBINED' AND alpha IS NULL)");
            table.HasCheckConstraint("ck_calculation_policy_version_sample_size", "default_min_sample_size > 0");
            table.HasCheckConstraint("ck_calculation_policy_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_calculation_policy_version_workflow");
        builder.HasOne(entity => entity.Policy).WithMany(entity => entity.Versions).HasForeignKey(entity => entity.PolicyId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_calculation_policy_version_policy");
        builder.HasOne(entity => entity.WorkflowInstance).WithMany().HasForeignKey(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_calculation_policy_version_workflow");
        builder.HasOne(entity => entity.Supersedes).WithMany(entity => entity.Successors).HasForeignKey(entity => entity.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_calculation_policy_version_supersedes");
    }
}
