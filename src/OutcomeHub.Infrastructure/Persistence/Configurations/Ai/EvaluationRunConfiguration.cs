using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class EvaluationRunConfiguration : IEntityTypeConfiguration<EvaluationRun>
{
    public void Configure(EntityTypeBuilder<EvaluationRun> builder)
    {
        builder.ToTable("evaluation_run", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_evaluation_run");

        builder.HasAlternateKey(entity => new
        {
            entity.Id,
            entity.ModelDeploymentVersionId,
            entity.PromptVersionId,
            entity.OutputSchemaVersionId,
            entity.DataHandlingPolicyVersionId,
            entity.ToolPolicyVersionId
        }).HasName("uq_evaluation_run_exact_bundle");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SuiteVersionId)
            .HasColumnName("suite_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SuiteChecksum)
            .HasColumnName("suite_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.EvaluationPolicyVersionId)
            .HasColumnName("evaluation_policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EvaluationPolicyChecksum)
            .HasColumnName("evaluation_policy_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.ModelDeploymentVersionId)
            .HasColumnName("model_deployment_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PromptVersionId)
            .HasColumnName("prompt_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OutputSchemaVersionId)
            .HasColumnName("output_schema_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DataHandlingPolicyVersionId)
            .HasColumnName("data_handling_policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ToolPolicyVersionId)
            .HasColumnName("tool_policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ConfigBundleChecksum)
            .HasColumnName("config_bundle_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.ResultChecksum)
            .HasColumnName("result_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_evaluation_run_governed_resource");

        builder.HasIndex(entity => new { entity.Status, entity.StartedAt })
            .HasDatabaseName("ix_evaluation_run_status_started_at");

        builder.HasOne(entity => entity.GovernedResource)
            .WithMany()
            .HasForeignKey(entity => entity.GovernedResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_run_governed_resource");

        builder.HasOne(entity => entity.SuiteVersion)
            .WithMany()
            .HasForeignKey(entity => new { entity.SuiteVersionId, entity.SuiteChecksum })
            .HasPrincipalKey(entity => new { entity.Id, entity.Checksum })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_run_suite_checksum");

        builder.HasOne(entity => entity.EvaluationPolicyVersion)
            .WithMany()
            .HasForeignKey(entity => new { entity.EvaluationPolicyVersionId, entity.EvaluationPolicyChecksum })
            .HasPrincipalKey(entity => new { entity.Id, entity.Checksum })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_run_policy_checksum");

        builder.HasOne(entity => entity.ModelDeploymentVersion)
            .WithMany()
            .HasForeignKey(entity => entity.ModelDeploymentVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_run_model_deployment_version");

        builder.HasOne(entity => entity.PromptVersion)
            .WithMany()
            .HasForeignKey(entity => new { entity.PromptVersionId, entity.OutputSchemaVersionId })
            .HasPrincipalKey(entity => new { entity.Id, entity.OutputSchemaVersionId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_run_prompt_output_schema_bundle");

        builder.HasOne(entity => entity.OutputSchemaVersion)
            .WithMany()
            .HasForeignKey(entity => entity.OutputSchemaVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_run_output_schema_version");

        builder.HasOne(entity => entity.DataHandlingPolicyVersion)
            .WithMany()
            .HasForeignKey(entity => entity.DataHandlingPolicyVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_run_data_handling_policy_version");

        builder.HasOne(entity => entity.ToolPolicyVersion)
            .WithMany()
            .HasForeignKey(entity => entity.ToolPolicyVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_run_tool_policy_version");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_evaluation_run_status", "status IN ('RUNNING','PASSED','FAILED','CANCELLED')");
            tableBuilder.HasCheckConstraint("ck_evaluation_run_time", "completed_at IS NULL OR completed_at >= started_at");
            tableBuilder.HasCheckConstraint("ck_evaluation_run_completion", "status = 'RUNNING' OR completed_at IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_evaluation_run_result_checksum", "status <> 'PASSED' OR result_checksum IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_evaluation_run_checksums", "suite_checksum ~ '^[0-9a-f]{64}$' AND evaluation_policy_checksum ~ '^[0-9a-f]{64}$' AND config_bundle_checksum ~ '^[0-9a-f]{64}$' AND (result_checksum IS NULL OR result_checksum ~ '^[0-9a-f]{64}$')");
        });
    }
}
