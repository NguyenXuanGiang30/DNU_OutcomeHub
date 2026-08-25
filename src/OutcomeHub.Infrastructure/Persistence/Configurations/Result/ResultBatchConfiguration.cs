using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Result;

public sealed class ResultBatchConfiguration : IEntityTypeConfiguration<ResultBatch>
{
    public void Configure(EntityTypeBuilder<ResultBatch> builder)
    {
        builder.ToTable("result_batch", "result");

        builder.HasKey(entity => entity.Id)
            .HasName("pk_result_batch");

        builder.HasAlternateKey(entity => new { entity.Id, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId })
            .HasName("uq_result_batch_scope_covering");
        builder.HasAlternateKey(entity => new { entity.Id, entity.InputSnapshotId, entity.AcademicYearStart, entity.OrgUnitId, entity.ProgramVersionId, entity.MeasurementPeriodId })
            .HasName("uq_result_batch_snapshot_scope_covering");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever()
            .IsRequired(true);

        builder.Property(entity => entity.GovernedResourceId)
            .HasColumnName("governed_resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.MeasurementPeriodId)
            .HasColumnName("measurement_period_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.PolicyVersionId)
            .HasColumnName("policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramPolicyBindingId)
            .HasColumnName("program_policy_binding_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.OrgUnitId)
            .HasColumnName("org_unit_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ProgramVersionId)
            .HasColumnName("program_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.AcademicYearStart)
            .HasColumnName("academic_year_start")
            .HasColumnType("smallint")
            .IsRequired(true);

        builder.Property(entity => entity.BatchNo)
            .HasColumnName("batch_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.EngineVersion)
            .HasColumnName("engine_version")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.SourceCommit)
            .HasColumnName("source_commit")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.ContainerDigest)
            .HasColumnName("container_digest")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(24)")
            .HasMaxLength(24)
            .IsRequired(true);

        builder.Property(entity => entity.IdempotencyKey)
            .HasColumnName("idempotency_key")
            .HasColumnType("varchar(128)")
            .HasMaxLength(128)
            .IsRequired(true);

        builder.Property(entity => entity.RequestChecksum)
            .HasColumnName("request_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.RecalculatesBatchId)
            .HasColumnName("recalculates_batch_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(entity => entity.RecalculationReason)
            .HasColumnName("recalculation_reason")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(entity => entity.WorkflowInstanceId)
            .HasColumnName("workflow_instance_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.SodPolicyVersionId)
            .HasColumnName("sod_policy_version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ResultChecksum)
            .HasColumnName("result_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(entity => entity.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(entity => entity.PublishedAt)
            .HasColumnName("published_at")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.HasIndex(entity => entity.GovernedResourceId)
            .IsUnique()
            .HasDatabaseName("uq_result_batch_1");

        builder.HasIndex(entity => new { entity.MeasurementPeriodId, entity.BatchNo })
            .IsUnique()
            .HasDatabaseName("uq_result_batch_2");

        builder.HasIndex(entity => new { entity.MeasurementPeriodId, entity.IdempotencyKey })
            .IsUnique()
            .HasDatabaseName("uq_result_batch_3");

        builder.HasIndex(entity => new { entity.Id, entity.MeasurementPeriodId })
            .IsUnique()
            .HasDatabaseName("uq_result_batch_4");

        builder.HasIndex(entity => new { entity.Id, entity.AcademicYearStart })
            .IsUnique()
            .HasDatabaseName("uq_result_batch_5");

        builder.HasIndex(entity => new { entity.Id, entity.InputSnapshotId, entity.AcademicYearStart })
            .IsUnique()
            .HasDatabaseName("uq_result_batch_6");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_result_batch_batch_no", "batch_no > 0");
            table.HasCheckConstraint("ck_result_batch_status", "status IN ('QUEUED', 'RUNNING', 'CALCULATED', 'VALIDATED', 'IN_REVIEW', 'APPROVED', 'PUBLISHED', 'FAILED', 'CANCELLED')");
            table.HasCheckConstraint("ck_result_batch_request_checksum", "request_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_result_batch_result_checksum", "result_checksum IS NULL OR result_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_result_batch_recalculation", "num_nonnulls(recalculates_batch_id, recalculation_reason) IN (0, 2)");
            table.HasCheckConstraint("ck_result_batch_no_self_recalculation", "recalculates_batch_id IS NULL OR recalculates_batch_id <> id");
            table.HasCheckConstraint("ck_result_batch_times", "(completed_at IS NULL OR (started_at IS NOT NULL AND completed_at >= started_at)) AND (published_at IS NULL OR (completed_at IS NOT NULL AND published_at >= completed_at))");
        });

        builder.HasOne(entity => entity.GovernedResource).WithMany().HasForeignKey(entity => entity.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_governed_resource");
        builder.HasOne(entity => entity.InputSnapshot).WithMany()
            .HasForeignKey(entity => new { entity.InputSnapshotId, entity.MeasurementPeriodId, entity.PolicyVersionId, entity.ProgramPolicyBindingId, entity.OrgUnitId, entity.ProgramVersionId, entity.AcademicYearStart })
            .HasPrincipalKey(entity => new { entity.Id, entity.MeasurementPeriodId, entity.PolicyVersionId, entity.ProgramPolicyBindingId, entity.OrgUnitId, entity.ProgramVersionId, entity.AcademicYearStart })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_snapshot_scope_policy");
        builder.HasOne(entity => entity.MeasurementPeriod).WithMany().HasForeignKey(entity => entity.MeasurementPeriodId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_measurement_period");
        builder.HasOne(entity => entity.PolicyVersion).WithMany().HasForeignKey(entity => entity.PolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_policy_version");
        builder.HasOne(entity => entity.ProgramPolicyBinding).WithMany().HasForeignKey(entity => entity.ProgramPolicyBindingId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_policy_binding");
        builder.HasOne(entity => entity.OrgUnit).WithMany().HasForeignKey(entity => entity.OrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_org_unit");
        builder.HasOne(entity => entity.ProgramVersion).WithMany().HasForeignKey(entity => entity.ProgramVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_program_version");
        builder.HasOne(entity => entity.RecalculatesBatch).WithMany().HasForeignKey(entity => entity.RecalculatesBatchId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_recalculates_batch");
        builder.HasOne(entity => entity.WorkflowInstance).WithOne().HasForeignKey<ResultBatch>(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_workflow_instance");
        builder.HasOne(entity => entity.SodPolicyVersion).WithMany().HasForeignKey(entity => entity.SodPolicyVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_result_batch_sod_policy_version");
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_result_batch_workflow_instance");
    }
}
