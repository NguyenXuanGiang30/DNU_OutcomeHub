using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class EvaluationPolicyVersionConfiguration : IEntityTypeConfiguration<EvaluationPolicyVersion>
{
    public void Configure(EntityTypeBuilder<EvaluationPolicyVersion> builder)
    {
        builder.ToTable("evaluation_policy_version", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_evaluation_policy_version");

        builder.HasAlternateKey(entity => new { entity.Id, entity.Checksum })
            .HasName("uq_evaluation_policy_version_id_checksum");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Code)
            .HasColumnName("code")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.VersionNo)
            .HasColumnName("version_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.MetricDefinition)
            .HasColumnName("metric_definition")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.ThresholdDefinition)
            .HasColumnName("threshold_definition")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.AggregationRule)
            .HasColumnName("aggregation_rule")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.SamplingRule)
            .HasColumnName("sampling_rule")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.Classification)
            .HasColumnName("classification")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasMaxLength(20)
            .IsRequired(true);

        builder.Property(entity => entity.WorkflowInstanceId)
            .HasColumnName("workflow_instance_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.DecisionId)
            .HasColumnName("decision_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_evaluation_policy_version_governed_resource");

        builder.HasIndex(entity => new { entity.Code, entity.VersionNo })
            .IsUnique()
            .HasDatabaseName("uq_evaluation_policy_version_code_version_no");

        builder.HasIndex(entity => entity.WorkflowInstanceId)
            .IsUnique()
            .HasDatabaseName("uq_evaluation_policy_version_workflow");

        builder.HasOne(entity => entity.GovernedResource)
            .WithMany()
            .HasForeignKey(entity => entity.GovernedResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_policy_version_governed_resource");

        builder.HasOne(entity => entity.WorkflowInstance)
            .WithMany()
            .HasForeignKey(entity => entity.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_policy_version_workflow");

        builder.HasOne(entity => entity.Decision)
            .WithMany()
            .HasForeignKey(entity => entity.DecisionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_evaluation_policy_version_decision");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_evaluation_policy_version_code", "code = upper(btrim(code)) AND char_length(code) > 0");
            tableBuilder.HasCheckConstraint("ck_evaluation_policy_version_no", "version_no > 0");
            tableBuilder.HasCheckConstraint("ck_evaluation_policy_version_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
            tableBuilder.HasCheckConstraint("ck_evaluation_policy_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            tableBuilder.HasCheckConstraint("ck_evaluation_policy_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
