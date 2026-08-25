using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class AiJobConfiguration : IEntityTypeConfiguration<AiJob>
{
    public void Configure(EntityTypeBuilder<AiJob> builder)
    {
        builder.ToTable("ai_job", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_ai_job");

        builder.HasAlternateKey(entity => new { entity.Id, entity.TargetResourceType, entity.TargetResourceId })
            .HasName("uq_ai_job_id_target");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.JobType)
            .HasColumnName("job_type")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
            .IsRequired(true);

        builder.Property(entity => entity.Classification)
            .HasColumnName("classification")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.RequestedBy)
            .HasColumnName("requested_by")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AccessScopeId)
            .HasColumnName("access_scope_id")
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

        builder.Property(entity => entity.GenerationParameters)
            .HasColumnName("generation_parameters")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.InputChecksum)
            .HasColumnName("input_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.RequestId)
            .HasColumnName("request_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.CorrelationId)
            .HasColumnName("correlation_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.QueuedAt)
            .HasColumnName("queued_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.Property(entity => entity.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.InputTokens)
            .HasColumnName("input_tokens")
            .HasColumnType("bigint")
            .IsRequired(false);

        builder.Property(entity => entity.OutputTokens)
            .HasColumnName("output_tokens")
            .HasColumnType("bigint")
            .IsRequired(false);

        builder.Property(entity => entity.EstimatedCost)
            .HasColumnName("estimated_cost")
            .HasColumnType("numeric(20,10)")
            .HasPrecision(20, 10)
            .IsRequired(false);

        builder.Property(entity => entity.ErrorCode)
            .HasColumnName("error_code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.ErrorDetailRedacted)
            .HasColumnName("error_detail_redacted")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.TargetResourceType)
            .HasColumnName("target_resource_type")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.TargetResourceId)
            .HasColumnName("target_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.TargetResourceVersion)
            .HasColumnName("target_resource_version")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.TargetContentChecksum)
            .HasColumnName("target_content_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.TargetRowVersion)
            .HasColumnName("target_row_version")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_ai_job_governed_resource");

        builder.HasIndex(entity => entity.RequestId)
            .IsUnique()
            .HasDatabaseName("uq_ai_job_request_id");

        builder.HasIndex(entity => new { entity.Status, entity.QueuedAt })
            .HasDatabaseName("ix_ai_job_status_queued_at");

        builder.HasIndex(entity => entity.CorrelationId)
            .HasDatabaseName("ix_ai_job_correlation_id");

        builder.HasOne(entity => entity.GovernedResource)
            .WithMany()
            .HasForeignKey(entity => entity.GovernedResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_governed_resource");

        builder.HasOne(entity => entity.RequestedByPrincipal)
            .WithMany()
            .HasForeignKey(entity => entity.RequestedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_requested_by");

        builder.HasOne(entity => entity.AccessScope)
            .WithMany()
            .HasForeignKey(entity => entity.AccessScopeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_access_scope");

        builder.HasOne(entity => entity.ModelDeploymentVersion)
            .WithMany()
            .HasForeignKey(entity => entity.ModelDeploymentVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_model_deployment_version");

        builder.HasOne(entity => entity.PromptVersion)
            .WithMany()
            .HasForeignKey(entity => new { entity.PromptVersionId, entity.OutputSchemaVersionId })
            .HasPrincipalKey(entity => new { entity.Id, entity.OutputSchemaVersionId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_prompt_output_schema_bundle");

        builder.HasOne(entity => entity.OutputSchemaVersion)
            .WithMany()
            .HasForeignKey(entity => entity.OutputSchemaVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_output_schema_version");

        builder.HasOne(entity => entity.DataHandlingPolicyVersion)
            .WithMany()
            .HasForeignKey(entity => entity.DataHandlingPolicyVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_data_handling_policy_version");

        builder.HasOne(entity => entity.ToolPolicyVersion)
            .WithMany()
            .HasForeignKey(entity => entity.ToolPolicyVersionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_job_tool_policy_version");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_ai_job_job_type", "job_type IN ('EXTRACT','GENERATE','CHAT','DETECT_ANOMALY')");
            tableBuilder.HasCheckConstraint("ck_ai_job_status", "status IN ('QUEUED','RUNNING','NEEDS_REVIEW','PARTIAL','ACCEPTED','REJECTED','APPLIED','FAILED','CANCELLED')");
            tableBuilder.HasCheckConstraint("ck_ai_job_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
            tableBuilder.HasCheckConstraint("ck_ai_job_timestamps", "(started_at IS NULL OR started_at >= queued_at) AND (completed_at IS NULL OR completed_at >= COALESCE(started_at, queued_at))");
            tableBuilder.HasCheckConstraint("ck_ai_job_status_timestamps", "(status = 'QUEUED' AND started_at IS NULL AND completed_at IS NULL) OR (status = 'RUNNING' AND started_at IS NOT NULL AND completed_at IS NULL) OR (status = 'CANCELLED' AND completed_at IS NOT NULL) OR (status NOT IN ('QUEUED','RUNNING','CANCELLED') AND started_at IS NOT NULL AND completed_at IS NOT NULL)");
            tableBuilder.HasCheckConstraint("ck_ai_job_failure", "status <> 'FAILED' OR error_code IS NOT NULL");
            tableBuilder.HasCheckConstraint("ck_ai_job_token_counts", "(input_tokens IS NULL OR input_tokens >= 0) AND (output_tokens IS NULL OR output_tokens >= 0)");
            tableBuilder.HasCheckConstraint("ck_ai_job_estimated_cost", "estimated_cost IS NULL OR (estimated_cost >= 0 AND estimated_cost <> 'NaN'::numeric AND estimated_cost NOT IN ('Infinity'::numeric, '-Infinity'::numeric))");
            tableBuilder.HasCheckConstraint("ck_ai_job_target_versions", "target_resource_version >= 0 AND target_row_version >= 0");
            tableBuilder.HasCheckConstraint("ck_ai_job_checksums", "input_checksum ~ '^[0-9a-f]{64}$' AND target_content_checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
