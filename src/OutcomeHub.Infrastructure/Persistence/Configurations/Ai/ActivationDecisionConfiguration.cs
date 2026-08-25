using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class ActivationDecisionConfiguration : IEntityTypeConfiguration<ActivationDecision>
{
    public void Configure(EntityTypeBuilder<ActivationDecision> builder)
    {
        builder.ToTable("activation_decision", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_activation_decision");

        builder.HasAlternateKey(entity => new { entity.Id, entity.ModelDeploymentVersionId })
            .HasName("uq_activation_decision_id_model_version");

        builder.HasAlternateKey(entity => new { entity.Id, entity.PromptVersionId })
            .HasName("uq_activation_decision_id_prompt_version");

        builder.HasAlternateKey(entity => new { entity.Id, entity.OutputSchemaVersionId })
            .HasName("uq_activation_decision_id_output_schema_version");

        builder.HasAlternateKey(entity => new { entity.Id, entity.DataHandlingPolicyVersionId })
            .HasName("uq_activation_decision_id_data_policy_version");

        builder.HasAlternateKey(entity => new { entity.Id, entity.ToolPolicyVersionId })
            .HasName("uq_activation_decision_id_tool_policy_version");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.EvaluationRunId)
            .HasColumnName("evaluation_run_id")
            .HasColumnType("uuid")
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

        builder.Property(entity => entity.DecisionRecordId)
            .HasColumnName("decision_record_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ApprovedBy)
            .HasColumnName("approved_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ApprovedAt)
            .HasColumnName("approved_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => new { entity.EvaluationRunId, entity.ModelDeploymentVersionId, entity.PromptVersionId, entity.OutputSchemaVersionId, entity.DataHandlingPolicyVersionId, entity.ToolPolicyVersionId })
            .IsUnique()
            .HasDatabaseName("uq_activation_decision_exact_bundle");

        builder.HasIndex(entity => entity.DecisionRecordId)
            .HasDatabaseName("ix_activation_decision_decision_record");

        builder.HasOne(entity => entity.EvaluationRun)
            .WithMany()
            .HasForeignKey(entity => new
            {
                entity.EvaluationRunId,
                entity.ModelDeploymentVersionId,
                entity.PromptVersionId,
                entity.OutputSchemaVersionId,
                entity.DataHandlingPolicyVersionId,
                entity.ToolPolicyVersionId
            })
            .HasPrincipalKey(entity => new
            {
                entity.Id,
                entity.ModelDeploymentVersionId,
                entity.PromptVersionId,
                entity.OutputSchemaVersionId,
                entity.DataHandlingPolicyVersionId,
                entity.ToolPolicyVersionId
            })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_activation_decision_exact_evaluation_bundle");

        builder.HasOne(entity => entity.ModelDeploymentVersion)
            .WithMany()
            .HasForeignKey(entity => entity.ModelDeploymentVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_activation_decision_model_deployment_version");

        builder.HasOne(entity => entity.PromptVersion)
            .WithMany()
            .HasForeignKey(entity => new { entity.PromptVersionId, entity.OutputSchemaVersionId })
            .HasPrincipalKey(entity => new { entity.Id, entity.OutputSchemaVersionId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_activation_decision_prompt_output_schema_bundle");

        builder.HasOne(entity => entity.OutputSchemaVersion)
            .WithMany()
            .HasForeignKey(entity => entity.OutputSchemaVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_activation_decision_output_schema_version");

        builder.HasOne(entity => entity.DataHandlingPolicyVersion)
            .WithMany()
            .HasForeignKey(entity => entity.DataHandlingPolicyVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_activation_decision_data_handling_policy_version");

        builder.HasOne(entity => entity.ToolPolicyVersion)
            .WithMany()
            .HasForeignKey(entity => entity.ToolPolicyVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_activation_decision_tool_policy_version");

        builder.HasOne(entity => entity.DecisionRecord)
            .WithMany()
            .HasForeignKey(entity => entity.DecisionRecordId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_activation_decision_decision_record");

        builder.HasOne(entity => entity.Approver)
            .WithMany()
            .HasForeignKey(entity => entity.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_activation_decision_approved_by");

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint(
            "ck_activation_decision_checksum",
            "checksum ~ '^[0-9a-f]{64}$'"));
    }
}
