using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class GroundTruthSuiteVersionConfiguration : IEntityTypeConfiguration<GroundTruthSuiteVersion>
{
    public void Configure(EntityTypeBuilder<GroundTruthSuiteVersion> builder)
    {
        builder.ToTable("ground_truth_suite_version", "ai");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_ground_truth_suite_version");

        builder.HasAlternateKey(entity => new { entity.Id, entity.Checksum })
            .HasName("uq_ground_truth_suite_version_id_checksum");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SuiteId)
            .HasColumnName("suite_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.VersionNo)
            .HasColumnName("version_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.JobType)
            .HasColumnName("job_type")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
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

        builder.Property(entity => entity.EffectiveFrom)
            .HasColumnName("effective_from")
            .HasColumnType("date")
            .IsRequired(true);

        builder.Property(entity => entity.EffectiveTo)
            .HasColumnName("effective_to")
            .HasColumnType("date")
            .IsRequired(false);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_ground_truth_suite_version_governed_resource");

        builder.HasIndex(entity => new { entity.SuiteId, entity.VersionNo })
            .IsUnique()
            .HasDatabaseName("uq_ground_truth_suite_version_suite_version_no");

        builder.HasIndex(entity => entity.WorkflowInstanceId)
            .IsUnique()
            .HasDatabaseName("uq_ground_truth_suite_version_workflow");

        builder.HasOne(entity => entity.GovernedResource)
            .WithMany()
            .HasForeignKey(entity => entity.GovernedResourceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ground_truth_suite_version_governed_resource");

        builder.HasOne(entity => entity.Suite)
            .WithMany()
            .HasForeignKey(entity => entity.SuiteId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ground_truth_suite_version_suite");

        builder.HasOne(entity => entity.WorkflowInstance)
            .WithMany()
            .HasForeignKey(entity => entity.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ground_truth_suite_version_workflow");

        builder.HasOne(entity => entity.Decision)
            .WithMany()
            .HasForeignKey(entity => entity.DecisionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ground_truth_suite_version_decision");

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("ck_ground_truth_suite_version_no", "version_no > 0");
            tableBuilder.HasCheckConstraint("ck_ground_truth_suite_version_job_type", "job_type IN ('EXTRACT','GENERATE','CHAT','DETECT_ANOMALY')");
            tableBuilder.HasCheckConstraint("ck_ground_truth_suite_version_classification", "classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')");
            tableBuilder.HasCheckConstraint("ck_ground_truth_suite_version_status", "status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')");
            tableBuilder.HasCheckConstraint("ck_ground_truth_suite_version_range", "effective_to IS NULL OR effective_to > effective_from");
            tableBuilder.HasCheckConstraint("ck_ground_truth_suite_version_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
    }
}
